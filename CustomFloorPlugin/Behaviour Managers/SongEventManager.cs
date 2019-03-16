using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using CustomFloorPlugin.Util;

namespace CustomFloorPlugin
{
    class SongEventManager : MonoBehaviour
    {
        public SongEventHandler _songEventHandler;
        
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
        
        private void OnEnable()
        {
            BSEvents.beatmapEvent += HandleSongEvent;
        }

        private void OnDisable()
        {
            BSEvents.beatmapEvent -= HandleSongEvent;
        }
    }
}
