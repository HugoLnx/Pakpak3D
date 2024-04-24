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
        private Vector2 _inputDirection = Vector2.down;
        private Vector2? _targetPosition;
        private bool _isMoving;

        [LnxInit]
        private void Init(Rigidbody rbody)
        {
            _rbody = rbody;
        }

        private IEnumerator Start()
        {
            yield return new WaitForFixedUpdate();
            _isMoving = true;
        }

        private void FixedUpdate()
        {
            if (!_isMoving) return;
            if (!_targetPosition.HasValue) UpdateTarget();
            Vector2 toTarget = _targetPosition.Value - _rbody.position.XZ();
            float targetDistance = toTarget.magnitude;
            Vector2 targetDirection = toTarget / targetDistance;
            float step = _speed * Time.fixedDeltaTime;

            if (step > targetDistance)
            {
                TranslateBy(toTarget);
                step -= targetDistance;
                UpdateTarget();
            }

            TranslateBy(targetDirection * step);
        }

        public void TurnTo(Vector2 direction)
        {
            _inputDirection = direction;
        }

        private void UpdateTarget()
        {
            Vector3 currentPosition = _rbody.position;
            Vector2Int currentCell = _grid.GetClosestCell(currentPosition).XY();
            Vector2Int targetCell = currentCell + _inputDirection.AsVector2Int();
            _targetPosition = _grid.Cell2DToPosition(targetCell);
        }

        private void TranslateBy(Vector2 offset)
        {
            _rbody.MovePosition(_rbody.position + offset.X0Y());
        }
    }
}
