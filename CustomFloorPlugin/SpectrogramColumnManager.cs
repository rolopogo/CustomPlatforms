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
            Console.WriteLine("SpectrogramManager.CreateColumns");
            try
            {
                spectrogramColumns = new List<SpectrogramColumns>();
                columnDescriptors = gameObject.GetComponentsInChildren<Spectrogram>(true);

                if (columnDescriptors == null) return;

                Console.WriteLine("Descriptors found");
                foreach (Spectrogram spec in columnDescriptors)
                {
                    Console.WriteLine("Creating column");
                    SpectrogramColumns specCol = spec.gameObject.AddComponent<SpectrogramColumns>();
                    Console.WriteLine("_columnPrefab");
                    ReflectionUtil.SetPrivateField(specCol, "_columnPrefab", spec.columnPrefab);
                    Console.WriteLine("_separator");
                    ReflectionUtil.SetPrivateField(specCol, "_separator", spec.separator);
                    Console.WriteLine("_minHeight");
                    ReflectionUtil.SetPrivateField(specCol, "_minHeight", spec.minHeight);
                    Console.WriteLine("_maxHeight");
                    ReflectionUtil.SetPrivateField(specCol, "_maxHeight", spec.maxHeight);
                    Console.WriteLine("_columnWidth");
                    ReflectionUtil.SetPrivateField(specCol, "_columnWidth", spec.columnWidth);
                    Console.WriteLine("_columnDepth");
                    ReflectionUtil.SetPrivateField(specCol, "_columnDepth", spec.columnDepth);

                    specCol.CreateColums();

                    spectrogramColumns.Add(specCol);
                    Console.WriteLine("column created");
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        public void UpdateSpectrogramDataProvider()
        {
            Console.WriteLine("SpectrogramManager.UpdateSpectrogram");
            try
            {
                BasicSpectrogramData[] datas = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>();
                if (datas.Length == 0) return;
                BasicSpectrogramData spectrogramData = datas.First();
                
                foreach (SpectrogramColumns specCol in spectrogramColumns)
                {
                    ReflectionUtil.SetPrivateField(specCol, "_spectrogramData", spectrogramData);
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
