using System;
using System.Collections.Generic;
using LnxArch;

namespace Pakpak3D
{
    public class KillPakpakState : EnemyFSMState
    {
        private GameOverService _gameOverService;
        private EnemyFSMMachine _fsm;

        [LnxInit]
        private void Init(GameOverService gameOverService, EnemyFSMMachine fsm)
        {
            _gameOverService = gameOverService;
            _fsm = fsm;
        }

        protected override void OnEnter(EnemyFSMState previousState)
        {
            base.OnEnter(previousState);
            _gameOverService.EndGame();
            _fsm.SendMessage(_fsm.TimerEnded);
        }
    }
}
