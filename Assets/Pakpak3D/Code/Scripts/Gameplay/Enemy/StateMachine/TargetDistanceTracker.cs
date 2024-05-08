using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxAutoAdd]
    public class TargetDistanceTracker : MonoBehaviour
    {
        private readonly WaitForSeconds _waitForNextTrack = new(0.2f);
        [SerializeField] private Transform _target;
        private Rigidbody _rbody;
        public float DistanceToTarget => _target == null ? -1 : Vector3.Distance(_rbody.position, _target.position);
        private List<(float, Action, Action)> _distanceCallbacks = new();
        private Dictionary<float, bool> _distanceIsInside = new();

        [LnxInit]
        private void Init(Rigidbody rbody)
        {
            _rbody = rbody;
        }

        private void Start()
        {
            StartCoroutine(TrackDistances());
        }

        public void SetDistanceCallbacks(
            float distance,
            Action onEnter = null,
            Action onExit = null
        )
        {
            _distanceCallbacks.Add((distance, onEnter, onExit));
            ReviewDistanceCallback(distance, onEnter, onExit);
        }

        private IEnumerator TrackDistances()
        {
            while (true)
            {
                yield return _waitForNextTrack;
                foreach ((float distance, Action onEnter, Action onExit) in _distanceCallbacks)
                {
                    ReviewDistanceCallback(distance, onEnter, onExit);
                }
            }
        }

        private void ReviewDistanceCallback(float distance, Action onEnter, Action onExit)
        {
            if (DistanceToTarget <= distance)
            {
                if (_distanceIsInside.ContainsKey(distance) && _distanceIsInside[distance]) return;
                _distanceIsInside[distance] = true;
                onEnter?.Invoke();
            }
            else
            {
                if (_distanceIsInside.ContainsKey(distance) && !_distanceIsInside[distance]) return;
                _distanceIsInside[distance] = false;
                onExit?.Invoke();
            }
        }
    }
}
