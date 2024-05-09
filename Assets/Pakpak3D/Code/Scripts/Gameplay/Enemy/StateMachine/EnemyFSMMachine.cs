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
        [field: SerializeField] public EnemyFSMMessage EndScared { get; private set; }
        [field: SerializeField] public EnemyFSMMessage TouchedPlayer { get; private set; }
        [field: SerializeField] public EnemyFSMMessage TimerEnded { get; private set; }
        [field: SerializeField] public EnemyFSMMessage EnteredHouse { get; private set; }
        public EnemyFSMState Boot { get; private set; }
        public EnemyFSMState Hunt { get; private set; }
        public EnemyFSMState Scared { get; private set; }
        public EnemyFSMState Eaten { get; private set; }
        protected override EnemyFSMState InitialState => Boot;

        [LnxInit]
        private void Init(
            BootState bootState,
            HuntState huntState,
            ScaredState scaredState,
            EatenState eatenState
        )
        {
            Boot = bootState;
            Hunt = huntState;
            Scared = scaredState;
            Eaten = eatenState;
        }

        protected override Dictionary<(EnemyFSMState, EnemyFSMMessage), EnemyFSMState> TransitionsAwakeSetup()
        {
            FSMTransitionBuilder<EnemyFSMMessage, EnemyFSMState> builder = new();
            builder.From(Boot)
                .AddTransition(TimerEnded, Hunt);

            builder.From(Hunt, Boot)
                .AddTransition(GetScared, Scared);

            builder.From(Scared)
                .AddTransition(EndScared, Hunt)
                .AddTransition(TouchedPlayer, Eaten);

            builder.From(Eaten)
                .AddTransition(EnteredHouse, Boot);

            return builder.Get();
        }
    }
}
