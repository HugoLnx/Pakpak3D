using LnxArch;
using UnityEngine;
using TMPro;

namespace Pakpak3D
{
    public class LevelUI : MonoBehaviour
    {
        private GameLoopService _loopService;
        private TMP_Text _levelText;

        [LnxInit]
        private void Init(
            TMP_Text levelText,
            GameLoopService loopService
        )
        {
            _levelText = levelText;
            _loopService = loopService;
            if (_loopService != null)
            {
                _loopService.OnBeforeWaveStart += RefreshLevel;
            }
        }

        private void Start()
        {
            RefreshLevel();
        }

        private void RefreshLevel()
        {
            if (_loopService == null)
            {
                _levelText.text = "1A";
            }
            else
            {
                int level = _loopService.CurrentLevel;
                int waveInx = _loopService.CurrentWaveInx;
                string waveLetter = waveInx switch { 0 => "A", 1 => "B", 2 => "C", 3 => "D", 4 => "E", 5 => "F", 6 => "G", 7 => "H", 8 => "I", 9 => "J", _ => "K" };
                _levelText.text = $"{level}{waveLetter}";
            }
        }
    }
}
