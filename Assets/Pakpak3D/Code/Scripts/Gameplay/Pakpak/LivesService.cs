using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class LivesService : MonoBehaviour
    {
        [field: SerializeField] public int Deaths { get; private set; }
        public event System.Action OnLoseLife;

        public void IncreaseDeaths()
        {
            Deaths++;
            OnLoseLife?.Invoke();
        }
    }
}
