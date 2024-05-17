using System;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class EatenState : EnemyFSMState
    {
        [SerializeField] private int _score = 235;
        [SerializeField] private Material _eatenMaterial;
        [SerializeField] private ParticleSystem _explosionVfx;
        private EnemyFSMMachine _fsm;
        private GhostMovementFacade _movementFacade;
        private MeshRenderer _meshRenderer;
        private ScoreService _scoreService;
        private Material _previousMaterial;

        [LnxInit]
        private void Init(
            EnemyFSMMachine fsm,
            GhostMovementFacade movementFacade,
            MeshRenderer meshRenderer,
            ScoreService scoreService
        )
        {
            _fsm = fsm;
            _movementFacade = movementFacade;
            _meshRenderer = meshRenderer;
            _scoreService = scoreService;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            base.OnEnter(previousState);
            _movementFacade.OnReachTrack += OnReachBootTrack;
            _movementFacade.LoopBootTrack();
            _previousMaterial = _meshRenderer.material;
            _meshRenderer.material = _eatenMaterial;
            bool wasEaten = previousState == _fsm.Scared;
            if (wasEaten)
            {
                _explosionVfx.Play();
                _scoreService.AddScore(_score);
            }
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
