using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CustomPlatformPlugin
{
    class PlatformLoader : MonoBehaviour
    {
        public static PlatformLoader Instance;
        
        private const string customFolder = "CustomPlatforms";

        GameObject[] platforms;
        ArrayList originalPlatform;
        int platformIndex = 0;
        bool originalPlatformActive = true;

        public static void OnLoad()
        {
            if (Instance != null) return;
            new GameObject("Platform Loader").AddComponent<PlatformLoader>();
        }

        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            // find bundles in our 
            string[] bundlePaths = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, customFolder), "*.plat");

            platforms = new GameObject[bundlePaths.Length];

            for (int i = 0; i < bundlePaths.Length; i++)
            {
                // load bundle
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePaths[i]);
                platforms[i] = CreatePlatform(bundle);
                bundle.Unload(false);
            }

            // Get index from player prefs
            if (PlayerPrefs.HasKey("CustomPlatformIndex"))
            {
                
                int savedIndex = PlayerPrefs.GetInt("CustomPlatformIndex");

                // safety to account for removing assetbundles
                if (savedIndex >= 0 && savedIndex < bundlePaths.Length)
                {
                    platformIndex = savedIndex;
                }
            }

            DontDestroyOnLoad(gameObject);
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            FindOriginalPlatform();
            originalPlatformActive = true;

            PlayerPrefs.SetInt("CustomPlatformIndex", platformIndex);

            // update to the right platform
            SetOriginalPlatformActive(false);

            // show new platform
            SetPlatformActive(platformIndex, true);
        }

        private void FindOriginalPlatform()
        {
            // find parts of original platform
            originalPlatform = new ArrayList();
            FindAddGameObject("Column", originalPlatform);
            FindAddGameObject("Feet", originalPlatform);
            FindAddGameObject("GlowLine", originalPlatform);
            FindAddGameObject("GlowLine (1)", originalPlatform);
            FindAddGameObject("GlowLine (2)", originalPlatform);
            FindAddGameObject("GlowLine (3)", originalPlatform);
            FindAddGameObject("BorderLine (13)", originalPlatform);
            FindAddGameObject("BorderLine (14)", originalPlatform);
            FindAddGameObject("BorderLine (15)", originalPlatform);
            FindAddGameObject("MirrorSurface", originalPlatform);
            FindAddGameObject("PlayersPlace/PlayersPlace", originalPlatform);
            Log("done");
        }

        private void FindAddGameObject(string name, ArrayList alist)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                alist.Add(go);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                // hide current platform
                SetPlatformActive(platformIndex, false);
                // increment index
                platformIndex = (platformIndex + 1) % (platforms.Length + 1);
                // save index into prefs
                PlayerPrefs.SetInt("CustomPlatformIndex", platformIndex);
                // show new platform
                SetPlatformActive(platformIndex, true);
            }
        }
        
        public GameObject CreatePlatform(AssetBundle bundle)
        {
            
            GameObject platformPrefab = bundle.LoadAsset<GameObject>("_CustomPlatform");
            if (platformPrefab == null)
            {
                Log("Empty assetbundle");
                return null;
            }
            
            GameObject newPlatform = Instantiate(platformPrefab.gameObject);
            newPlatform.transform.parent = transform;

            CustomPlatform customPlatform = newPlatform.GetComponent<CustomPlatform>();
            if (customPlatform != null)
            {
                newPlatform.name = customPlatform.platName + " by " + customPlatform.platAuthor;
            }

            // display name on screen ?
            Log("Loaded: " + newPlatform.name);

            newPlatform.SetActive(false);
            
            return newPlatform;
            
        }

        private void SetPlatformActive(int index, bool active)
        {
            if (index > platforms.Length || index < 0)
            {
                Log("Bad index passed to CreatePlatform: " + index);
                return;
            }

            if (index == 0)
            {
                SetOriginalPlatformActive(active);
            }
            else
            {
                if (platforms[index - 1] == null) SetOriginalPlatformActive(active);
                platforms[index - 1].SetActive(active);
            }
        }

        private void SetOriginalPlatformActive(bool active)
        {
            if (active == originalPlatformActive) return; // no change

            foreach (GameObject go in originalPlatform)
            {
                go.SetActive(active);
            }

            originalPlatformActive = active;
        }
        
        private static void Log(string s)
        {
            Console.WriteLine("[PlatLoader] " + s);
        }
    }
}