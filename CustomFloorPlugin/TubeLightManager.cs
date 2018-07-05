using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    class TubeLightManager : MonoBehaviour
    {
        private List<TubeBloomPrePassLight> tbppLights;
        private TubeLight[] tubeLightDescriptors;
        
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            foreach (TubeLight tl in tubeLightDescriptors)
            {
                TubeBloomPrePassLight tubeBloomLight = tl.gameObject.GetComponent<TubeBloomPrePassLight>();
                tubeBloomLight.color = tl.color;
            }
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            foreach (TubeLight tl in tubeLightDescriptors)
            {
                TubeBloomPrePassLight tubeBloomLight = tl.gameObject.GetComponent<TubeBloomPrePassLight>();
                tubeBloomLight.color = tl.color;
            }
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        public void CreateTubeLights()
        {
            tbppLights = new List<TubeBloomPrePassLight>();
            tubeLightDescriptors = gameObject.GetComponentsInChildren<TubeLight>(true);

            if (tubeLightDescriptors == null) return;

            foreach (TubeLight tl in tubeLightDescriptors)
            {
                TubeBloomPrePassLight tubeBloomLight = tl.gameObject.AddComponent<TubeBloomPrePassLight>();
                tubeBloomLight.Init();

                ReflectionUtil.SetPrivateField(tubeBloomLight, "_width", tl.width);
                ReflectionUtil.SetPrivateField(tubeBloomLight, "_length", tl.length);
                ReflectionUtil.SetPrivateField(tubeBloomLight, "_center", tl.center);
                tubeBloomLight.color = tl.color;
                ReflectionUtil.SetPrivateFieldBase(tubeBloomLight, "_ID", (int)tl.lightsID);

                tbppLights.Add(tubeBloomLight);
            }
            
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
            LightSwitchEventEffect[] lightSwitchEvents = Resources.FindObjectsOfTypeAll<LightSwitchEventEffect>();
            foreach (LightSwitchEventEffect switchEffect in lightSwitchEvents)
            {
                ReflectionUtil.SetPrivateField(
                    switchEffect, 
                    "_lights", 
                    BloomPrePass.GetLightsWithID(ReflectionUtil.GetPrivateField<int>(switchEffect, "_lightsID"))
                );
            }
        }
    }
}
