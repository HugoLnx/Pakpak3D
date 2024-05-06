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
        [SerializeField] private Transform _skin;
        private PakpakControls _controls;
        private GridMovement _movement;
        private GridJump _jump;

        [LnxInit]
        private void Init(PakpakControls controls, GridMovement movement, GridJump jump)
        {
            _controls = controls;
            _movement = movement;
            _jump = jump;

            _controls.OnTurn += _movement.TurnTo;
            _controls.OnJump += _jump.Jump;
            _movement.OnSnapInCell += (_) => _jump.Fall();
            _movement.OnUpdateTarget += UpdateTargetCallback;
        }

        private void UpdateTargetCallback(Vector2? targetPosition2d)
        {
            if (!_movement.HasTarget) return;
            Vector2Int? direction = _movement.CellDirection;
            Vector3 forwardDirection = direction.Value.AsVector2Float().X0Y().normalized;
            Debug.Log($"UpdateTargetCallback: {targetPosition2d} {direction} {forwardDirection}");
            _skin.forward = forwardDirection;
        }
    }
}
