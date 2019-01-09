using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    class BSSceneManager : MonoBehaviour
    {
        public static BSSceneManager Instance;
        private static Scene _arg0;
        private static Scene _arg1;

        public static Action<Scene, Scene> activeSceneChanged;

        /// <summary>
        /// Create a GameObject with this component attached.
        /// Call this from an IPA Plugin.
        /// </summary>
        public static void OnLoad()
        {
            if (Instance != null) return;
            GameObject go = new GameObject("BSSceneManager");
            go.AddComponent<BSSceneManager>();
        }

        /// <summary>
        /// Called when the GameObject is created.
        /// 
        /// </summary>
        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            DontDestroyOnLoad(gameObject);
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            _arg0 = arg0;
            _arg1 = arg1;

            var sceneManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();

            if (sceneManager == null)
            {
                // The scene was loaded normally
                SceneWasLoaded();
            }
            else
            {
                // AsyncScenesLoader is loading the scene, subscribe to the load complete event
                sceneManager.transitionDidFinishEvent -= SceneWasLoaded; // make sure we don't subscribe twice
                sceneManager.transitionDidFinishEvent += SceneWasLoaded;
            }
        }

        private void SceneWasLoaded()
        {
            activeSceneChanged?.Invoke(_arg0, _arg1);
        }

    }
}