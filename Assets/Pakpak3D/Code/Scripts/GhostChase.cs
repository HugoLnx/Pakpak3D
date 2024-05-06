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
        [SerializeField] private Transform _target;
        private Transform MainTarget => _target;
        private Vector2Int _direction;
        private FlyingMover _flying;

        [LnxInit]
        private void Init(
            FlyingMover flying
        )
        {
            _flying = flying;
            _flying.OnSnapInCell += SnapInCellCallback;
        }

        private void Start()
        {
            UpdateDirection();
        }

        private void SnapInCellCallback(Vector3Int cell)
        {
            UpdateDirection();
        }

        private void UpdateDirection()
        {

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
            if (!navmeshDirection.HasValue)
            {
                Debug.LogWarning($"No navmesh direction found. Fallback to direction approximation.");
            }

            Vector2 targetDirection = navmeshDirection.HasValue
                ? navmeshDirection.Value
                : (MainTarget.position - _flying.Position).XZ().normalized;

            return directions
                .OrderBy(d => Vector2.Angle(d, targetDirection))
                .First();
        }

        private Vector2? GetDirectionFromPathfinding()
        {
            Vector2Int targetCell2d = _grid.GetClosestCell(MainTarget.position).XZ();
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
    }
}
