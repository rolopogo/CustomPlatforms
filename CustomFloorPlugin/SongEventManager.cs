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
        private SongController _songController;

        public void Awake()
        {
            _songEventHandler = GetComponent<SongEventHandler>();
        }

        public void HandleSongEvent(SongEventData songEventData)
        {
            if (songEventData.type == (SongEventData.Type)_songEventHandler.eventType)
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
            _songController.songEvent += HandleSongEvent;
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            UpdateSongController();
            _songController.songEvent += HandleSongEvent;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            _songController.songEvent -= HandleSongEvent;
        }

        public void UpdateSongController()
        {
            _songController = Resources.FindObjectsOfTypeAll<SongController>().First();
        }
    }
}
