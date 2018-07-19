using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class RotationEventEffectManager : MonoBehaviour
    {
        List<RotationEventEffect> effectDescriptors;
        List<LightRotationEventEffect> lightRotationEffects;

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            UpdateSongController();

            // reset position on new scene
            foreach(LightRotationEventEffect rotEffect in lightRotationEffects)
            {
                rotEffect.transform.localRotation = ReflectionUtil.GetPrivateField<Quaternion>(rotEffect, "_startRotation");
                rotEffect.enabled = false;
            }
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        public void CreateEffects(GameObject go)
        {
            if(lightRotationEffects == null) lightRotationEffects = new List<LightRotationEventEffect>();
            if (effectDescriptors == null) effectDescriptors = new List<RotationEventEffect>();

            RotationEventEffect[] localDescriptors = go.GetComponentsInChildren<RotationEventEffect>(true);

            if (localDescriptors == null) return;

            foreach (RotationEventEffect effectDescriptor in effectDescriptors)
            {
                LightRotationEventEffect rotEvent = effectDescriptor.gameObject.AddComponent<LightRotationEventEffect>();

                ReflectionUtil.SetPrivateField(rotEvent, "_event", (BeatmapEventType)effectDescriptor.eventType);
                ReflectionUtil.SetPrivateField(rotEvent, "_rotationVector", effectDescriptor.rotationVector);
                ReflectionUtil.SetPrivateField(rotEvent, "_transform", rotEvent.transform);
                ReflectionUtil.SetPrivateField(rotEvent, "_startRotation", rotEvent.transform.rotation);
                lightRotationEffects.Add(rotEvent);
                effectDescriptors.Add(effectDescriptor);
            }
        }

        public void UpdateSongController()
        {
            BeatmapObjectCallbackController beatmapObjectCallbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().First();
            
            if (beatmapObjectCallbackController == null) return;

            foreach (LightRotationEventEffect rotationEffect in lightRotationEffects)
            {
                ReflectionUtil.SetPrivateField(rotationEffect, "_beatmapObjectCallbackController", beatmapObjectCallbackController);
                beatmapObjectCallbackController.beatmapEventDidTriggerEvent += rotationEffect.HandleBeatmapObjectCallbackControllerBeatmapEventDidTrigger;
            }
        }
    }
}
