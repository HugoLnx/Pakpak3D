using UnityEngine;
using Pakpak3D.Generated;
using System;

namespace Pakpak3D
{
    public class PakpakControls : MonoBehaviour
    {
        public event Action<Vector2> OnTurn;
        public event Action OnJump;
        private GameInputActions _input;
        private bool _wasEqual;
        private Vector2 _intentDirection;

        private void Awake()
        {
            this._input = new GameInputActions();
            this._input.Gameplay.Enable();
            this._input.Gameplay.Move.performed += ctx => OnMoveInput(ctx.ReadValue<Vector2>());
            this._input.Gameplay.Jump.performed += ctx => OnJump?.Invoke();
        }

        private void OnMoveInput(Vector2 direction)
        {
            if (direction == Vector2.zero) return;

            float xForce = Mathf.Abs(direction.x);
            float yForce = Mathf.Abs(direction.y);
            bool isEqual = xForce == yForce;

            if (isEqual && !_wasEqual)
            {
                _intentDirection = _intentDirection.x == 0 ?
                    new Vector2(Mathf.Sign(direction.x), 0) :
                    new Vector2(0, Mathf.Sign(direction.y));
            }
            else if (xForce > yForce)
            {
                _intentDirection = new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                _intentDirection = new Vector2(0, Mathf.Sign(direction.y));
            }
            OnTurn?.Invoke(_intentDirection);
            _wasEqual = isEqual;
        }
    }
}
