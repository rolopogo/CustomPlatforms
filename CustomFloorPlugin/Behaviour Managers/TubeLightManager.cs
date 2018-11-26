using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace CustomFloorPlugin
{
    public class TubeLightManager : MonoBehaviour
    {
        private List<BloomPrePassLight> tbppLights;
        private List<TubeLight> tubeLightDescriptors;
        
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            SetColorToDefault();
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
                }

                MeshBloomPrePassLight mesh = tl.gameObject.GetComponent<MeshBloomPrePassLight>();
                if (mesh != null)
                {
                    mesh.color = tl.color;
                }
            }
        }

        private void OnEnable()
        {
            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            SetColorToDefault();
        }

        private void OnDisable()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }
        

        public void CreateTubeLights(GameObject go)
        {
            if(tbppLights == null) tbppLights = new List<BloomPrePassLight>();
            if (tubeLightDescriptors == null) tubeLightDescriptors = new List<TubeLight>();

            TubeLight[] localDescriptors = go.GetComponentsInChildren<TubeLight>(true);

            if (localDescriptors == null) return;

            foreach (TubeLight tl in localDescriptors)
            {
                BloomPrePassLight tubeBloomLight;

                if (tl.GetComponent<MeshFilter>().mesh.vertexCount == 0)
                {
                    tubeBloomLight = tl.gameObject.AddComponent<TubeBloomPrePassLight>();
                    (tubeBloomLight as TubeBloomPrePassLight).Init();
                } else
                {
                    tubeBloomLight = tl.gameObject.AddComponent<MeshBloomPrePassLight>();
                    (tubeBloomLight as MeshBloomPrePassLight).Init();
                }

                ReflectionUtil.SetPrivateField(tubeBloomLight, "_width", tl.width);
                ReflectionUtil.SetPrivateField(tubeBloomLight, "_length", tl.length);
                ReflectionUtil.SetPrivateField(tubeBloomLight, "_center", tl.center);
                tubeBloomLight.color = tl.color;

                var prop = typeof(BloomPrePassLight).GetField("_ID", BindingFlags.NonPublic | BindingFlags.Instance);
                prop.SetValue(tubeBloomLight, (int)tl.lightsID);

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
