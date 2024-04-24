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
        private bool _isJumping;

        [LnxInit]
        private void Init(Rigidbody rbody)
        {
            _rbody = rbody;
        }

        public void Jump()
        {
            if (_isJumping) return;
            _isJumping = true;
            StartCoroutine(JumpCoroutine());
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
            float jumpHeight = _grid.CellSize;
            yield return HeightVariationCoroutine(
                deltaHeight: jumpHeight,
                ease: Ease.OutSine,
                reverse: true
            );
        }

        private IEnumerator HeightVariationCoroutine(float deltaHeight, Ease ease, bool reverse = false)
        {
            float currentDelta = 0f;
            float startY = _rbody.position.y;
            float sign = reverse ? -1 : 1;
            Tween tween = DOTween.To(
                getter: () => currentDelta,
                setter: value =>
                {
                    currentDelta = value;
                    _rbody.position = new(
                        x: _rbody.position.x,
                        y: startY + (currentDelta * sign),
                        z: _rbody.position.z
                    );
                },
                endValue: deltaHeight,
                duration: deltaHeight / _jumpSpeed
            )
            .SetEase(ease)
            .SetUpdate(UpdateType.Fixed);

            yield return tween.WaitForCompletion();
        }
    }
}
