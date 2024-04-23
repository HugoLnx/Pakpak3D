using UnityEngine;
using Pakpak3D.Generated;
using System;

namespace Pakpak3D
{
    public class PakpakControls : MonoBehaviour
    {
        public event Action<Vector2> OnMove;
        public event Action OnJump;
        private GameInputActions _input;

        private void Awake()
        {
            this._input = new GameInputActions();
            this._input.Gameplay.Enable();
            this._input.Gameplay.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
            this._input.Gameplay.Jump.performed += ctx => OnJump?.Invoke();
        }
    }
}
