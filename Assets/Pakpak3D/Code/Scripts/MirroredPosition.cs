using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pakpak3D
{
    public class MirroredPosition : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform _pivot;

        private void Update()
        {
            transform.position = GetMirroredPosition();
        }

        public Vector3 GetMirroredPosition()
        {
            Vector3 toTarget = _target.position - _pivot.position;
            return _pivot.position - toTarget;
        }
    }
}
