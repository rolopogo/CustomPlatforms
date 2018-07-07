using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomFloorPlugin
{
    public class Spectrogram : MonoBehaviour
    {
        public GameObject columnPrefab;
        public Vector3 separator = Vector3.forward;
        public float minHeight = 1f;
        public float maxHeight = 10f;
        public float columnWidth = 1f;
        public float columnDepth = 1f;

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Vector3 zOffset = Vector3.zero;

            for (int i = -64; i < 64; i++)
            {
                zOffset = i * separator;
                if (columnPrefab != null)
                {
                    foreach (Renderer r in columnPrefab.GetComponentsInChildren<Renderer>())
                    {
                        Gizmos.DrawCube(zOffset + r.bounds.center, r.bounds.size);
                    }
                }
                else
                {
                    Gizmos.DrawCube(zOffset, Vector3.one * 0.5f);
                }
            }
        }
    }
}
