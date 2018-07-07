using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace CustomFloorPlugin
{
    public class SongEventHandler : MonoBehaviour
    {
        public SongEventType eventType;
        public int value; // enum?
        public bool anyValue;
        public UnityEvent OnTrigger;
        
        private void Awake()
        {
            SongEventManager manager = gameObject.AddComponent<SongEventManager>();
            manager.songEventHandler = this;
        }

        
    }
}
