using System;
using System.Collections.Generic;
using UnityEngine;

namespace Sensen.Components
{
    public abstract class FSMMachine<TMessage, TState, TMachine> : MonoBehaviour
    where TMessage : FSMMessage
    where TState : FSMState<TMessage, TState>
    where TMachine : FSMMachine<TMessage, TState, TMachine>
    {
        [SerializeField] protected bool _debug;
        public TState CurrentState { get; private set; } = null;

        protected Dictionary<(TState, TMessage), TState> _transitions;

        protected abstract Dictionary<(TState, TMessage), TState> TransitionsAwakeSetup();
        protected abstract TState InitialState { get; }

        protected void Awake()
        {
            if (InitialState == null) throw new Exception("Initial state is not set");
            _transitions = TransitionsAwakeSetup();
            SwitchStateTo(InitialState);
        }


        public void SendMessage(TMessage message)
        {
            DebugLog($"Message: {message}");
            if (CurrentState == null) throw new Exception("No initial state set.");

            TState nextState = _transitions.GetValueOrDefault((CurrentState, message));
            if (nextState != null)
            {
                SwitchStateTo(nextState);
            }
        }

        private void SwitchStateTo(TState newState)
        {
            DebugLog($"Switching from {CurrentState?.DescriptiveName} to {newState?.DescriptiveName}");
            CurrentState?.Exit(newState);

            TState previousState = CurrentState;
            CurrentState = newState;
            CurrentState.Enter(previousState);
        }

        private void DebugLog(string msg)
        {
            if (!_debug) return;
            Debug.Log($"[FSM:{GetType().Name}] {msg}");
        }
    }
}
