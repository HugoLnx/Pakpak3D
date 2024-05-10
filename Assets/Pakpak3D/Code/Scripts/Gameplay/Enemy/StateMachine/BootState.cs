using System;
using System.Collections;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class BootState : EnemyFSMState
    {
        [SerializeField] private float _bootMinTime = 3f;
        [SerializeField] private float _bootMaxTime = 12f;
        private EnemyFSMMachine _fsm;
        private GhostMovementFacade _movementFacade;

        [LnxInit]
        private void Init(EnemyFSMMachine fsm, GhostMovementFacade movementFacade)
        {
            _fsm = fsm;
            _movementFacade = movementFacade;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            StopAllCoroutines();
            StartCoroutine(WaitBoot());
            _movementFacade.LoopBootTrack();
        }

        protected override void OnExit(EnemyFSMState nextState)
        {
            StopAllCoroutines();
            _movementFacade.StopChasing();
        }

        private IEnumerator WaitBoot()
        {
            float bootTime = UnityEngine.Random.Range(_bootMinTime, _bootMaxTime);
            yield return new WaitForSeconds(bootTime);
            _fsm.SendMessage(_fsm.TimerEnded);
        }
    }
}
