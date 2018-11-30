using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace CustomFloorPlugin
{
    class SongEventManager : MonoBehaviour
    {
        public SongEventHandler _songEventHandler;
        private BeatmapObjectCallbackController _beatmapObjectCallbackController;

        public void Awake()
        {
        }

        public void HandleSongEvent(BeatmapEventData songEventData)
        {
            if (songEventData.type == (BeatmapEventType)_songEventHandler.eventType)
            {
                if (songEventData.value == _songEventHandler.value || _songEventHandler.anyValue)
                {
                    try
                    {
                        _songEventHandler.OnTrigger.Invoke();
                    } catch
                    {
                        Console.WriteLine("Exception while invoking songEvent");
                    }
                }
            }
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            UpdateSongController();
            if (_beatmapObjectCallbackController == null) return;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent -= HandleSongEvent;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent += HandleSongEvent;
        }

        private void OnEnable()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            UpdateSongController();
            if (_beatmapObjectCallbackController == null) return;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent -= HandleSongEvent;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent += HandleSongEvent;
        }

        private void OnDisable()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            if (_beatmapObjectCallbackController == null) return;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent -= HandleSongEvent;
        }

        public void UpdateSongController()
        {
            _beatmapObjectCallbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().FirstOrDefault();
        }
    }
}
