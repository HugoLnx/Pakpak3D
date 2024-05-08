using System;
using System.Collections.Generic;
using System.Linq;
using LnxArch;
using SensenToolkit;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostChase : MonoBehaviour
    {
        private static readonly HashSet<Vector2Int> s_allDirections = new()
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };
        [SerializeField] private GridBoard _grid;
        [SerializeField] private GridPathfinding _pathfinding;
        [SerializeField] private Transform _mainTarget;
        [SerializeField] private Transform _preferredWaypoint;
        [SerializeField] private float _minDistanceToIgnoreWaypoint = 1f;
        [SerializeField] private bool _ignoreNavmesh = false;
        private Vector3 _currentTargetPosition;
        private Vector2Int _direction;
        private FlyingMover _flying;

        [LnxInit]
        private void Init(
            FlyingMover flying
        )
        {
            _flying = flying;
            _flying.OnReachCell += ReachCellCallback;
        }

        private void Start()
        {
            UpdateDirection();
        }

        public void SetChasing(
            Transform mainTarget,
            Transform preferredWaypoint = null,
            float? minDistanceToIgnoreWaypoint = null,
            bool ignoreNavmesh = false)
        {
            _mainTarget = mainTarget;
            _preferredWaypoint = preferredWaypoint;
            if (minDistanceToIgnoreWaypoint.HasValue)
            {
                _minDistanceToIgnoreWaypoint = minDistanceToIgnoreWaypoint.Value;
            }
            _ignoreNavmesh = ignoreNavmesh;
            UpdateCurrentTargetPosition();
        }

        public float GetDistanceToTarget()
        {
            return Vector2.Distance(_flying.Position.XZ(), _currentTargetPosition.XZ());
        }

        public void ResumeChasing()
        {
            _flying.ResumeMoving();
        }

        public void PauseChasing()
        {
            _flying.PauseMoving();
        }

        private void ReachCellCallback()
        {
            UpdateDirection();
        }

        private void UpdateDirection()
        {
            UpdateCurrentTargetPosition();
            Vector3 position = _flying.Position;
            HashSet<Vector2Int> validDirections = GetAllNonBlockedDirectionsFrom(position);
            Vector2Int backwardsDirection = -_direction;
            validDirections.Remove(backwardsDirection);

            if (validDirections.Count == 0)
            {
                throw new System.Exception("No valid directions to move");
            }

            Vector2Int bestDirection = ChooseBestDirectionToTarget(validDirections);
            TurnTo(bestDirection);
        }

        private Vector2Int ChooseBestDirectionToTarget(IEnumerable<Vector2Int> directions)
        {
            Vector2? navmeshDirection = GetDirectionFromPathfinding();
            if (!navmeshDirection.HasValue && !_ignoreNavmesh)
            {
                Debug.LogWarning($"No navmesh direction found. Fallback to direction approximation.");
            }

            Vector2 targetDirection = navmeshDirection.HasValue
                ? navmeshDirection.Value
                : (_currentTargetPosition - _flying.Position).XZ().normalized;

            return directions
                .OrderBy(d => Vector2.Angle(d, targetDirection))
                .First();
        }

        private Vector2? GetDirectionFromPathfinding()
        {
            if (_ignoreNavmesh) return null;
            Vector2Int targetCell2d = _grid.GetClosestCell(_currentTargetPosition).XZ();
            Vector2? chosen = TryPathfindingTo(targetCell2d);
            if (chosen.HasValue)
            {
                return chosen;
            }
            foreach (Vector2Int direction in s_allDirections)
            {
                chosen = TryPathfindingTo(targetCell2d + direction);
                if (chosen.HasValue)
                {
                    return chosen;
                }
            }

            return chosen;
        }

        private Vector2? TryPathfindingTo(Vector2Int targetCell2d)
        {
            Vector2Int? direction = _pathfinding.GetNextStepDirection(
                start: _flying.Cell.XZ(),
                target: targetCell2d
            );

            if (!direction.HasValue)
            {
                return null;
            }

            return direction.Value.AsVector2Float();
        }

        private HashSet<Vector2Int> GetAllNonBlockedDirectionsFrom(Vector3 position)
        {
            HashSet<Vector2Int> nonBlockedDirections = new();
            foreach (Vector2Int direction in s_allDirections)
            {
                if (CanMoveTowardsOrGoUpAndMoveTowards(position, direction.X0Y()))
                {
                    nonBlockedDirections.Add(direction);
                }
            }

            return nonBlockedDirections;
        }

        private bool CanMoveTowardsOrGoUpAndMoveTowards(Vector3 position, Vector3Int direction)
        {
            if (_grid.CanMoveTowards(position, direction))
            {
                return true;
            }
            Vector3 upPosition = position + Vector3.up * _grid.CellSize;
            return _grid.CanMoveTowards(upPosition, direction);
        }

        private void TurnTo(Vector2Int direction)
        {
            _direction = direction;
            _flying.TurnTo(direction);
        }

        private void UpdateCurrentTargetPosition()
        {
            _currentTargetPosition = GetCurrentTargetPosition();
        }

        private Vector3 GetCurrentTargetPosition()
        {
            if (_preferredWaypoint == null)
            {
                // Debug.Log($"No preferred waypoint. Using main target: {_mainTarget.position}");
                return _mainTarget.position;
            }

            Vector2 targetPosition = _mainTarget.position.XZ();
            Vector2 waypointPosition = _preferredWaypoint.position.XZ();
            Vector2 myPosition = _flying.Position.XZ();

            float distanceToWaypoint = Vector2.Distance(myPosition, waypointPosition);
            if (distanceToWaypoint <= _minDistanceToIgnoreWaypoint)
            {
                // Debug.Log($"Distance to waypoint: {distanceToWaypoint} <= {_minDistanceToIgnoreWaypoint}. Ignoring waypoint.");
                return _mainTarget.position;
            }

            bool isWithinSegmentLimits = CalcDistanceFromSegmentToPoint(
                segmentStart: targetPosition,
                segmentEnd: waypointPosition,
                point: myPosition,
                out float segmentDistance
            );

            if (!isWithinSegmentLimits
                || segmentDistance <= _minDistanceToIgnoreWaypoint)
            {
                // Debug.Log($"Distance to waypoint SEGMENT: {segmentDistance} <= {_minDistanceToIgnoreWaypoint}. Ignoring waypoint.");
                return _mainTarget.position;
            }

            // Debug.Log($"Using waypoint: {waypointPosition}. segmentDistance:{segmentDistance} isWithinSegmentLimits:{isWithinSegmentLimits}");
            return _preferredWaypoint.position;
        }

        private bool CalcDistanceFromSegmentToPoint(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point, out float distance)
        {
            Vector2 segment = segmentEnd - segmentStart;
            Vector2 startToPoint = point - segmentStart;
            float segmentLength = segment.magnitude;
            Vector2 segmentDirection = segment / segmentLength;
            float projectionPointToSegment = Vector2.Dot(segmentDirection, startToPoint);
            bool isProjectionWithinSegmentLimits = projectionPointToSegment >= 0 && projectionPointToSegment <= segmentLength;

            Vector2 intersectionPoint = segmentDirection * projectionPointToSegment;
            float distanceToSegment = (intersectionPoint - startToPoint).magnitude;
            distance = distanceToSegment;

            return isProjectionWithinSegmentLimits;
        }
    }
}
