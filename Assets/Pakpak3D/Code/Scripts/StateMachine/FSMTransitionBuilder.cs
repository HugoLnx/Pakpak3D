using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sensen.Components
{
    public class FSMTransitionBuilder<TMessage, TState>
    where TMessage : FSMMessage
    where TState : FSMState<TMessage, TState>
    {
        protected Dictionary<(TState, TMessage), TState> _transitions = new();

        public FSMTransitionBuilderFrom<TMessage, TState> From(IEnumerable<TState> states)
        {
            if (states == null || states.Count() == 0)
            {
                throw new System.Exception($"States cannot be null or empty");
            }
            return new FSMTransitionBuilderFrom<TMessage, TState>(this, states);
        }
        public FSMTransitionBuilderFrom<TMessage, TState> From(params TState[] states)
        {
            return From((IEnumerable<TState>)states);
        }

        public void AddTransition(TState from, TMessage message, TState to)
        {
            if (_transitions.ContainsKey((from, message)))
            {
                Debug.LogWarning($"Transition from {from} with message {message} already exists");
                return;
            }
            _transitions.Add((from, message), to);
        }

        public Dictionary<(TState, TMessage), TState> Get()
        {
            return _transitions;
        }
    }
}
