using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class ScoreService : MonoBehaviour
    {
        private int _score;
        private GameLoopService _loopService;
        private LivesService _livesService;

        public int Score => _score;
        public int Deaths => _livesService.Deaths;
        public int LevelInx => _loopService.CurrentLevel - 1;
        public int WaveInx => _loopService.CurrentWaveInx;
        public delegate void ScoreChangedHandler(int total, int diff);
        public event ScoreChangedHandler OnScoreChanged;

        [LnxInit]
        private void Init(GameLoopService loopService, LivesService livesService)
        {
            _loopService = loopService;
            _livesService = livesService;
        }

        public void AddScore(int scoreGain)
        {
            ChangeScore(_score + BoostScoreGain(scoreGain));
        }

        public void DecreaseScoreBy(int scoreLoss)
        {
            ChangeScore(_score - scoreLoss);
        }

        private void ChangeScore(int score)
        {
            _score = score;
            OnScoreChanged?.Invoke(_score, score);
        }

        private int BoostScoreGain(int score)
        {
            float levelBonus = 1.5f * LevelInx;
            float waveBonus = 2f * WaveInx;
            float bonus = Mathf.Max(1f, levelBonus + waveBonus);
            float multiplier = bonus / (Deaths + 1f);
            multiplier = Mathf.Max(0.1f, multiplier);
            return Mathf.RoundToInt(score * multiplier);
        }
    }
}
