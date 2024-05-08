using System;
using System.Collections;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class BootState : EnemyFSMState
    {
        [SerializeField] private Transform _bootTrack;
        [SerializeField] private float _bootTime = 5f;
        private EnemyFSMMachine _fsm;
        private GhostChaseTrack _chaseTrack;

        [LnxInit]
        private void Init(EnemyFSMMachine fsm, GhostChaseTrack chaseTrack)
        {
            _fsm = fsm;
            _chaseTrack = chaseTrack;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            StopAllCoroutines();
            StartCoroutine(WaitBoot());
            ChaseBootTrack();
        }

        protected override void OnExit(EnemyFSMState nextState)
        {
            StopAllCoroutines();
            _chaseTrack.StopChasing();
        }

        private void ChaseBootTrack()
        {
            _chaseTrack.EnsureChasing(_bootTrack);
        }

        private IEnumerator WaitBoot()
        {
            yield return new WaitForSeconds(_bootTime);
            _fsm.SendMessage(_fsm.TimerEnded);
        }
    }
}
