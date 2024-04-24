using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    [DefaultExecutionOrder(99999)]
    public class MovingPhysics : MonoBehaviour
    {
        private Vector3 _movement;
        private Rigidbody _rbody;
        public Vector3 PositionPreview => _rbody.position + _movement;

        [LnxInit]
        private void Init(Rigidbody rbody)
        {
            this._rbody = rbody;
        }

        public void TranslateBy(Vector3 movement)
        {
            _movement += movement;
        }

        public void TranslateTo(Vector3 position)
        {
            _movement = position - _rbody.position;
        }

        private void FixedUpdate()
        {
            if (_movement != Vector3.zero)
            {
                _rbody.MovePosition(PositionPreview);
                _movement = Vector3.zero;
            }
        }
    }
}
