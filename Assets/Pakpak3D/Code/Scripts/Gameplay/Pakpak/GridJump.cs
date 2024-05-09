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
        [SerializeField] private float _jumpSpeed = 5f;
        [SerializeField] private float _topHeightDuration = 0.5f;
        private GridBoard _grid;
        private Grid2DMovement _movement;
        private MovingPhysics _moving;
        private bool _isJumping;
        private bool _isFalling;
        private Vector3 Position => _moving.PositionPreview;
        public event Action AfterJumpRise;

        [LnxInit]
        private void Init(
            Grid2DMovement movement,
            MovingPhysics moving,
            [FromParentEntity] GridBoard grid
        )
        {
            _movement = movement;
            _moving = moving;
            _grid = grid;
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
            yield return new WaitForFixedUpdate();
            AfterJumpRise?.Invoke();
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
                x: Position.x,
                y: cellPosition.y,
                z: Position.z
            );
            bool hasSomething = !_grid.CanFall(position);
            return hasSomething;
        }

        private bool HasSomethingBellowMovementTarget()
        {
            if (!_movement.IsMoving) return false;
            Vector2 targetPosition = _movement.GetMovementEndPositionPreview();
            Vector3Int cell = _grid.GetClosestCell(Position);
            Vector3 cellPosition = _grid.Cell3DToPosition(cell);
            Vector3 position = new(
                x: targetPosition.x,
                y: cellPosition.y,
                z: targetPosition.y
            );
            bool hasSomething = !_grid.CanFall(position);
            return hasSomething;
        }
    }
}
