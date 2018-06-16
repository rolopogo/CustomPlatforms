using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    /// <summary>
    /// Loads AssetBundles containing CustomPlatforms and handles cycling between them
    /// </summary>
    class PlatformLoader : MonoBehaviour
    {
        public static PlatformLoader Instance;

        private EnvironmentHider envHider;
        private const string customFolder = "CustomPlatforms";
        private CustomPlatform[] platforms;
        private int platformIndex = 0;

        /// <summary>
        /// Create a GameObject with this component attached.
        /// Call this from an IPA Plugin.
        /// </summary>
        public static void OnLoad()
        {
            if (Instance != null) return;
            new GameObject("Platform Loader").AddComponent<PlatformLoader>();
        }

        /// <summary>
        /// Called when the GameObject is created.
        /// 
        /// </summary>
        private void Awake()
        {
            if (Instance != null) return;
            Instance = this;

            envHider = new EnvironmentHider();

            CreateAllPlatforms();

            // Retrieve saved index from player prefs if it exists
            // Platform will be loaded 
            if (PlayerPrefs.HasKey("CustomPlatformIndex"))
            {
                int savedIndex = PlayerPrefs.GetInt("CustomPlatformIndex");

                // safety to account for removing assetbundles
                if (savedIndex >= 0 && savedIndex < platforms.Length)
                {
                    platformIndex = savedIndex;
                }
            }

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Handles active scene change. Hides Objects as required for the current platform
        /// </summary>
        /// <param name="arg0">Previous Active Scene</param>
        /// <param name="arg1">New Active Scene</param>
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            // Find environment parts after scene change
            envHider.FindEnvironment();
            // Hide environment
            envHider.HideObjectsForPlatform(platforms[platformIndex]);
        }

        /// <summary>
        /// Loads AssetBundles and populates the platforms array with CustomPlatform objects
        /// </summary>
        private void CreateAllPlatforms()
        {
            // Find AssetBundles in our CustomPlatforms directory
            string[] bundlePaths = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, customFolder), "*.plat");

            // Load and instantiate each AssetBundle
            platforms = new CustomPlatform[bundlePaths.Length + 1];

            // Create a dummy CustomPlatform for the original platform
            platforms[0] = new GameObject("Default Platform").AddComponent<CustomPlatform>();
            // By default no environment items are hidden
            platforms[0].platName = "Default Platform";
            platforms[0].platAuthor = "Beat Saber";

            for (int i = 0; i < bundlePaths.Length; i++)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePaths[i]);
                platforms[i+1] = CreatePlatform(bundle);
            }
        }
        
        /// <summary>
        /// Attempts to find a GameObject by namd and add it to the provided arraylist
        /// </summary>
        /// <param name="name">The name of a GameObject</param>
        /// <param name="alist">The ArrayList to add to</param>
        private void FindAddGameObject(string name, ArrayList alist)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                alist.Add(go);
            }
        }

        /// <summary>
        /// Called every frame.
        /// Handles user input to cycle platforms
        /// </summary>
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                // Hide current Platform
                platforms[platformIndex].gameObject.SetActive(false);
                // Increment index
                platformIndex = (platformIndex + 1) % (platforms.Length + 1);
                // Save index into PlayerPrefs
                PlayerPrefs.SetInt("CustomPlatformIndex", platformIndex);
                // Show new platform
                platforms[platformIndex].gameObject.SetActive(true);
                // Hide environment parts
                envHider.HideObjectsForPlatform(platforms[platformIndex]);
            }
        }
        
        /// <summary>
        /// Instantiate a platform from an assetbundle.
        /// </summary>
        /// <param name="bundle">An AssetBundle containing a CustomPlatform</param>
        /// <returns></returns>
        public CustomPlatform CreatePlatform(AssetBundle bundle)
        {
            GameObject platformPrefab = bundle.LoadAsset<GameObject>("_CustomPlatform");
            if (platformPrefab == null)
            {
                Log("Empty assetbundle");
                return null;
            }
            
            GameObject newPlatform = Instantiate(platformPrefab.gameObject);
            newPlatform.transform.parent = transform;

            bundle.Unload(false);

            // Collect author and name
            CustomPlatform customPlatform = newPlatform.GetComponent<CustomPlatform>();
            if (customPlatform != null)
            {
                newPlatform.name = customPlatform.platName + " by " + customPlatform.platAuthor;
            }

            // display name on screen ?
            Log("Loaded: " + newPlatform.name);

            newPlatform.SetActive(false);
            
            return customPlatform;
            
        }
        
        private static void Log(string s)
        {
            Console.WriteLine("[PlatLoader] " + s);
        }
    }
}