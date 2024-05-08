using System;
using System.Collections.Generic;
using EasyButtons;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class GhostTargetChase : MonoBehaviour
    {
        [SerializeField] private Transform _mainTarget;
        [SerializeField] private Transform _preferredWaypoint;
        [SerializeField] private float _minDistanceToIgnoreWaypoint = 1f;
        private GhostChase _chase;
        private bool _isChasing;

        [LnxInit]
        private void Init(GhostChase chase)
        {
            _chase = chase;
        }

        [Button]
        public void EnsureChasing()
        {
            _chase.SetChasing(_mainTarget, _preferredWaypoint, _minDistanceToIgnoreWaypoint);
            _chase.ResumeChasing();
        }

        [Button]
        public void StopChasing()
        {
            _chase.PauseChasing();
        }
    }
}
