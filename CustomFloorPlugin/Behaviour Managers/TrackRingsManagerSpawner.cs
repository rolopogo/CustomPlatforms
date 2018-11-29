using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    class TrackRingsManagerSpawner : MonoBehaviour
    {
        List<TrackRings> trackRingsDescriptors;
        public List<TrackLaneRingsManager> trackLaneRingsManagers;
        List<TrackLaneRingsRotationEffectSpawner> rotationSpawners;
        List<TrackLaneRingsPositionStepEffectSpawner> stepSpawners;

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            FindBeatMapEventController();
        }

        private void OnEnable()
        {
            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            FindBeatMapEventController();
        }

        private void Start()
        {
            // make sure the rings are parented to this transform
            foreach (TrackLaneRingsManager trackLaneRingsManager in trackLaneRingsManagers)
            {
                TrackLaneRing[] rings = ReflectionUtil.GetPrivateField<TrackLaneRing[]>(trackLaneRingsManager, "_rings");
                foreach (TrackLaneRing ring in rings)
                {
                    ring.transform.parent = transform;
                }
            }
        }

        private void OnDisable()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        public void CreateTrackRings(GameObject go)
        {
            if(rotationSpawners == null) rotationSpawners = new List<TrackLaneRingsRotationEffectSpawner>();
            if (stepSpawners == null) stepSpawners = new List<TrackLaneRingsPositionStepEffectSpawner>();
            if (trackLaneRingsManagers == null) trackLaneRingsManagers = new List<TrackLaneRingsManager>();
            if (trackRingsDescriptors == null) trackRingsDescriptors = new List<TrackRings>();
            
            TrackRings[] ringsDescriptors = go.GetComponentsInChildren<TrackRings>();

            foreach (TrackRings trackRingDesc in ringsDescriptors)
            {
                trackRingsDescriptors.Add(trackRingDesc);

                TrackLaneRingsManager ringsManager =
                    trackRingDesc.gameObject.AddComponent<TrackLaneRingsManager>();
                trackLaneRingsManagers.Add(ringsManager);

                TrackLaneRing ring = trackRingDesc.trackLaneRingPrefab.AddComponent<TrackLaneRing>();

                ReflectionUtil.SetPrivateField(ringsManager, "_trackLaneRingPrefab", ring);
                ReflectionUtil.SetPrivateField(ringsManager, "_ringCount", trackRingDesc.ringCount);
                ReflectionUtil.SetPrivateField(ringsManager, "_ringPositionStep", trackRingDesc.ringPositionStep);

                if (trackRingDesc.useRotationEffect)
                {
                    TrackLaneRingsRotationEffect rotationEffect =
                        trackRingDesc.gameObject.AddComponent<TrackLaneRingsRotationEffect>();

                    ReflectionUtil.SetPrivateField(rotationEffect, "_trackLaneRingsManager", ringsManager);
                    ReflectionUtil.SetPrivateField(rotationEffect, "_startupRotationAngle", trackRingDesc.startupRotationAngle);
                    ReflectionUtil.SetPrivateField(rotationEffect, "_startupRotationStep", trackRingDesc.startupRotationStep);
                    ReflectionUtil.SetPrivateField(rotationEffect, "_startupRotationPropagationSpeed", trackRingDesc.startupRotationPropagationSpeed);
                    ReflectionUtil.SetPrivateField(rotationEffect, "_startupRotationFlexySpeed", trackRingDesc.startupRotationFlexySpeed);

                    TrackLaneRingsRotationEffectSpawner rotationEffectSpawner =
                        trackRingDesc.gameObject.AddComponent<TrackLaneRingsRotationEffectSpawner>();
                    rotationSpawners.Add(rotationEffectSpawner);

                    ReflectionUtil.SetPrivateField(rotationEffectSpawner, "_beatmapEventType", (BeatmapEventType)trackRingDesc.rotationSongEventType);
                    ReflectionUtil.SetPrivateField(rotationEffectSpawner, "_rotationStep", trackRingDesc.rotationStep);
                    ReflectionUtil.SetPrivateField(rotationEffectSpawner, "_rotationPropagationSpeed", trackRingDesc.rotationPropagationSpeed);
                    ReflectionUtil.SetPrivateField(rotationEffectSpawner, "_rotationFlexySpeed", trackRingDesc.rotationFlexySpeed);
                    ReflectionUtil.SetPrivateField(rotationEffectSpawner, "_trackLaneRingsRotationEffect", rotationEffect);
                }
                if (trackRingDesc.useStepEffect)
                {
                    TrackLaneRingsPositionStepEffectSpawner stepEffectSpawner =
                        trackRingDesc.gameObject.AddComponent<TrackLaneRingsPositionStepEffectSpawner>();
                    stepSpawners.Add(stepEffectSpawner);

                    ReflectionUtil.SetPrivateField(stepEffectSpawner, "_trackLaneRingsManager", ringsManager);
                    ReflectionUtil.SetPrivateField(stepEffectSpawner, "_beatmapEventType", (BeatmapEventType)trackRingDesc.stepSongEventType);
                    ReflectionUtil.SetPrivateField(stepEffectSpawner, "_minPositionStep", trackRingDesc.minPositionStep);
                    ReflectionUtil.SetPrivateField(stepEffectSpawner, "_maxPositionStep", trackRingDesc.maxPositionStep);
                    ReflectionUtil.SetPrivateField(stepEffectSpawner, "_moveSpeed", trackRingDesc.moveSpeed);
                }
            }
        }

        public void FindBeatMapEventController()
        {
            BeatmapObjectCallbackController _beatmapObjectCallbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
            if (_beatmapObjectCallbackController == null) return;

            foreach (TrackLaneRingsRotationEffectSpawner spawner in rotationSpawners)
            {
                ReflectionUtil.SetPrivateField(spawner, "_beatmapObjectCallbackController", _beatmapObjectCallbackController);
                _beatmapObjectCallbackController.beatmapEventDidTriggerEvent += spawner.HandleBeatmapObjectCallbackControllerBeatmapEventDidTrigger;
            }
            foreach (TrackLaneRingsPositionStepEffectSpawner spawner in stepSpawners)
            {
                ReflectionUtil.SetPrivateField(spawner, "_beatmapObjectCallbackController", _beatmapObjectCallbackController);
                _beatmapObjectCallbackController.beatmapEventDidTriggerEvent += spawner.HandleBeatmapObjectCallbackControllerBeatmapEventDidTrigger;
            }
        }
    }
}
