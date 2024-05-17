using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using LnxArch;
using Sensen.Toolkit;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

namespace Pakpak3D
{
    public class Pakpak : MonoBehaviour
    {
        private const string ShaderColorProperty = "_BaseColor";
        public const string TAG = "Pakpak";
        private const int HitScoreDecrease = 100;
        [SerializeField] private ParticleSystem _redExplosionVfx;
        [SerializeField] private float _hitCooldown = 7f;
        private Grid3DMovement _movement3d;
        private LivesService _livesService;
        private ScoreService _scoreService;
        private List<MeshRenderer> _meshRenderers;
        private Dictionary<MeshRenderer, (Material material, Color color)> _meshProps = new();
        public bool IsInCooldown { get; private set; }

        private Tween _tween;

        [LnxInit]
        private void Init(
            LivesService livesService,
            ScoreService scoreService,
            Grid3DMovement movement3d,
            List<MeshRenderer> meshRenderers
        )
        {
            _movement3d = movement3d;
            _livesService = livesService;
            _scoreService = scoreService;
            _meshRenderers = meshRenderers
                .Where(m => m.enabled && m.gameObject.activeInHierarchy)
                .ToList();

            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                Material material = meshRenderer.material;
                Color color = material.GetColor(ShaderColorProperty);
                _meshProps[meshRenderer] = (material, color);
            }
        }

        public void SetSpeed(float speed)
        {
            _movement3d.SetSpeed(speed);
        }

        public void TakeHit()
        {
            if (IsInCooldown) return;
            _redExplosionVfx.Play();
            _livesService.IncreaseDeaths();
            _scoreService.DecreaseScoreBy(HitScoreDecrease);
            StartCoroutine(WaitCooldown());
        }

        private IEnumerator WaitCooldown()
        {
            IsInCooldown = true;
            _tween = SimpleTweening.Flash(
                action: SetCooldownFlash,
                durationIn: 0.25f,
                durationOut: 0.25f
            )
            .SetLoops(-1, LoopType.Restart);
            yield return new WaitForSeconds(_hitCooldown);
            _tween.SmoothRewind();
            yield return _tween.WaitForRewind();
            SetCooldownFlash(0f);
            IsInCooldown = false;
            _tween = null;
        }

        private void SetCooldownFlash(float t)
        {
            foreach (MeshRenderer meshRenderer in _meshRenderers)
            {
                (Material material, Color originalColor) = _meshProps[meshRenderer];
                Color flashColor = Color.white;
                var newColor = Color.Lerp(originalColor, flashColor, t);
                material.SetColor(ShaderColorProperty, newColor);
            }
        }
    }
}
