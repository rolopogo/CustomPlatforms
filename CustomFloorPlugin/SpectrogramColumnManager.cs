using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class SpectrogramColumnManager : MonoBehaviour
    {
        Spectrogram[] columnDescriptors;
        List<SpectrogramColumns> spectrogramColumns;

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            UpdateSpectrogramDataProvider();
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            UpdateSpectrogramDataProvider();
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        public void CreateColumns()
        {
            spectrogramColumns = new List<SpectrogramColumns>();
            columnDescriptors = gameObject.GetComponentsInChildren<Spectrogram>(true);

            if (columnDescriptors == null) return;

            foreach (Spectrogram spec in columnDescriptors)
            {
                SpectrogramColumns specCol = spec.gameObject.AddComponent<SpectrogramColumns>();
                ReflectionUtil.SetPrivateField(specCol, "_columnPrefab", spec.columnPrefab);
                ReflectionUtil.SetPrivateField(specCol, "_separator", spec.separator);
                ReflectionUtil.SetPrivateField(specCol, "_minHeight", spec.minHeight);
                ReflectionUtil.SetPrivateField(specCol, "_maxHeight", spec.maxHeight);
                ReflectionUtil.SetPrivateField(specCol, "_columnWidth", spec.columnWidth);
                ReflectionUtil.SetPrivateField(specCol, "_columnDepth", spec.columnDepth);
                specCol.CreateColums();
                spectrogramColumns.Add(specCol);
            }
        }
        
        public void UpdateSpectrogramDataProvider()
        {
            BasicSpectrogramData[] datas = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>();
            if (datas.Length == 0) return;
            BasicSpectrogramData spectrogramData = datas.First();
                
            foreach (SpectrogramColumns specCol in spectrogramColumns)
            {
                ReflectionUtil.SetPrivateField(specCol, "_spectrogramData", spectrogramData);
            }
        }
    }
}
