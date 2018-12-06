using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomFloorPlugin
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TubeLight : MonoBehaviour
    {
        public enum LightsID {
            Static = 0,
            BackLights = 1,
            BigRingLights = 2,
            LeftLasers = 3,
            RightLasers = 4,
            TrackAndBottom = 5,
            Unused5 = 6,
            Unused6 = 7,
            Unused7 = 8,
            RingsRotationEffect = 9,
            RingsStepEffect = 10,
            Unused10 = 11,
            Unused11 = 12,
            RingSpeedLeft = 13,
            RingSpeedRight = 14,
            Unused14 = 15,
            Unused15 = 16
        }

        public float width = 0.5f;
        public float length = 1f;
        [Range(0,1)]
        public float center = 0.5f;
        public Color color = Color.white;
        public LightsID lightsID = LightsID.Static;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.matrix = transform.localToWorldMatrix;
            Vector3 cubeCenter = Vector3.up * (0.5f - center) * length;
            Gizmos.DrawCube(cubeCenter, new Vector3(2 * width, length, 2 * width));
        }
    }
}
