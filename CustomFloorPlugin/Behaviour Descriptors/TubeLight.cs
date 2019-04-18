using CustomFloorPlugin.Util;
using CustomUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        // ----------------

        private TubeBloomPrePassLight tubeBloomLight;

        private void Awake()
        {
            var prefab = Resources.FindObjectsOfTypeAll<TubeBloomPrePassLight>().First(x => x.name == "Neon");
            
            TubeLight[] localDescriptors = GetComponentsInChildren<TubeLight>(true);

            if (localDescriptors == null) return;

            TubeLight tl = this;
            
            tubeBloomLight = Instantiate(prefab);
            tubeBloomLight.transform.SetParent(tl.transform);
            tubeBloomLight.transform.localRotation = Quaternion.identity;
            tubeBloomLight.transform.localPosition = Vector3.zero;
            tubeBloomLight.transform.localScale = new Vector3(1 / tl.transform.lossyScale.x, 1 / tl.transform.lossyScale.y, 1 / tl.transform.lossyScale.z);

            if (tl.GetComponent<MeshFilter>().mesh.vertexCount == 0)
            {
                tl.GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                // swap for MeshBloomPrePassLight
                tubeBloomLight.gameObject.SetActive(false);
                MeshBloomPrePassLight meshbloom = ReflectionUtil.CopyComponent(tubeBloomLight, typeof(TubeBloomPrePassLight), typeof(MeshBloomPrePassLight), tubeBloomLight.gameObject) as MeshBloomPrePassLight;
                tubeBloomLight.gameObject.SetActive(true);
                meshbloom.Init(tl.GetComponent<Renderer>());
                Destroy(tubeBloomLight);
                tubeBloomLight = meshbloom;
            }

            tubeBloomLight.SetPrivateField("_width", tl.width * 2);
            tubeBloomLight.SetPrivateField("_length", tl.length);
            tubeBloomLight.SetPrivateField("_center", tl.center);
            tubeBloomLight.SetPrivateField("_transform", tubeBloomLight.transform);
            var parabox = tubeBloomLight.GetComponentInChildren<ParametricBoxController>();
            tubeBloomLight.SetPrivateField("_parametricBoxController", parabox);
            var parasprite = tubeBloomLight.GetComponentInChildren<Parametric3SliceSpriteController>();
            tubeBloomLight.SetPrivateField("_dynamic3SliceSprite", parasprite);
            parasprite.Init();
            parasprite.GetComponent<MeshRenderer>().enabled = false;

            tubeBloomLight.color = tl.color * 0.9f;

            var prop = typeof(BSLight).GetField("_ID", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(tubeBloomLight, (int)tl.lightsID);

            //tubeBloomLight.InvokePrivateMethod("OnDisable", new object[0]);
            tubeBloomLight.Refresh();
            TubeLightManager.UpdateEventTubeLightList();
        }


        private void OnEnable()
        {
            BSEvents.menuSceneLoaded += SetColorToDefault;
            BSEvents.menuSceneLoadedFresh += SetColorToDefault;
            SetColorToDefault();
            tubeBloomLight.Refresh();
        }

        private void OnDisable()
        {
            BSEvents.menuSceneLoaded -= SetColorToDefault;
            BSEvents.menuSceneLoadedFresh -= SetColorToDefault;
        }

        private void SetColorToDefault()
        {
            tubeBloomLight.color = color * 0.9f;
            tubeBloomLight.Refresh();
        }
    }
}
