using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomFloorPlugin
{
    public class TrackRings : MonoBehaviour
    {
        [Space]
        [Header("Rings")]
        public GameObject trackLaneRingPrefab;
        public int ringCount = 10;
        public float ringPositionStep = 2f;
        [Space]
        [Header("Rotation Effect")]
        public bool useRotationEffect = false;
        public SongEventType rotationSongEventType = SongEventType.RingsRotationEffect; // replace this with a easier to read enum
        [Space]
        public float rotationStep = 5f;
        public float rotationPropagationSpeed = 1f;
        public float rotationFlexySpeed = 1f;
        [Space]
        public float startupRotationAngle = 0f;
        public float startupRotationStep = 10f;
        public float startupRotationPropagationSpeed = 10f;
        public float startupRotationFlexySpeed = 0.5f;
        [Space]
        [Header("Step Effect")]
        public bool useStepEffect = false;
        public SongEventType stepSongEventType = SongEventType.RingsStepEffect; // replace this with a easier to read enum
        [Space]
        public float minPositionStep = 1f;
        public float maxPositionStep = 2f;
        public float moveSpeed = 1f;
        
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.blue;
            Vector3 zOffset = Vector3.zero;
            for (int i = 0; i < ringCount; i++)
            {
                zOffset = i * ringPositionStep * Vector3.forward;
                if (trackLaneRingPrefab != null)
                {
                    foreach (Renderer r in trackLaneRingPrefab.GetComponentsInChildren<Renderer>())
                    {
                        Gizmos.DrawCube(zOffset + r.bounds.center, r.bounds.size);
                    }
                } else
                {
                    Gizmos.DrawCube(zOffset, Vector3.one * 0.5f);
                }
            }
        }

        
    }

}
