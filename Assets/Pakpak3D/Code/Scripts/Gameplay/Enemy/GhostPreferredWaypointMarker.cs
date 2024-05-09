using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostPreferredWaypointMarker : MonoBehaviour
    {
        [SerializeField] private GhostTypeSO _ghostType;
        [LnxInit]
        private void Init(GhostChaseConfigProvider chaseConfigProvider)
        {
            chaseConfigProvider.RegisterPreferredWaypointForGhost(_ghostType, transform);
        }
    }
}
