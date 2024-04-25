using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pakpak3D
{
    public class GridMovement : MonoBehaviour
    {
        [SerializeField] private GridBoard _grid;
        [SerializeField] private float _speed = 5f;
        private Rigidbody _rbody;
        private MovingPhysics _movingPhysics;
        private Vector2 _inputDirection = Vector2.down;
        private Vector2? _targetPosition;
        private bool _isMoving;
        private bool _isBlocked;
        private bool CanNotMove => _isBlocked || !_isMoving;
        private Vector3 Position => _movingPhysics.PositionPreview;
        public bool HasTarget => _targetPosition.HasValue;
        public Vector2? Target => _targetPosition;

        public event Action<Vector2Int> OnSnapInCell;

        [LnxInit]
        private void Init(Rigidbody rbody, MovingPhysics movingPhysics)
        {
            _rbody = rbody;
            _movingPhysics = movingPhysics;
        }

        private IEnumerator Start()
        {
            StartCoroutine(FixIfFrozen());
            yield return new WaitForFixedUpdate();
            _isMoving = true;
        }

        private void FixedUpdate()
        {
            if (CanNotMove) return;

            float step = _speed * Time.fixedDeltaTime;
            UpdateTarget();
            while (HasTarget && step > 0)
            {
                step = ConsumeStepTowardsTargetBy(step);
                UpdateTarget();
            }
        }

        public void TurnTo(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                Debug.LogWarning($"!!! TurnTo: zero direction");
                return;
            }
            _inputDirection = direction;
            _isBlocked = false;
        }

        private void UpdateTarget()
        {
            if (HasTarget) return;

            Vector3 currentPosition = this.Position;
            _isBlocked = !_grid.CanMoveTowards(currentPosition, _inputDirection.AsVector2Int());
            if (_isBlocked)
            {
                // Debug.Log($"IsBlocked. from:{currentPosition} dir:{_inputDirection}");
                _targetPosition = null;
                return;
            }

            Vector2Int currentCell = _grid.GetClosestCell(currentPosition).XY();
            Vector2Int cellDirection = _inputDirection.AsVector2Int();
            Vector2Int targetCell = currentCell + cellDirection;
            _targetPosition = _grid.Cell2DToPosition(targetCell);
            // Debug.Log($"Target Selected cell:{targetCell} pos:{_targetPosition}");
        }

        private float ConsumeStepTowardsTargetBy(float step)
        {
            if (!HasTarget || CanNotMove) return 0;
            Vector2 toTarget = _targetPosition.Value - this.Position.XZ();
            float targetDistance = toTarget.magnitude;

            float remainingStep = 0;
            if (step >= targetDistance)
            {
                TranslateBy(toTarget);
                _targetPosition = null;
                remainingStep = step - targetDistance;
                OnSnapInCell?.Invoke(_grid.GetClosestCell(this.Position).XY());
            }
            else
            {
                Vector2 targetDirection = toTarget / targetDistance;
                TranslateBy(targetDirection * step);
                remainingStep = 0;
            }

            return remainingStep;
        }

        private void TranslateBy(Vector2 offset)
        {
            this._movingPhysics.TranslateBy(offset.X0Y());
        }

        private WaitForSeconds WaitCheckThrottling = new(0.35f);
        private WaitForFixedUpdate WaitFixedUpdate = new();
        private WaitForEndOfFrame WaitFrame = new();
        private IEnumerator FixIfFrozen()
        {
            while (true)
            {
                yield return WaitCheckThrottling;
                if (!HasTarget) continue;
                Vector3 lastPosition = this.transform.position;
                yield return WaitFixedUpdate;
                yield return WaitFrame;
                if (lastPosition == this.transform.position && HasTarget)
                {
                    Debug.LogWarning($"FixIfFrozen: {lastPosition}");
                    _targetPosition = null;
                }
            }
        }
    }
}
