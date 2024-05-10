using System;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class GhostChaseConfigProvider : MonoBehaviour
    {
        private Dictionary<GhostTypeSO, GhostChaseConfig> _chaseConfigs = new();
        private GhostChaseConfig _defaultConfig = new();
        private Transform _mainTarget;

        public GhostChaseConfig GetChaseConfig(GhostTypeSO ghostType)
        {
            return _chaseConfigs.GetValueOrDefault(ghostType, _defaultConfig);
        }

        public void RegisterMainTarget(Transform target)
        {
            _mainTarget = target;
            UpdateMainTargetOnAll();
        }

        public void RegisterPreferredWaypointForGhost(GhostTypeSO ghostType, Transform waypoint)
        {
            _chaseConfigs.TryAdd(ghostType, new GhostChaseConfig()
            {
                MainTarget = _mainTarget,
                PreferredWaypoint = waypoint
            });
            _chaseConfigs[ghostType].PreferredWaypoint = waypoint;
            UpdateMainTargetOnAll();
        }

        private void UpdateMainTargetOnAll()
        {
            foreach (GhostChaseConfig chaseConfig in _chaseConfigs.Values)
            {
                chaseConfig.MainTarget = _mainTarget;
            }
            _defaultConfig.MainTarget = _mainTarget;
        }
    }
}
