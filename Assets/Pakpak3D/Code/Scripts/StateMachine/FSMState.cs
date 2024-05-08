using LnxArch;
using UnityEngine;

namespace Sensen.Components
{
    [LnxAutoAdd]
    public abstract class FSMState<TMessage, TState> : MonoBehaviour
    where TMessage : FSMMessage
    where TState : FSMState<TMessage, TState>
    {
        public bool IsActive { get; private set; } = false;
        public string DescriptiveName => $"{GetType().Name}:{gameObject.name}";
        public void Enter(TState previousState)
        {
            IsActive = true;
            OnEnter(previousState);
        }

        public void Exit(TState nextState)
        {
            IsActive = false;
            OnExit(nextState);
        }
        protected virtual void OnEnter(TState previousState) { }
        protected virtual void OnExit(TState nextState) { }
    }
}
