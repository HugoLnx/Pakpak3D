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
        private Rigidbody _rbody;
        private MovingPhysics _movingPhysics;
        private Vector3Int _targetDirection = Vector3Int.forward;
        private Vector3Int? _targetCell;
        private bool _isMoving;
        public Vector3 Position => _rbody.position;
        public Vector3Int Cell => _grid.GetClosestCell(Position);
        public bool HasTarget => _targetCell.HasValue;
        public Vector3Int? TargetCell => _targetCell;
        public bool IsMoving => _isMoving;

        public event Action<Vector3Int> OnSnapInCell;
        public event Action OnBlocked;

        [LnxInit]
        private void Init(Rigidbody rbody, MovingPhysics movingPhysics)
        {
            _rbody = rbody;
            _movingPhysics = movingPhysics;
        }

        private void FixedUpdate()
        {
            if (!IsMoving) return;
            float step = _speed * Time.fixedDeltaTime;
            UpdateTarget();
            while (HasTarget && step > 0)
            {
                step = ConsumeStepTowardsTargetBy(step);
                UpdateTarget();
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
            _isMoving = true;
        }

        public void StopMoving()
        {
            _isMoving = false;
        }

        private void UpdateTarget()
        {
            if (HasTarget) return;

            Vector3 currentPosition = this.Position;
            bool isBlocked = !_grid.CanMoveTowards(currentPosition, _targetDirection);
            if (isBlocked)
            {
                _targetCell = null;
                StopMoving();
                OnBlocked?.Invoke();
                return;
            }

            Vector3Int currentCell = _grid.GetClosestCell(currentPosition);
            Vector3Int cellDirection = _targetDirection;
            _targetCell = currentCell + cellDirection;
        }

        private float ConsumeStepTowardsTargetBy(float step)
        {
            if (!HasTarget || !IsMoving) return 0;
            Vector3 targetPosition = _grid.Cell3DToPosition(_targetCell.Value);
            Vector3 toTarget = targetPosition - this.Position;
            float targetDistance = toTarget.magnitude;

            float remainingStep = 0;
            if (step >= targetDistance)
            {
                TranslateBy(toTarget);
                _targetCell = null;
                remainingStep = step - targetDistance;
                OnSnapInCell?.Invoke(_grid.GetClosestCell(this.Position));
            }
            else
            {
                Vector3 targetDirection = toTarget / targetDistance;
                TranslateBy(targetDirection * step);
                remainingStep = 0;
            }

            return remainingStep;
        }

        private void TranslateBy(Vector3 offset)
        {
            this._movingPhysics.TranslateBy(offset);
        }
    }
}
