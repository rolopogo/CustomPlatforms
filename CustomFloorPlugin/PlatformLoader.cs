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

        private const string customFolder = "CustomPlatforms";

        private MaterialSwapper matSwapper;
        private EnvironmentHider envHider;

        private string[] bundlePaths;
        private CustomPlatform[] platforms;
        private int platformIndex = 0;

        /// <summary>
        /// Create a GameObject with this component attached.
        /// Call this from an IPA Plugin.
        /// </summary>
        public static void OnLoad()
        {
            if (Instance != null) return;
            GameObject go = new GameObject("Platform Loader");
            go.AddComponent<PlatformLoader>();
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

            envHider = new EnvironmentHider();
            //matSwapper = new MaterialSwapper();
            //matSwapper.GetMaterials();
            
            CreateAllPlatforms();

            // Retrieve saved path from player prefs if it exists
            if (PlayerPrefs.HasKey("CustomPlatformPath"))
            {
                string savedPath = PlayerPrefs.GetString("CustomPlatformPath");
                Log("last platform = " + savedPath);
                // Check if this path was loaded and update our platform index
                for (int i = 0; i < bundlePaths.Length; i++)
                {
                    Log(bundlePaths[i]);
                    if (savedPath == bundlePaths[i])
                    {
                        platformIndex = i + 1;
                        Log("Found match: " + (i + 1));
                    }
                }
            }
            DontDestroyOnLoad(gameObject);
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }
        
        /// <summary>
        /// Handles active scene change. Hides Objects as required for the current platform
        /// </summary>
        /// <param name="arg0">Previous Active Scene</param>
        /// <param name="arg1">New Active Scene</param>
        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            // Show current platform
            platforms[platformIndex].gameObject.SetActive(true);

            // Find environment parts after scene change
            envHider.FindEnvironment();
            
            envHider.HideObjectsForPlatform(platforms[platformIndex]);
        }

        /// <summary>
        /// Loads AssetBundles and populates the platforms array with CustomPlatform objects
        /// </summary>
        private void CreateAllPlatforms()
        {
            // Find AssetBundles in our CustomPlatforms directory
            bundlePaths = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, customFolder), "*.plat");

            // Load and instantiate each AssetBundle
            platforms = new CustomPlatform[bundlePaths.Length + 1];

            // Create a dummy CustomPlatform for the original platform
            platforms[0] = new GameObject("Default Platform").AddComponent<CustomPlatform>();
            platforms[0].transform.parent = transform;
            platforms[0].platName = "Default Platform";
            platforms[0].platAuthor = "Beat Saber";
            
            // Populate the platforms array
            for (int i = 0; i < bundlePaths.Length; i++)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(bundlePaths[i]);

                platforms[i+1] = CreatePlatform(bundle);
                // Rename outdated platform to filename (workaround for losing data?)
                if(platforms[i+1].name == "_CustomPlatform(Clone)")
                {
                    platforms[i + 1].name = Path.GetFileName(bundlePaths[i]);
                }
                Log("Loaded: " + platforms[i + 1].name);
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
                platformIndex = (platformIndex + 1) % platforms.Length;
                // Save index into PlayerPrefs
                string currentPlatformPath = platformIndex == 0 ? "" : bundlePaths[platformIndex-1];
                PlayerPrefs.SetString("CustomPlatformPath", currentPlatformPath);
                Log("Saved platformpath = " + currentPlatformPath);
                // Show new platform
                platforms[platformIndex].gameObject.SetActive(true);

                envHider.HideObjectsForPlatform(platforms[platformIndex]);
                Log("finished swapping platform");
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
                Log("Assetbundle didnt contain a Custom Platform");
                return null;
            }
            
            GameObject newPlatform = Instantiate(platformPrefab.gameObject);
            newPlatform.transform.parent = transform;

            bundle.Unload(false);

            // Collect author and name
            CustomPlatform customPlatform = newPlatform.GetComponent<CustomPlatform>();
            newPlatform.name = customPlatform.platName + " by " + customPlatform.platAuthor;
            
            if(newPlatform.GetComponent<HideEnvironment>() == null)
            {
                // add a new HideEnvironment that hides only the default platform for legacy platforms
                newPlatform.AddComponent<HideEnvironment>().hideDefaultPlatform = true;
            }

            // TODO display name on screen ?


            // Replace materials for this object
            //matSwapper.ReplaceMaterialsForGameObject(newPlatform);

            newPlatform.SetActive(false);
            
            return customPlatform;
        }
        
        private static void Log(string s)
        {
            Console.WriteLine("[PlatLoader] " + s);
        }
    }
}