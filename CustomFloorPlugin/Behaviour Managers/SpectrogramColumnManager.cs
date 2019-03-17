using CustomFloorPlugin.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CustomUI.Utilities;

namespace CustomFloorPlugin
{
    public class SpectrogramColumnManager : MonoBehaviour
    {
        List<Spectrogram> columnDescriptors;
        List<SpectrogramColumns> spectrogramColumns;
        
        private void OnEnable()
        {
            BSEvents.gameSceneLoaded += UpdateSpectrogramDataProvider;
            UpdateSpectrogramDataProvider();
        }

        private void OnDisable()
        {
            BSEvents.gameSceneLoaded -= UpdateSpectrogramDataProvider;
        }

        public void CreateColumns(GameObject go)
        {
            if (spectrogramColumns == null) spectrogramColumns = new List<SpectrogramColumns>();
            if (columnDescriptors == null) columnDescriptors = new List<Spectrogram>();

            Spectrogram[] localDescriptors = go.GetComponentsInChildren<Spectrogram>(true);
            
            foreach (Spectrogram spec in localDescriptors)
            {

                SpectrogramColumns specCol = spec.gameObject.AddComponent<SpectrogramColumns>();
                ReflectionUtil.SetPrivateField(specCol, "_columnPrefab", spec.columnPrefab);
                ReflectionUtil.SetPrivateField(specCol, "_separator", spec.separator);
                ReflectionUtil.SetPrivateField(specCol, "_minHeight", spec.minHeight);
                ReflectionUtil.SetPrivateField(specCol, "_maxHeight", spec.maxHeight);
                ReflectionUtil.SetPrivateField(specCol, "_columnWidth", spec.columnWidth);
                ReflectionUtil.SetPrivateField(specCol, "_columnDepth", spec.columnDepth);
                
                spectrogramColumns.Add(specCol);
                columnDescriptors.Add(spec);
            }
        }
        
        public void UpdateSpectrogramDataProvider()
        {
            BasicSpectrogramData[] datas = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>();
            if (datas.Length == 0) return;
            BasicSpectrogramData spectrogramData = datas.FirstOrDefault();

            if (spectrogramData == null) return;
            foreach (SpectrogramColumns specCol in spectrogramColumns)
            {
                ReflectionUtil.SetPrivateField(specCol, "_spectrogramData", spectrogramData);
            }
        }
    }
}
