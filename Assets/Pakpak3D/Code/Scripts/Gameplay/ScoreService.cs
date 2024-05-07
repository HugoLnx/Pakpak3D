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
        public int Score => _score;
        public delegate void ScoreChangedHandler(int total, int diff);
        public event ScoreChangedHandler OnScoreChanged;

        public void AddScore(int score)
        {
            _score += score;
            OnScoreChanged?.Invoke(_score, score);
        }
    }
}
