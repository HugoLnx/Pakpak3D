using System;
using System.Collections;
using UnityEngine;
using SensenToolkit;
using LnxArch;

namespace Pakpak3D
{
    public class Grid3DMovement : MonoBehaviour
    {
        [SerializeField] private GridBoard _grid;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private bool _forceCellSnapping = true;
        [SerializeField] private bool _ignoreHeightWhenSnapping = true;
        private MovingPhysics _movingPhysics;
        private Vector3Int _targetDirection = Vector3Int.forward;
        private Vector3? _currentMovement;
        private bool _isBlocked;

        public bool WasAskedToPause { get; private set; } = true;
        public bool IsPaused => WasAskedToPause && !IsMoving;
        public Vector3 Position => _movingPhysics.PositionPreview;
        public Vector3Int Cell => _grid.GetClosestCell(Position);
        public bool IsMoving => _currentMovement.HasValue;
        public Vector3Int? MovementDirection => IsMoving ? _targetDirection : null;

        public event Action OnReachCell;
        public event Action OnBlocked;
        public event Action OnUpdateTarget;

        [LnxInit]
        private void Init(MovingPhysics movingPhysics)
        {
            _movingPhysics = movingPhysics;
        }

        private void FixedUpdate()
        {
            if (_isBlocked || IsPaused) return;
            float step = _speed * Time.fixedDeltaTime;
            UpdateTargetMovement();
            while (IsMoving && step > 0)
            {
                step = ConsumeTargetMovementBy(step);
                UpdateTargetMovement();
            }
        }

        public void TurnTo(Vector3Int direction)
        {
            if (!Grid3DDirection.IsValidDirection(direction))
            {
                throw new ArgumentException($"Invalid direction: {direction}. It should be a normalized vector with only one non-zero component.");
            }
            _targetDirection = direction;
        }

        public void ResumeMoving()
        {
            _isBlocked = false;
            WasAskedToPause = false;
        }

        public void PauseMoving()
        {
            WasAskedToPause = true;
        }

        public Vector3 GetMovementEndPositionPreview()
        {
            if (!IsMoving) return Position;
            Vector3 endPosition = Position + _currentMovement.Value;
            if (!_forceCellSnapping) return endPosition;

            Vector3 endCellPosition = _grid.Cell3DToPosition(_grid.GetClosestCell(endPosition));
            if (!_ignoreHeightWhenSnapping) return endCellPosition;
            return endCellPosition.X0Z();
        }

        private void UpdateTargetMovement()
        {
            if (IsMoving || WasAskedToPause) return;

            Vector3 currentPosition = this.Position;
            _isBlocked = !_grid.CanMoveTowards(currentPosition, _targetDirection);
            if (_isBlocked)
            {
                _currentMovement = null;
                OnBlocked?.Invoke();
                OnUpdateTarget?.Invoke();
                return;
            }

            Vector3Int currentCell = _grid.GetClosestCell(currentPosition);
            _currentMovement = _targetDirection.AsVector3Float() * _grid.CellSize;
            OnUpdateTarget?.Invoke();
        }

        private float ConsumeTargetMovementBy(float step)
        {
            if (!IsMoving || _isBlocked) return 0;
            float remainingMovement = _currentMovement.Value.magnitude;

            float remainingStep = 0;
            if (step >= remainingMovement)
            {
                TranslateBy(_currentMovement.Value);
                _currentMovement = null;
                remainingStep = step - remainingMovement;
                ForceCellSnapping();
                OnReachCell?.Invoke();
            }
            else
            {
                Vector3 movementDirection = _currentMovement.Value / remainingMovement;
                TranslateBy(movementDirection * step);
                _currentMovement = movementDirection * (remainingMovement - step);
                remainingStep = 0;
            }

            return remainingStep;
        }

        private void ForceCellSnapping()
        {
            if (!_forceCellSnapping) return;
            Vector3Int cell = _grid.GetClosestCell(Position);
            Vector3 snapPosition = _grid.Cell3DToPosition(cell);
            Vector3 myPosition = Position;
            Vector3 snapOffset = snapPosition - myPosition;
            if (_ignoreHeightWhenSnapping)
            {
                snapOffset.y = 0;
            }
            TranslateBy(snapOffset);
        }

        private void TranslateBy(Vector3 offset)
        {
            this._movingPhysics.TranslateBy(offset);
        }
    }
}
