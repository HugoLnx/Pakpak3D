using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;
using TMPro;
using System;

namespace Pakpak3D
{
    public class ScoreUI : MonoBehaviour
    {
        private ScoreService _scoreService;
        private TMP_Text _scoreText;

        [LnxInit]
        private void Init(
            TMP_Text scoreText,
            ScoreService scoreService
        )
        {
            _scoreText = scoreText;
            _scoreService = scoreService;
            if (_scoreService != null)
            {
                _scoreService.OnScoreChanged += (total, _) => UpdateScore(total);
            }
        }

        private void Start()
        {
            UpdateScore(_scoreService == null ? 0 : _scoreService.Score);
        }

        private void UpdateScore(int total)
        {
            _scoreText.text = total.ToString();
        }
    }
}
