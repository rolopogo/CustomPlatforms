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
        public SongEventHandler songEventHandler;

        private SongController _songController;

        public void HandleSongEvent(SongEventData songEventData)
        {
            if (songEventData.type == (SongEventData.Type)songEventHandler.eventType)
            {
                if (songEventData.value == songEventHandler.value || songEventHandler.anyValue)
                {
                    songEventHandler.OnTrigger.Invoke();
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
