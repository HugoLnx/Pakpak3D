using Codice.CM.Common;
using LnxArch;
using Sensen.Components;
using UnityEngine;

namespace Pakpak3D
{

    public class HuntState : EnemyFSMState
    {
        [SerializeField] private Transform _scatterTrack;
        [SerializeField] private float _forceScatterIfCloserThan = -1f;
        private GhostChaseTrack _chaseTrack;
        private GhostTargetChase _chasePakpak;
        private HuntInstructorService _instructor;
        private TargetDistanceTracker _targetDistanceTracker;

        public bool HasToForceScatter => _forceScatterIfCloserThan > 0
            && _targetDistanceTracker.DistanceToTarget < _forceScatterIfCloserThan;

        [LnxInit]
        private void Init(
            GhostChaseTrack chaseTrack,
            GhostTargetChase chasePakpak,
            HuntInstructorService huntInstructor,
            TargetDistanceTracker targetDistanceTracker
        )
        {
            _chaseTrack = chaseTrack;
            _chasePakpak = chasePakpak;
            _instructor = huntInstructor;
            _instructor.OnInstructionChanged += OnInstructionChanged;
            _targetDistanceTracker = targetDistanceTracker;
            SetupDistanceTracker();
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            base.OnEnter(previousState);
            SwitchToCurrentInstruction();
        }

        protected override void OnExit(EnemyFSMState nextState)
        {
            base.OnExit(nextState);
            StopChasing();
        }

        private void OnInstructionChanged(HuntInstruction instruction)
        {
            SwitchToCurrentInstruction();
        }

        private void SwitchToCurrentInstruction()
        {
            if (!IsActive) return;
            switch (_instructor.Instruction)
            {
                case HuntInstruction.Attack:
                    SwitchToAttack();
                    break;
                case HuntInstruction.Scatter:
                    SwitchToScatter();
                    break;
            }
        }

        private void SwitchToAttack()
        {
            if (!IsActive) return;
            if (HasToForceScatter)
            {
                SwitchToScatter();
                return;
            }
            _chaseTrack.StopChasing();
            _chasePakpak.EnsureChasing();
        }

        private void SwitchToScatter()
        {
            if (!IsActive) return;
            _chasePakpak.StopChasing();
            _chaseTrack.EnsureChasing(_scatterTrack);
        }

        private void StopChasing()
        {
            _chasePakpak.StopChasing();
            _chaseTrack.StopChasing();
        }

        private void SetupDistanceTracker()
        {
            if (_forceScatterIfCloserThan > 0)
            {
                _targetDistanceTracker.SetDistanceCallbacks(
                    distance: _forceScatterIfCloserThan,
                    onEnter: EnableForceScatter,
                    onExit: DisableForceScatter
                );
            }
        }

        private void EnableForceScatter()
        {
            SwitchToScatter();
        }

        private void DisableForceScatter()
        {
            if (_instructor.Instruction == HuntInstruction.Attack)
            {
                SwitchToAttack();
            }
        }
    }
}
