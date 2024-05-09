using System;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxService]
    public class TrackProviderService : MonoBehaviour
    {
        private Dictionary<GhostTypeSO, Transform> _tracks = new();
        private Transform _bootTrack;

        public Transform GetGhostTrack(GhostTypeSO ghostType)
        {
            if (!_tracks.ContainsKey(ghostType))
            {
                throw new Exception($"No track registered for ghost type {ghostType.name}");
            }

            return _tracks[ghostType];
        }

        public Transform GetBootTrack()
        {
            if (_bootTrack == null)
            {
                throw new Exception("No boot track registered");
            }
            return _bootTrack;
        }

        public void RegisterGhostTrack(GhostTypeSO ghostType, Transform track)
        {
            if (track == null) throw new Exception("Track can't be null");
            if (ghostType == null) throw new Exception("Ghost can't be null");
            _tracks.Add(ghostType, track);
        }

        public void RegisterBootTrack(Transform track)
        {
            Debug.Log($"[TrackProviderService.RegisterBootTrack] {this.transform.name}");
            _bootTrack = track;
        }
    }
}
