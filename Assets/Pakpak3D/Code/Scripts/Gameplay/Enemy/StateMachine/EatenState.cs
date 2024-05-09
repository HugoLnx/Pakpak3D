using System;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class EatenState : EnemyFSMState
    {
        [SerializeField] private Material _eatenMaterial;
        [SerializeField] private Transform _bootTrack;
        private EnemyFSMMachine _fsm;
        private GhostChaseTrack _chaseTrack;
        private MeshRenderer _meshRenderer;
        private Material _previousMaterial;

        [LnxInit]
        private void Init(
            EnemyFSMMachine fsm,
            GhostChaseTrack chaseTrack,
            MeshRenderer meshRenderer
        )
        {
            _fsm = fsm;
            _chaseTrack = chaseTrack;
            _meshRenderer = meshRenderer;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            base.OnEnter(previousState);
            _chaseTrack.OnReachTrack += OnReachBootTrack;
            _chaseTrack.EnsureChasing(_bootTrack);
            _previousMaterial = _meshRenderer.material;
            _meshRenderer.material = _eatenMaterial;
        }

        protected override void OnExit(EnemyFSMState nextState)
        {
            base.OnExit(nextState);
            _chaseTrack.StopChasing();
            _meshRenderer.material = _previousMaterial;
            _chaseTrack.OnReachTrack -= OnReachBootTrack;
        }

        private void OnReachBootTrack()
        {
            _fsm.SendMessage(_fsm.EnteredHouse);
        }
    }
}
