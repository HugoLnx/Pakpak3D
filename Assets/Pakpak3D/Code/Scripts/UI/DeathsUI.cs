using LnxArch;
using UnityEngine;
using TMPro;

namespace Pakpak3D
{
    public class DeathsUI : MonoBehaviour
    {
        private LivesService _livesService;
        private TMP_Text _deathsText;

        [LnxInit]
        private void Init(
            TMP_Text deathsText,
            LivesService livesService
        )
        {
            _deathsText = deathsText;
            _livesService = livesService;
            if (_livesService != null)
            {
                _livesService.OnLoseLife += RefreshDeaths;
            }
        }

        private void Start()
        {
            RefreshDeaths();
        }

        private void RefreshDeaths()
        {
            _deathsText.text = _livesService == null ? "0" : _livesService.Deaths.ToString();
        }
    }
}
