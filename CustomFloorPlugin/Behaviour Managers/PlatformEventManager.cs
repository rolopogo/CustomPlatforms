using CustomFloorPlugin.Util;
using UnityEngine;

namespace CustomFloorPlugin
{
    class PlatformEventManager : MonoBehaviour
    {
        public EventManager _EventManager;
        
        private void OnEnable()
        {
            SubscribeToEvents();
            _EventManager.OnLevelStart.Invoke();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        
        private void SubscribeToEvents()
        {
            BSEvents.gameSceneLoaded += delegate () { _EventManager.OnLevelStart.Invoke(); };
            BSEvents.noteWasCut += delegate (NoteData data, NoteCutInfo info, int multiplier) { if (info.allIsOK) _EventManager.OnSlice.Invoke(); };
            BSEvents.comboDidBreak += delegate () { _EventManager.OnComboBreak.Invoke(); };
            BSEvents.multiplierDidIncrease += delegate (int multiplier) { _EventManager.MultiplierUp.Invoke(); };
            BSEvents.comboDidChange += delegate (int combo) { _EventManager.OnComboChanged.Invoke(combo); };
            BSEvents.sabersStartCollide += delegate (Saber.SaberType saber) { _EventManager.SaberStartColliding.Invoke(); };
            BSEvents.sabersEndCollide += delegate (Saber.SaberType saber) { _EventManager.SaberStopColliding.Invoke(); };
            BSEvents.levelFailed += delegate (StandardLevelScenesTransitionSetupDataSO transition, LevelCompletionResults results) { _EventManager.OnLevelFail.Invoke(); };
            BSEvents.beatmapEvent += LightEventCallBack;
        }

        private void UnsubscribeFromEvents()
        {
            BSEvents.gameSceneLoaded -= delegate () { _EventManager.OnLevelStart.Invoke(); };
            BSEvents.noteWasCut -= delegate (NoteData data, NoteCutInfo info, int multiplier) { if (info.allIsOK) _EventManager.OnSlice.Invoke(); };
            BSEvents.comboDidBreak -= delegate () { _EventManager.OnComboBreak.Invoke(); };
            BSEvents.multiplierDidIncrease -= delegate (int multiplier) { _EventManager.MultiplierUp.Invoke(); };
            BSEvents.comboDidChange -= delegate (int combo) { _EventManager.OnComboChanged.Invoke(combo); };
            BSEvents.sabersStartCollide -= delegate (Saber.SaberType saber) { _EventManager.SaberStartColliding.Invoke(); };
            BSEvents.sabersEndCollide -= delegate (Saber.SaberType saber) { _EventManager.SaberStopColliding.Invoke(); };
            BSEvents.levelFailed -= delegate (StandardLevelScenesTransitionSetupDataSO transition, LevelCompletionResults results) { _EventManager.OnLevelFail.Invoke(); };
            BSEvents.beatmapEvent -= LightEventCallBack;
        }
        
        private void LightEventCallBack(BeatmapEventData songEvent)
        {
            if ((int)songEvent.type < 5)
            {
                if (songEvent.value > 0 && songEvent.value < 4)
                {
                    _EventManager.OnBlueLightOn.Invoke();
                }

                if (songEvent.value > 4 && songEvent.value < 8)
                {
                    _EventManager.OnRedLightOn.Invoke();
                }
            }
        }
    }
}
