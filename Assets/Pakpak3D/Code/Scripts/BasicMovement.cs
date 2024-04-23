using System.Collections;
using System.Collections.Generic;
using LnxArch;
using SensenToolkit;
using UnityEngine;

namespace Pakpak3D
{
    public class BasicMovement : MonoBehaviour
    {
        [SerializeField] private float _speed = 5f;
        private Rigidbody _rbody;
        private PakpakControls _controls;
        private Vector2 _direction;

        [LnxInit]
        private void Init(Rigidbody rbody, PakpakControls controls)
        {
            _rbody = rbody;
            _controls = controls;
        }

        private void Start()
        {
            _controls.OnMove += OnMoveInput;
            _controls.OnJump += OnJumpInput;
        }

        private void FixedUpdate()
        {
            _rbody.velocity = _direction.X0Y() * _speed;
        }

        private void OnMoveInput(Vector2 direction)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                _direction = new Vector2(Mathf.Sign(direction.x), 0);
            }
            else
            {
                _direction = new Vector2(0, Mathf.Sign(direction.y));
            }
        }

        private void OnJumpInput()
        {
            Debug.Log("Jump");
        }
    }
}
