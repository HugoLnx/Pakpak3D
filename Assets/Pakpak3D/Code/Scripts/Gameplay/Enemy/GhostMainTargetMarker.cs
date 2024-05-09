using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostMainTargetMarker : MonoBehaviour
    {
        [LnxInit]
        private void Init(GhostChaseConfigProvider chaseConfigProvider)
        {
            chaseConfigProvider.RegisterMainTarget(transform);
        }
    }
}
