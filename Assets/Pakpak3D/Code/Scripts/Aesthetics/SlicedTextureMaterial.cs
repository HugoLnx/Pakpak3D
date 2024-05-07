using System.Collections;
using System.Collections.Generic;
using LnxArch;
using UnityEngine;

namespace Pakpak3D
{
    public class SlicedTextureMaterial : MonoBehaviour
    {
        private MeshRenderer _meshRenderer;

        private void Awake()
        {
            _meshRenderer = GetComponentInChildren<MeshRenderer>();
        }

        private void Start()
        {
            Material material = _meshRenderer.material;

            Vector3 scale = this.transform.lossyScale;
            material.SetVector("_Size", new Vector4(
                scale.x,
                scale.y,
                scale.z,
                0
            ));
        }
    }
}
