using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class TubeLightManager : MonoBehaviour
    {
        private List<TubeBloomPrePassLight> tbppLights;
        private List<TubeLight> tubeLightDescriptors;
        
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
        

        public void CreateTubeLights(GameObject go)
        {
            if(tbppLights == null) tbppLights = new List<TubeBloomPrePassLight>();
            if (tubeLightDescriptors == null) tubeLightDescriptors = new List<TubeLight>();

            TubeLight[] localDescriptors = go.GetComponentsInChildren<TubeLight>(true);

            if (localDescriptors == null) return;

            foreach (TubeLight tl in localDescriptors)
            {
                TubeBloomPrePassLight tubeBloomLight = tl.gameObject.AddComponent<TubeBloomPrePassLight>();
                tubeBloomLight.Init();

                ReflectionUtil.SetPrivateField(tubeBloomLight, "_width", tl.width);
                ReflectionUtil.SetPrivateField(tubeBloomLight, "_length", tl.length);
                ReflectionUtil.SetPrivateField(tubeBloomLight, "_center", tl.center);
                tubeBloomLight.color = tl.color;
                ReflectionUtil.SetPrivateFieldBase(tubeBloomLight, "_ID", (int)tl.lightsID);

                tbppLights.Add(tubeBloomLight);
                tubeLightDescriptors.Add(tl);
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
