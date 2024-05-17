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
        [SerializeField] private Vector2Int _initialDirection = Vector2Int.up;
        [SerializeField] private bool _stopOnButtonRelease = true;
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
            _movement.OnReachCell += OnReachCell;
            _movement.OnUpdateTarget += UpdateTargetCallback;
            _jump.AfterJumpRise += () => _movement.ResumeMoving();
        }

        private void OnReachCell()
        {
            TurnTo(_controls.PressingDirection);
            _jump.Fall();
        }

        private void Start()
        {
            TurnTo(_initialDirection);
        }

        public void TurnTo(Vector2 direction)
        {
            bool isForcingDirectionSomewhere = direction != Vector2.zero;
            if (isForcingDirectionSomewhere)
            {
                _movement.TurnTo(direction);
                _movement.ResumeMoving();
            }
            else if (_stopOnButtonRelease)
            {
                _movement.PauseMoving();
            }
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
