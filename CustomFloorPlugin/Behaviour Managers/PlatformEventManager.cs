using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    class PlatformEventManager : MonoBehaviour
    {
        public EventManager _EventManager;

        private ScoreController _scoreController;
        private ObstacleSaberSparkleEffectManager _saberCollisionManager;
        private GameEnergyCounter _gameEnergyCounter;
        private BeatmapObjectCallbackController _beatmapObjectCallbackController;
        
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        private void OnDisable()
        {
            UnsubscribeFromEvents();
        }
        
        private void Init()
        {
            try
            {
                _EventManager.OnLevelStart.Invoke();

                _scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                _saberCollisionManager =
                    Resources.FindObjectsOfTypeAll<ObstacleSaberSparkleEffectManager>().FirstOrDefault();
                _gameEnergyCounter = Resources.FindObjectsOfTypeAll<GameEnergyCounter>().FirstOrDefault();
                _beatmapObjectCallbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();

                UnsubscribeFromEvents();
                SubscribeToEvents();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SubscribeToEvents()
        {
            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            if (_scoreController != null)
            {
                _scoreController.noteWasCutEvent += SliceCallBack;
                _scoreController.noteWasMissedEvent += NoteMissCallBack;
                _scoreController.multiplierDidChangeEvent += MultiplierCallBack;
                _scoreController.comboDidChangeEvent += ComboChangeEvent;
            }
            if (_saberCollisionManager != null)
            {
                _saberCollisionManager.sparkleEffectDidStartEvent += SaberStartCollide;
                _saberCollisionManager.sparkleEffectDidEndEvent += SaberEndCollide;
            }
            if (_gameEnergyCounter != null)
            {
                _gameEnergyCounter.gameEnergyDidReach0Event += FailLevelCallBack;
            }
            if (_beatmapObjectCallbackController != null)
            {
                _beatmapObjectCallbackController.beatmapEventDidTriggerEvent += LightEventCallBack;
            }
        }

        private void UnsubscribeFromEvents()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            if (_scoreController != null)
            {
                _scoreController.noteWasCutEvent -= SliceCallBack;
                _scoreController.noteWasMissedEvent -= NoteMissCallBack;
                _scoreController.multiplierDidChangeEvent -= MultiplierCallBack;
                _scoreController.comboDidChangeEvent -= ComboChangeEvent;
            }
            if (_saberCollisionManager != null)
            {
                _saberCollisionManager.sparkleEffectDidStartEvent -= SaberStartCollide;
                _saberCollisionManager.sparkleEffectDidEndEvent -= SaberEndCollide;
            }
            if (_gameEnergyCounter != null)
            {
                _gameEnergyCounter.gameEnergyDidReach0Event -= FailLevelCallBack;
            }
            if (_beatmapObjectCallbackController != null)
            {
                _beatmapObjectCallbackController.beatmapEventDidTriggerEvent -= LightEventCallBack;
            }
        }

        private void SliceCallBack(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
        {
            try
            {
                if (!noteCutInfo.allIsOK)
                {
                    _EventManager.OnComboBreak.Invoke();
                }
                else
                {
                    _EventManager.OnSlice.Invoke();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void NoteMissCallBack(NoteData noteData, int multiplier)
        {
            try
            {
                if (noteData.noteType != NoteType.Bomb)
                {
                    _EventManager.OnComboBreak.Invoke();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MultiplierCallBack(int multiplier, float progress)
        {
            try
            {
                if (multiplier > 1 && progress < 0.1f)
                {
                    _EventManager.MultiplierUp.Invoke();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SaberStartCollide(Saber.SaberType saber)
        {
            try
            {
                _EventManager.SaberStartColliding.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SaberEndCollide(Saber.SaberType saber)
        {
            try
            {
                _EventManager.SaberStopColliding.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void FailLevelCallBack()
        {
            try
            {
                _EventManager.OnLevelFail.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void LightEventCallBack(BeatmapEventData songEvent)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ComboChangeEvent(int combo)
        {
            try
            {
                _EventManager.OnComboChanged.Invoke(combo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
