using CustomFloorPlugin.Util;
using System.Collections.Generic;
using UnityEngine;
using CustomUI.Utilities;

namespace CustomFloorPlugin
{
    public class RotationEventEffectManager : MonoBehaviour
    {
        List<RotationEventEffect> effectDescriptors;
        List<LightRotationEventEffect> lightRotationEffects;
        
        private void OnEnable()
        {
            foreach (LightRotationEventEffect rotEffect in lightRotationEffects)
            {
                BSEvents.beatmapEvent += rotEffect.HandleBeatmapObjectCallbackControllerBeatmapEventDidTrigger;
            }
            BSEvents.menuSceneLoaded += HandleSceneChange;
            BSEvents.gameSceneLoaded += HandleSceneChange;
            HandleSceneChange();
        }
        
        private void OnDisable()
        {
            foreach (LightRotationEventEffect rotEffect in lightRotationEffects)
            {
                BSEvents.beatmapEvent -= rotEffect.HandleBeatmapObjectCallbackControllerBeatmapEventDidTrigger;
            }
            BSEvents.menuSceneLoaded -= HandleSceneChange;
            BSEvents.gameSceneLoaded -= HandleSceneChange;
        }
        
        private void HandleSceneChange()
        {
            foreach (LightRotationEventEffect rotEffect in lightRotationEffects)
            {
                rotEffect.transform.localRotation = ReflectionUtil.GetPrivateField<Quaternion>(rotEffect, "_startRotation");
                rotEffect.enabled = false;
            }
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
    }
}
