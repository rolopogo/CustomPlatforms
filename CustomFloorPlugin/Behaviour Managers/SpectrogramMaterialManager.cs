using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class SpectrogramMaterialManager : MonoBehaviour
    {
        List<SpectrogramMaterial> spectrogramMaterials;

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            UpdateSpectrogramDataProvider();
        }

        private void OnEnable()
        {
            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            UpdateSpectrogramDataProvider();
        }

        private void OnDisable()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        public void UpdateMaterials()
        {
            if (spectrogramMaterials == null) spectrogramMaterials = new List<SpectrogramMaterial>();

            foreach (SpectrogramMaterial spec in Resources.FindObjectsOfTypeAll(typeof(SpectrogramMaterial)))
            {
                spectrogramMaterials.Add(spec);
            }
        }
        
        public void UpdateSpectrogramDataProvider()
        {

            BasicSpectrogramData[] datas = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>();
            if (datas.Length == 0) return;
            BasicSpectrogramData spectrogramData = datas.First();

            foreach (SpectrogramMaterial specMat in spectrogramMaterials)
            {
                specMat.setData(spectrogramData);
            }
        }
    }
}
