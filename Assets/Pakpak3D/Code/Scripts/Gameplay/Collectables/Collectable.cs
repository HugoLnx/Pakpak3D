using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class Collectable : MonoBehaviour
    {
        public event Action OnCollected;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag(PakpakMovement.TAG)) return;
            Collect();
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.CompareTag(PakpakMovement.TAG)) return;
            Collect();
        }

        public void Collect()
        {
            OnCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}
