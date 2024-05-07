using System;
using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class PointCollectable : MonoBehaviour
    {
        private Collectable _collectable;

        [LnxInit]
        private void Init(Collectable collectable)
        {
            _collectable = collectable;
            _collectable.OnCollected += Collect;
        }

        private void Collect()
        {
            Debug.Log("Point collected");
        }
    }
}
