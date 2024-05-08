using System;
using System.Collections;
using EasyButtons;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [LnxAutoAdd]
    public class GhostChaseTrack : MonoBehaviour
    {
        [SerializeField] protected Transform _track;
        [SerializeField] protected float _distanceToReachTarget = 1f;
        private GhostChase _chase;
        private bool _isChasing;

        [LnxInit]
        private void Init(GhostChase chase)
        {
            _chase = chase;
        }

        [Button]
        public void EnsureChasing(Transform newTrack = null)
        {
            if (newTrack != null && newTrack != _track)
            {
                _track = newTrack;
                StopChasing();
            }
            if (_isChasing) return;
            StartCoroutine(ChaseCoroutine());
        }

        [Button]
        public void StopChasing()
        {
            if (!_isChasing) return;
            StopAllCoroutines();
            _isChasing = false;
            _chase.PauseChasing();
        }

        private IEnumerator ChaseCoroutine()
        {
            Debug.Log($"Chasing Track {_track.gameObject.name}");
            _isChasing = true;
            int childCount = _track.childCount;
            var waypoints = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                waypoints[i] = _track.GetChild(i);
            }

            int inx = Mathf.FloorToInt(UnityEngine.Random.value * childCount);
            bool isInTrack = false;
            while (_isChasing)
            {
                _chase.SetChasing(waypoints[inx], ignoreNavmesh: isInTrack);
                _chase.ResumeChasing();
                yield return new WaitWhile(() => _isChasing && _chase.GetDistanceToTarget() > _distanceToReachTarget);
                inx = (inx + 1) % childCount;
                isInTrack = true;
            }

        }
    }
}
