using System;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class EatenState : EnemyFSMState
    {
        [SerializeField] private Material _eatenMaterial;
        private EnemyFSMMachine _fsm;
        private GhostMovementFacade _movementFacade;
        private MeshRenderer _meshRenderer;
        private Material _previousMaterial;

        [LnxInit]
        private void Init(
            EnemyFSMMachine fsm,
            GhostMovementFacade movementFacade,
            MeshRenderer meshRenderer
        )
        {
            _fsm = fsm;
            _movementFacade = movementFacade;
            _meshRenderer = meshRenderer;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            base.OnEnter(previousState);
            _movementFacade.OnReachTrack += OnReachBootTrack;
            _movementFacade.LoopBootTrack();
            _previousMaterial = _meshRenderer.material;
            _meshRenderer.material = _eatenMaterial;
        }

        protected override void OnExit(EnemyFSMState nextState)
        {
            base.OnExit(nextState);
            _movementFacade.StopChasing();
            _meshRenderer.material = _previousMaterial;
            _movementFacade.OnReachTrack -= OnReachBootTrack;
        }

        private void OnReachBootTrack()
        {
            _fsm.SendMessage(_fsm.EnteredHouse);
        }
    }
}
