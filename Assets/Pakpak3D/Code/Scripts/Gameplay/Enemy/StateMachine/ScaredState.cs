using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class ScaredState : EnemyFSMState
    {
        [SerializeField] private Material _scaredMaterial;
        [SerializeField] private ParticleSystem _explosionVfx;
        private GhostMovementFacade _movementFacade;
        private MeshRenderer _meshRenderer;
        private Material _previousMaterial;

        [LnxInit]
        private void Init(GhostMovementFacade movementFacade, MeshRenderer meshRenderer)
        {
            _movementFacade = movementFacade;
            _meshRenderer = meshRenderer;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            base.OnEnter(previousState);
            _movementFacade.LoopScatterTrack();
            _previousMaterial = _meshRenderer.material;
            _meshRenderer.material = _scaredMaterial;
            // _explosionVfx.Play();
        }

        protected override void OnExit(EnemyFSMState nextState)
        {
            base.OnExit(nextState);
            _movementFacade.StopChasing();
            _meshRenderer.material = _previousMaterial;
        }
    }
}
