using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class BootTrackMarker : MonoBehaviour
    {
        [LnxInit]
        private void Init(TrackProviderService trackProvider)
        {
            Debug.Log($"[BootTrackMarker.Init] {this.transform.name}");
            trackProvider.RegisterBootTrack(this.transform);
        }
    }
}
