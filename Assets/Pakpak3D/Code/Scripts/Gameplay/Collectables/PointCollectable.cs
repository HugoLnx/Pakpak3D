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
        [SerializeField] private float _scoreMultiplier = 1.0f;
        public int ScoreValue => Mathf.RoundToInt(_scoreValue * _scoreMultiplier);
        public Collectable Collectable { get; private set; }
        private ScoreService _scoreService;

        [LnxInit]
        private void Init(Collectable collectable, ScoreService scoreService)
        {
            Collectable = collectable;
            Collectable.OnCollected += Collect;
            _scoreService = scoreService;
        }

        public void SetScoreMultiplier(float multiplier)
        {
            _scoreMultiplier = multiplier;
        }

        private void Collect()
        {
            _scoreService.AddScore(ScoreValue);
        }
    }
}
