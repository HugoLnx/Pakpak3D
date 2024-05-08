using System.Collections.Generic;
using LnxArch;
using Sensen.Components;
using UnityEngine;

namespace Pakpak3D
{
    public class EnemyFSMMachine : FSMMachine<EnemyFSMMessage, EnemyFSMState, EnemyFSMMachine>
    {
        [field: SerializeField] public EnemyFSMMessage GoHunt { get; private set; }
        [field: SerializeField] public EnemyFSMMessage GetScared { get; private set; }
        [field: SerializeField] public EnemyFSMMessage TouchedPlayer { get; private set; }
        [field: SerializeField] public EnemyFSMMessage TimerEnded { get; private set; }
        [field: SerializeField] public EnemyFSMMessage EnteredHouse { get; private set; }
        private EnemyFSMState _bootState;
        private EnemyFSMState _huntState;
        private EnemyFSMState _scaredState;
        private EnemyFSMState _eatenState;
        protected override EnemyFSMState InitialState => _bootState;

        [LnxInit]
        private void Init(
            BootState bootState,
            HuntState huntState,
            ScaredState scaredState,
            EatenState eatenState
        )
        {
            _bootState = bootState;
            _huntState = huntState;
            _scaredState = scaredState;
            _eatenState = eatenState;
        }

        protected override Dictionary<(EnemyFSMState, EnemyFSMMessage), EnemyFSMState> TransitionsAwakeSetup()
        {
            FSMTransitionBuilder<EnemyFSMMessage, EnemyFSMState> builder = new();
            builder.From(_bootState)
                .AddTransition(TimerEnded, _huntState);

            builder.From(_huntState, _bootState)
                .AddTransition(GetScared, _scaredState);

            builder.From(_scaredState)
                .AddTransition(TimerEnded, _huntState)
                .AddTransition(TouchedPlayer, _eatenState);

            builder.From(_eatenState)
                .AddTransition(EnteredHouse, _bootState);

            return builder.Get();
        }
    }
}
