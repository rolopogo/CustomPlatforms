using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class PrefabLightmapData : MonoBehaviour
    {

        [SerializeField]
        public Renderer[] m_Renderers;
        [SerializeField]
        public Vector4[] m_LightmapOffsetScales;
        [SerializeField]
        public Texture2D[] m_Lightmaps;


        private void Start()
        {
            //Console.WriteLine("PrefabLightmapData loaded");
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            ApplyLightmaps();
        }

        private void OnDestroy()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }
        
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene scene)
        {
            ApplyLightmaps();
        }

        private void ApplyLightmaps()
        {
            try
            {
                if (m_Renderers == null || m_LightmapOffsetScales == null || m_Lightmaps == null ||
                    m_Renderers.Length <= 0 ||
                    m_Renderers.Length != m_LightmapOffsetScales.Length ||
                    m_Renderers.Length != m_Lightmaps.Length ||
                    m_LightmapOffsetScales.Length != m_Lightmaps.Length)
                    return;
                
                var lightmaps = LightmapSettings.lightmaps;
                var combinedLightmaps = new LightmapData[m_Lightmaps.Length + lightmaps.Length];

                Array.Copy(lightmaps, combinedLightmaps, lightmaps.Length);
                for (int i = 0; i < m_Lightmaps.Length; i++)
                {
                    combinedLightmaps[lightmaps.Length + i] = new LightmapData();
                    combinedLightmaps[lightmaps.Length + i].lightmapColor = m_Lightmaps[i];
                }

                ApplyRendererInfo(m_Renderers, m_LightmapOffsetScales, lightmaps.Length);
                LightmapSettings.lightmaps = combinedLightmaps;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }


        public static void ApplyRendererInfo(Renderer[] renderers, Vector4[] lightmapOffsetScales, int lightmapIndexOffset)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                renderer.lightmapIndex = i + lightmapIndexOffset;
                renderer.lightmapScaleOffset = lightmapOffsetScales[i];
            }
        }


    }
}
