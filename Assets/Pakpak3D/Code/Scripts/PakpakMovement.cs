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

        [LnxInit]
        private void Init(PakpakControls controls, GridMovement movement)
        {
            this._controls = controls;
            this._movement = movement;
        }

        private void Start()
        {
            this._controls.OnTurn += this._movement.TurnTo;
        }
    }
}
