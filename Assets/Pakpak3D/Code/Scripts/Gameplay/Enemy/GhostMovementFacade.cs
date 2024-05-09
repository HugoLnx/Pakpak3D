using System.Collections;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostMovementFacade : MonoBehaviour
    {
        private Ghost _ghost;
        private GhostTrackLooper _trackLooper;
        private GhostTargetChase _targetedChase;
        private TrackProviderService _trackProvider;
        private GhostChaseConfigProvider _chaseConfigProvider;

        public event System.Action OnReachTrack;

        [LnxInit]
        private void Init(
            Ghost ghost,
            GhostTrackLooper trackLooper,
            GhostTargetChase targetedChase,
            TrackProviderService trackProvider,
            GhostChaseConfigProvider chaseConfigProvider
        )
        {
            _ghost = ghost;
            _trackLooper = trackLooper;
            _targetedChase = targetedChase;
            _trackProvider = trackProvider;
            _chaseConfigProvider = chaseConfigProvider;
            _trackLooper.OnReachTrack += () => OnReachTrack?.Invoke();
        }

        public void ChasePakpak()
        {
            _trackLooper.StopChasing();
            GhostChaseConfig cfg = _chaseConfigProvider.GetChaseConfig(_ghost.GhostType);
            _targetedChase.SetChasing(cfg.MainTarget, cfg.PreferredWaypoint);
        }

        public void LoopScatterTrack()
        {
            Transform scatterTrack = _trackProvider.GetGhostTrack(_ghost.GhostType);
            LoopThroughTrack(scatterTrack);
        }

        public void LoopBootTrack()
        {
            Transform bootTrack = _trackProvider.GetBootTrack();
            LoopThroughTrack(bootTrack);
        }

        public void StopChasing()
        {
            _targetedChase.PauseChasing();
            _trackLooper.StopChasing();
        }

        private void LoopThroughTrack(Transform track)
        {
            _targetedChase.PauseChasing();
            _trackLooper.EnsureChasing(track);
        }
    }
}
