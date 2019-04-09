using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class SpectrogramAnimationState : MonoBehaviour
    {
        public AnimationClip animationClip;
        [Header("0: Low Frequency, 63 High Frequency")]
        [Range(0,63)]
        public int sample;
        [Header("Use the average of all samples, ignoring specified sample")]
        public bool averageAllSamples;

        private Animation animation;
        private BasicSpectrogramData spectrogramData;

        public void setData(BasicSpectrogramData newData)
        {
            spectrogramData = newData;
        }

        void Update()
        {
            try
            {
                if (animationClip != null)
                {
                    animation = GetComponent<Animation>();
                    if (animation == null)
                    {
                        animation = gameObject.AddComponent<Animation>();
                        animation.AddClip(animationClip, "clip");
                        animation.Play("clip");
                        animation["clip"].speed = 0;
                    }

                    if (spectrogramData != null)
                    {
                        float average = 0.0f;
                        for (int i = 0; i < 64; i++)
                        {
                            average += spectrogramData.ProcessedSamples[i];
                        }
                        average = average / 64.0f;

                        float value = averageAllSamples ? average : spectrogramData.ProcessedSamples[sample];

                        value = value * 5f;
                        if (value > 1f)
                        {
                            value = 1f;
                        }
                        value = Mathf.Pow(value, 2f);

                        animation["clip"].time = value * animation["clip"].length;

                    }
                }
            } catch (Exception e)
            {
                Plugin.logger.Error(e);
            }
        }
    }
}
