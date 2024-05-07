using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit;
using UnityEngine;

namespace Pakpak3D
{
    public class PakpakMovement : MonoBehaviour
    {
        public const string TAG = "Pakpak";
        [SerializeField] private Transform _skin;
        private PakpakControls _controls;
        private Grid2DMovement _movement;
        private GridJump _jump;

        [LnxInit]
        private void Init(PakpakControls controls, Grid2DMovement movement, GridJump jump)
        {
            _controls = controls;
            _movement = movement;
            _jump = jump;

            _controls.OnTurn += TurnTo;
            _controls.OnJump += Jump;
            _movement.OnReachCell += () => _jump.Fall();
            _movement.OnUpdateTarget += UpdateTargetCallback;
            _jump.AfterJumpRise += () => _movement.ResumeMoving();
        }

        private void Start()
        {
            TurnTo(Vector2Int.down);
        }

        public void TurnTo(Vector2 direction)
        {
            _movement.TurnTo(direction);
        }

        public void Jump()
        {
            _jump.Jump();
            _movement.ResumeMoving();
        }

        private void UpdateTargetCallback()
        {
            if (!_movement.IsMoving) return;
            // Debug.Log($"UpdateTargetCallback: {targetPosition2d} {targetDirection} {forwardDirection}");
            _skin.forward = _movement.MovementDirection.Value.X0Y();
        }
    }
}
