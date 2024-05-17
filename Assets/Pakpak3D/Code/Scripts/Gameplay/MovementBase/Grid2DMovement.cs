using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Pakpak3D
{
    public class Grid2DMovement : MonoBehaviour
    {
        private Grid3DMovement _movement3d;
        public bool IsMoving => _movement3d.IsMoving;
        public Vector2Int? MovementDirection => _movement3d.MovementDirection?.XZ();

        public event Action OnReachCell;
        public event Action OnUpdateTarget;

        [LnxInit]
        private void Init(Grid3DMovement movement3d)
        {
            _movement3d = movement3d;
            _movement3d.OnReachCell += () => OnReachCell?.Invoke();
            _movement3d.OnUpdateTarget += () => OnUpdateTarget?.Invoke();
        }

        public void TurnTo(Vector2 direction)
        {
            if (direction == Vector2.zero)
            {
                Debug.LogWarning($"!!! TurnTo: zero direction");
                return;
            }
            _movement3d.TurnTo(direction.X0Y().AsVector3Int());
        }

        public void ResumeMoving()
        {
            _movement3d.ResumeMoving();
        }

        public void PauseMoving()
        {
            _movement3d.PauseMoving();
        }

        public Vector2 GetMovementEndPositionPreview()
        {
            return _movement3d.GetMovementEndPositionPreview().XZ();
        }
    }
}
