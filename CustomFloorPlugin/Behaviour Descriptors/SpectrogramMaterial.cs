using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomFloorPlugin
{
    public class SpectrogramMaterial : MonoBehaviour
    {
        private Renderer renderer;
        private BasicSpectrogramData spectrogramData;
        [Header("The Array property (uniform float arrayName[64])")]
        public String PropertyName;
        [Header("The global intensity (float valueName)")]
        public String AveragePropertyName;

        public void setData(BasicSpectrogramData newData)
        {
            spectrogramData = newData;
        }

        void Start()
        {
            renderer = gameObject.GetComponent<Renderer>();
        }

        void Update()
        {
            if (spectrogramData != null && renderer != null)
            {
                float average = 0.0f;
                for (int i = 0; i < 64; i++)
                {
                    average += spectrogramData.ProcessedSamples[i];
                }
                average = average / 64.0f;

                foreach (Material mat in renderer.materials)
                {
                    mat.SetFloatArray(PropertyName, spectrogramData.ProcessedSamples);
                    mat.SetFloat(AveragePropertyName, average);
                }
            }
        }
    }
}
