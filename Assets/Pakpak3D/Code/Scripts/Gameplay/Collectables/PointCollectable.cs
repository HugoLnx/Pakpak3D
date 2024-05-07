using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class PointCollectable : MonoBehaviour
    {
        [SerializeField] private int _scoreValue = 15;
        private Collectable _collectable;
        private ScoreService _scoreService;

        [LnxInit]
        private void Init(Collectable collectable, ScoreService scoreService)
        {
            _collectable = collectable;
            _collectable.OnCollected += Collect;
            _scoreService = scoreService;
        }

        private void Collect()
        {
            _scoreService?.AddScore(_scoreValue);
        }
    }
}
