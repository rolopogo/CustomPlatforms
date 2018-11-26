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
        private SongEventHandler _songEventHandler;
        private BeatmapObjectCallbackController _beatmapObjectCallbackController;

        public void Awake()
        {
            _songEventHandler = GetComponent<SongEventHandler>();
        }

        public void HandleSongEvent(BeatmapEventData songEventData)
        {
            if (songEventData.type == (BeatmapEventType)_songEventHandler.eventType)
            {
                if (songEventData.value == _songEventHandler.value || _songEventHandler.anyValue)
                {
                    _songEventHandler.OnTrigger.Invoke();
                }
            }
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            UpdateSongController();
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent -= HandleSongEvent;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent += HandleSongEvent;
        }

        private void OnEnable()
        {
            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            UpdateSongController();
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent -= HandleSongEvent;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent += HandleSongEvent;
        }

        private void OnDisable()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            _beatmapObjectCallbackController.beatmapEventDidTriggerEvent -= HandleSongEvent;
        }

        public void UpdateSongController()
        {
            _beatmapObjectCallbackController = Resources.FindObjectsOfTypeAll<BeatmapObjectCallbackController>().First();
        }
    }
}
