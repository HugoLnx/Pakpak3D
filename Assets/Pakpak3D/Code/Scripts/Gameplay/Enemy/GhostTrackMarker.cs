using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostTrackMarker : MonoBehaviour
    {
        [SerializeField] private GhostTypeSO _ghostType;

        [LnxInit]
        private void Init(TrackProviderService trackProvider)
        {
            trackProvider.RegisterGhostTrack(_ghostType, this.transform);
        }
    }
}
