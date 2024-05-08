using LnxArch;
using Sensen.Components;
using UnityEngine;

namespace Pakpak3D
{
    public class HuntState : EnemyFSMState
    {
        [SerializeField] private Transform _scatterTrack;
        private GhostChaseTrack _chaseTrack;
        private GhostTargetChase _chasePakpak;
        private HuntInstructorService _instructor;

        [LnxInit]
        private void Init(
            GhostChaseTrack chaseTrack,
            GhostTargetChase chasePakpak,
            HuntInstructorService huntInstructor
        )
        {
            _chaseTrack = chaseTrack;
            _chasePakpak = chasePakpak;
            _instructor = huntInstructor;
            _instructor.OnInstructionChanged += OnInstructionChanged;
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
            switch (_instructor.Instruction)
            {
                case HuntInstruction.Hunt:
                    SwitchToHunt();
                    break;
                case HuntInstruction.Scatter:
                    SwitchToScatter();
                    break;
            }
        }

        private void SwitchToHunt()
        {
            _chaseTrack.StopChasing();
            _chasePakpak.EnsureChasing();
        }

        private void SwitchToScatter()
        {
            _chasePakpak.StopChasing();
            _chaseTrack.EnsureChasing(_scatterTrack);
        }

        private void StopChasing()
        {
            _chasePakpak.StopChasing();
            _chaseTrack.StopChasing();
        }
    }
}
