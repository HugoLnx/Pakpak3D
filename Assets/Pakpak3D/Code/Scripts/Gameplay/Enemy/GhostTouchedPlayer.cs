using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit.Sensors;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostTouchedPlayer : MonoBehaviour
    {
        private Ghost _ghost;
        private TriggerSensor _sensor;

        [LnxInit]
        private void Init(Ghost ghost, TriggerSensor sensor)
        {
            _ghost = ghost;
            _sensor = sensor;
            _sensor.OnSensedEnter += OnSensedEnter;
        }

        private void OnSensedEnter(Collider collider)
        {
            if (collider.CompareTag(Pakpak.TAG))
            {
                Pakpak pakpak = collider.GetComponentInChildren<Pakpak>();
                if (_ghost.IsDangerous && !pakpak.IsInCooldown)
                {
                    pakpak.TakeHit();
                }
                _ghost.TouchedPlayer();
            }
        }
    }
}
