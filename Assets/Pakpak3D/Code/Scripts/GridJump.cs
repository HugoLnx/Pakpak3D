using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;
using DG.Tweening;

namespace Pakpak3D
{
    public class GridJump : MonoBehaviour
    {
        [SerializeField] private GridBoard _grid;
        [SerializeField] private float _jumpSpeed = 5f;
        [SerializeField] private float _topHeightDuration = 0.5f;
        private Rigidbody _rbody;
        private GridMovement _movement;
        private MovingPhysics _moving;
        private bool _isJumping;
        private bool _isFalling;
        private Vector3 Position => _moving.PositionPreview;

        [LnxInit]
        private void Init(Rigidbody rbody, GridMovement movement, MovingPhysics moving)
        {
            _rbody = rbody;
            _movement = movement;
            _moving = moving;
        }

        public void Jump()
        {
            if (_isJumping || _isFalling) return;
            _isJumping = true;
            StartCoroutine(JumpCoroutine());
        }

        public void Fall()
        {
            if (_isJumping || _isFalling) return;
            _isFalling = true;
            StartCoroutine(FallCoroutine());
        }

        private IEnumerator JumpCoroutine()
        {
            yield return RiseCoroutine();
            yield return new WaitForSeconds(_topHeightDuration);
            yield return FallCoroutine();
            _isJumping = false;
        }

        private IEnumerator RiseCoroutine()
        {
            float jumpHeight = _grid.CellSize;
            yield return HeightVariationCoroutine(
                deltaHeight: jumpHeight,
                ease: Ease.OutQuad
            );
        }

        private IEnumerator FallCoroutine()
        {
            if (HasSomethingBellow() || HasSomethingBellowMovementTarget())
            {
                _isFalling = false;
                yield break;
            }

            float jumpHeight = _grid.CellSize;
            yield return HeightVariationCoroutine(
                deltaHeight: jumpHeight,
                ease: Ease.OutSine,
                reverse: true
            );
            _isFalling = false;
        }

        private IEnumerator HeightVariationCoroutine(float deltaHeight, Ease ease, bool reverse = false)
        {
            float currentDelta = 0f;
            float startY = Position.y;
            float sign = reverse ? -1 : 1;
            Tween tween = DOTween.To(
                getter: () => currentDelta,
                setter: value =>
                {
                    float deltaDiff = value - currentDelta;
                    currentDelta = value;
                    Vector3 offset = deltaDiff * sign * Vector3.up;
                    _moving.TranslateBy(offset);
                },
                endValue: deltaHeight,
                duration: deltaHeight / _jumpSpeed
            )
            .SetEase(ease)
            .SetUpdate(UpdateType.Fixed);

            yield return tween.WaitForCompletion();
        }

        private bool HasSomethingBellow()
        {
            Vector3Int cell = _grid.GetClosestCell(Position);
            Vector3 cellPosition = _grid.Cell3DToPosition(cell);
            Vector3 position = new(
                x: _rbody.position.x,
                y: cellPosition.y,
                z: _rbody.position.z
            );
            Debug.Log($"HasSomethingBellow from: {position}");
            return !_grid.CanFall(position);
        }

        private bool HasSomethingBellowMovementTarget()
        {
            if (!_movement.HasTarget) return false;
            Vector2 targetPosition = _movement.Target.Value;
            Vector3Int cell = _grid.GetClosestCell(Position);
            Vector3 cellPosition = _grid.Cell3DToPosition(cell);
            Vector3 position = new(
                x: targetPosition.x,
                y: cellPosition.y,
                z: targetPosition.y
            );
            Debug.Log($"HasSomethingBellowMovementTarget from:{position}");
            return !_grid.CanFall(position);
        }
    }
}
