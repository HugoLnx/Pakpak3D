using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class PakpakMovement : MonoBehaviour
    {
        private PakpakControls _controls;
        private GridMovement _movement;
        private GridJump _jump;

        [LnxInit]
        private void Init(PakpakControls controls, GridMovement movement, GridJump jump)
        {
            this._controls = controls;
            this._movement = movement;
            this._jump = jump;
        }

        private void Start()
        {
            this._controls.OnTurn += this._movement.TurnTo;
            this._controls.OnJump += this._jump.Jump;
            this._movement.OnSnapInCell += (_) => this._jump.Fall();
        }
    }
}
