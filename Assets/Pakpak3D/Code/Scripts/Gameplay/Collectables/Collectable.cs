using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LnxArch;
using SensenToolkit.Sensors;
using UnityEngine;

namespace Pakpak3D
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private float _floatDuration = 1f;
        [SerializeField] private float _floatYOffset = 0.5f;
        private Tween _tween;
        private MeshRenderer _mesh;
        private float _meshInitialLocalY;

        public event Action OnCollected;
        [LnxInit]
        private void Init(TriggerSensor sensor, MeshRenderer mesh)
        {
            _mesh = mesh;
            sensor.OnSensedEnter += OnTriggerEnter;
        }

        private void OnEnable()
        {
            _meshInitialLocalY = _mesh.transform.localPosition.y;
            StartFloating();
        }

        private void OnDisable()
        {
            StopFloating();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(Pakpak.TAG)) return;
            Collect();
        }

        public void Collect()
        {
            OnCollected?.Invoke();
            Destroy(gameObject);
        }

        private void StartFloating()
        {
            if (_tween != null) return;
            _tween = _mesh.transform
                .DOLocalMoveY(_meshInitialLocalY + _floatYOffset, _floatDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void StopFloating()
        {
            if (_tween == null) return;
            _tween.Rewind();
            _tween.Kill();
            _tween = null;
        }
    }
}
