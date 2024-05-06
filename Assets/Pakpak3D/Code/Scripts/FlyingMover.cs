using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit;
using UnityEngine;

namespace Pakpak3D
{
    public class FlyingMover : MonoBehaviour
    {
        [SerializeField] private GridBoard _grid;
        private Grid3DMovement _movement;
        private Vector2Int _targetDirection;
        public event Action<Vector3Int> OnSnapInCell;

        [LnxInit]
        private void Init(Grid3DMovement movement)
        {
            _movement = movement;
            _movement.OnBlocked += BlockedCallback;
            _movement.OnSnapInCell += SnapInCellCallback;
            _movement.StopMoving();
        }

        public void TurnTo(Vector2Int direction)
        {
            _targetDirection = direction;
            _movement.TurnTo(direction.X0Y());
            _movement.ResumeMoving();
        }

        public void StopMoving()
        {
            _movement.StopMoving();
        }

        private void BlockedCallback()
        {
            _movement.TurnTo(Vector3Int.up);
            _movement.ResumeMoving();
        }

        private void SnapInCellCallback(Vector3Int cell)
        {
            OnSnapInCell?.Invoke(cell);
            Vector3Int turnToDirection = HasToFall()
                ? Vector3Int.down
                : _targetDirection.X0Y();
            _movement.TurnTo(turnToDirection);
        }

        private bool HasToFall()
        {
            bool canFall = _grid.CanMoveTowards(_movement.Position, Vector3Int.down);
            if (!canFall) return false;

            Vector3Int nextCellCandidate = _movement.Cell + _targetDirection.X0Y();
            Vector3 nextTargetPosition = _grid.Cell3DToPosition(nextCellCandidate);
            bool nextWillFall = _grid.CanMoveTowards(nextTargetPosition, Vector3Int.down);
            return canFall && nextWillFall;
        }
    }
}
