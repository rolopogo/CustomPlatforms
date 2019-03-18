using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;
using CustomFloorPlugin.Util;
using CustomUI.Utilities;

namespace CustomFloorPlugin
{
    public class TubeLightManager : MonoBehaviour
    {
        private List<TubeBloomPrePassLight> tbppLights;
        private List<TubeLight> tubeLightDescriptors;

        static TubeBloomPrePassLight prefab;

        private void OnEnable()
        {
            BSEvents.menuSceneLoaded += SetColorToDefault;
            BSEvents.menuSceneLoadedFresh += SetColorToDefault;
            SetColorToDefault();
            foreach (BloomPrePassLight light in tbppLights)
            {
                (light as TubeBloomPrePassLight).Refresh();
            }
        }

        private void OnDisable()
        {
            BSEvents.menuSceneLoaded -= SetColorToDefault;
            BSEvents.menuSceneLoadedFresh -= SetColorToDefault;
        }

        private void SetColorToDefault()
        {
            tubeLightDescriptors = GameObject.FindObjectsOfType<TubeLight>().ToList();

            foreach (TubeLight tl in tubeLightDescriptors)
            {
                TubeBloomPrePassLight tube = tl.gameObject.GetComponent<TubeBloomPrePassLight>();
                if (tube != null)
                {
                    tube.color = tl.color;
                    tube.Refresh();
                }
            }
        }
        
        public void CreateTubeLights(GameObject go)
        {
            if (prefab == null)
            {
                prefab = Resources.FindObjectsOfTypeAll<TubeBloomPrePassLight>().First(x => x.name == "Neon");
            }
            
            if (tbppLights == null) tbppLights = new List<TubeBloomPrePassLight>();
            if (tubeLightDescriptors == null) tubeLightDescriptors = new List<TubeLight>();
             
            TubeLight[] localDescriptors = go.GetComponentsInChildren<TubeLight>(true);

            if (localDescriptors == null) return;
            
            foreach (TubeLight tl in localDescriptors)
            {
                TubeBloomPrePassLight tubeBloomLight;
                tubeBloomLight = Instantiate(prefab, tl.transform);
                tubeBloomLight.transform.localScale = new Vector3(1 / tl.transform.lossyScale.x, 1 / tl.transform.lossyScale.y, 1 / tl.transform.lossyScale.z);

                if (tl.GetComponent<MeshFilter>().mesh.vertexCount == 0)
                {
                    tl.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    // swap for MeshBloomPrePassLight
                    MeshBloomPrePassLight meshbloom = ReflectionUtil.CopyComponent(tubeBloomLight, typeof(TubeBloomPrePassLight), typeof(MeshBloomPrePassLight), tubeBloomLight.gameObject) as MeshBloomPrePassLight;
                    meshbloom.Init(tl.GetComponent<Renderer>());
                    Destroy(tubeBloomLight);
                    tubeBloomLight = meshbloom;
                }

                tubeBloomLight.SetPrivateField("_width", tl.width * 2);
                tubeBloomLight.SetPrivateField("_length", tl.length);
                tubeBloomLight.SetPrivateField("_center", tl.center);
                tubeBloomLight.SetPrivateField("_transform", tubeBloomLight.transform);
                var parabox = tubeBloomLight.GetComponentInChildren<ParametricBoxController>();
                parabox.GetComponent<MeshRenderer>().enabled = false;
                tubeBloomLight.SetPrivateField("_parametricBoxController", parabox);
                var parasprite = tubeBloomLight.GetComponentInChildren<Parametric3SliceSpriteController>();
                tubeBloomLight.SetPrivateField("_dynamic3SliceSprite", parasprite);
                parasprite.Init();
                parasprite.GetComponent<MeshRenderer>().enabled = false;
                
                tubeBloomLight.color = tl.color;
                tubeBloomLight.transform.localPosition = Vector3.zero;
                
                var prop = typeof(BSLight).GetField("_ID", BindingFlags.NonPublic | BindingFlags.Instance);
                prop.SetValue(tubeBloomLight, (int)tl.lightsID);
                
                tubeBloomLight.InvokePrivateMethod("OnDisable", new object[0]);

                tbppLights.Add(tubeBloomLight);
                tubeLightDescriptors.Add(tl);
            }
        }

        public static void CreateAdditionalLightSwitchControllers()
        {
            LightSwitchEventEffect templateSwitchEffect = Resources.FindObjectsOfTypeAll<LightSwitchEventEffect>().FirstOrDefault();

            for (int i = 6; i < 16; i++)
            {
                LightSwitchEventEffect newSwitchEffect = ReflectionUtil.CopyComponent(templateSwitchEffect, typeof(LightSwitchEventEffect), typeof(LightSwitchEventEffect), templateSwitchEffect.gameObject) as LightSwitchEventEffect;
                newSwitchEffect.SetPrivateField("_lightsID", i);
                newSwitchEffect.SetPrivateField("_event", (BeatmapEventType)(i-1));
            }
            UpdateEventTubeLightList();
        }

        private void DestroyTubeLights()
        {
            try
            {
                for (int i = 0; i < tbppLights.Count; i++)
                {
                    GameObject.Destroy(tbppLights.Last());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void UpdateEventTubeLightList()
        {
            BSLight[] allLights = GameObject.FindObjectsOfType<BSLight>();

            LightSwitchEventEffect[] lightSwitchEvents = Resources.FindObjectsOfTypeAll<LightSwitchEventEffect>();
            foreach (LightSwitchEventEffect switchEffect in lightSwitchEvents)
            {
                ReflectionUtil.SetPrivateField(
                    switchEffect,
                    "_lights",
                    allLights.Where(x => x.ID == switchEffect.LightsID).ToArray()
                );
            }
            
        }
    }
}
