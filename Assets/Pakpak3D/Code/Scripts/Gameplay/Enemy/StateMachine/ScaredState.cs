using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class ScaredState : EnemyFSMState
    {
        [SerializeField] private Material _scaredMaterial;
        [SerializeField] private Transform _scatterTrack;
        private GhostChaseTrack _chaseTrack;
        private MeshRenderer _meshRenderer;
        private Material _previousMaterial;

        [LnxInit]
        private void Init(GhostChaseTrack chaseTrack, MeshRenderer meshRenderer)
        {
            _chaseTrack = chaseTrack;
            _meshRenderer = meshRenderer;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            base.OnEnter(previousState);
            _chaseTrack.EnsureChasing(_scatterTrack);
            _previousMaterial = _meshRenderer.material;
            _meshRenderer.material = _scaredMaterial;
        }

        protected override void OnExit(EnemyFSMState nextState)
        {
            base.OnExit(nextState);
            _chaseTrack.StopChasing();
            _meshRenderer.material = _previousMaterial;
        }
    }
}
