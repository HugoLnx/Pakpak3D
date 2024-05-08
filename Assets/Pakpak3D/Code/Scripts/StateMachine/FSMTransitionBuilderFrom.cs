using System.Collections.Generic;

namespace Sensen.Components
{
    public class FSMTransitionBuilderFrom<TMessage, TState>
    where TMessage : FSMMessage
    where TState : FSMState<TMessage, TState>
    {
        private FSMTransitionBuilder<TMessage, TState> _builder;
        private IEnumerable<TState> _fromStates;

        public FSMTransitionBuilderFrom(FSMTransitionBuilder<TMessage, TState> builder, IEnumerable<TState> states)
        {
            _builder = builder;
            _fromStates = states;
        }

        public FSMTransitionBuilderFrom(FSMTransitionBuilder<TMessage, TState> builder, TState state)
            : this(builder, new List<TState> { state }) { }


        public FSMTransitionBuilderFrom<TMessage, TState> AddTransition(TMessage message, TState state)
        {
            if (message == null) throw new System.Exception($"Message cannot be null");
            if (state == null) throw new System.Exception($"State cannot be null");
            foreach (TState fromState in _fromStates)
            {
                _builder.AddTransition(
                    from: fromState,
                    message: message,
                    to: state
                );
            }
            return this;
        }
    }
}
