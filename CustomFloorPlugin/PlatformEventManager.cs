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
        private EventManager _EventManager;

        private ScoreController _scoreController;
        private ObstacleSaberSparkleEffectManager _saberCollisionManager;
        private GameEnergyCounter _gameEnergyCounter;
        private SongController _songController;

        private void Awake()
        {
            _EventManager = this.gameObject.GetComponent<EventManager>();
            if (_EventManager == null)
                _EventManager = this.gameObject.AddComponent<EventManager>();
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            Init();
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        private void OnEnable()
        {
            Init();
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            if (_scoreController != null)
            {
                _scoreController.noteWasCutEvent -= SliceCallBack;
                _scoreController.noteWasMissedEvent -= NoteMissCallBack;
                _scoreController.multiplierDidChangeEvent -= MultiplierCallBack;
                _scoreController.comboDidChangeEvent -= ComboChangeEvent;
            }
            if(_saberCollisionManager != null) { 
            _saberCollisionManager.sparkleEffectDidStartEvent -= SaberStartCollide;
            _saberCollisionManager.sparkleEffectDidEndEvent -= SaberEndCollide;
            }
            if (_gameEnergyCounter != null)
            {
                _gameEnergyCounter.gameEnergyDidReach0Event -= FailLevelCallBack;
            }
            if (_songController != null)
            {
                _songController.songEvent -= LightEventCallBack;
            }
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
                _songController = Resources.FindObjectsOfTypeAll<SongController>().FirstOrDefault();
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
                if (_songController != null)
                {
                    _songController.songEvent += LightEventCallBack;
                }
            } catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SliceCallBack(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
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

        private void NoteMissCallBack(NoteData noteData, int multiplier)
        {
            if (noteData.noteType != NoteData.NoteType.Bomb)
            {
                _EventManager.OnComboBreak.Invoke();
            }
        }

        private void MultiplierCallBack(int multiplier, float progress)
        {
            if (multiplier > 1 && progress < 0.1f)
            {
                _EventManager.MultiplierUp.Invoke();
            }
        }

        private void SaberStartCollide(Saber.SaberType saber)
        {
            _EventManager.SaberStartColliding.Invoke();
        }

        private void SaberEndCollide(Saber.SaberType saber)
        {
            _EventManager.SaberStopColliding.Invoke();
        }

        private void FailLevelCallBack()
        {
            _EventManager.OnLevelFail.Invoke();
        }

        private void LightEventCallBack(SongEventData songEvent)
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

        private void ComboChangeEvent(int combo)
        {
            _EventManager.OnComboChanged.Invoke(combo);
        }
    }
}
