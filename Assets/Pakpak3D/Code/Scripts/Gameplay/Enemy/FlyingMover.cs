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
        private GridBoard _grid;
        private Grid3DMovement _movement;
        private Vector2Int _targetDirection;

        public Vector3 Position => _movement.Position;
        public Vector3Int Cell => _movement.Cell;


        public event Action OnReachCell;

        [LnxInit]
        private void Init(
            Grid3DMovement movement,
            [FromParentEntity] GridBoard grid
        )
        {
            _movement = movement;
            _movement.OnBlocked += BlockedCallback;
            _movement.OnReachCell += ReachCellCallback;
            _grid = grid;
        }

        public void TurnTo(Vector2Int direction)
        {
            _targetDirection = direction;
            _movement.TurnTo(direction.X0Y());
            ResumeMoving();
        }

        public void ResumeMoving()
        {
            _movement.ResumeMoving();
        }

        public void PauseMoving()
        {
            _movement.PauseMoving();
        }

        private void BlockedCallback()
        {
            _movement.TurnTo(Vector3Int.up);
            ResumeMoving();
        }

        private void ReachCellCallback()
        {
            OnReachCell?.Invoke();
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
            // Vector3 nextTargetPosition = _grid.Cell3DToPosition(nextCellCandidate);
            // bool nextWillFall = _grid.CanMoveTowards(nextTargetPosition, Vector3Int.down);
            int nextAboveGroundY = _grid.GetCellAboveGround(nextCellCandidate.XZ()).Value.y;
            bool nextWillFall = nextAboveGroundY < _movement.Cell.y;
            return canFall && nextWillFall;
        }
    }
}
