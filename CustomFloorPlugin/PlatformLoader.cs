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

        private List<string> bundlePaths;
        private List<CustomPlatform> platforms;
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
            matSwapper = new MaterialSwapper();
            matSwapper.GetMaterials();

            CreateAllPlatforms();
            
            // Retrieve saved path from player prefs if it exists
            if (PlayerPrefs.HasKey("CustomPlatformPath"))
            {
                string savedPath = PlayerPrefs.GetString("CustomPlatformPath");
                Log("last platform = " + savedPath);
                // Check if this path was loaded and update our platform index
                for (int i = 0; i < bundlePaths.Count; i++)
                {
                    Log(bundlePaths.ElementAt(i));
                    if (savedPath == bundlePaths.ElementAt(i))
                    {
                        platformIndex = i;
                        Log("Found match: " + i);
                    }
                }
            }

            // show current platform
            platforms.ElementAt(platformIndex).gameObject.SetActive(true);

            // find env
            envHider.FindEnvironment();

            // hide env for platform ( in case we missed the first hide on scene change )
            envHider.HideObjectsForPlatform(platforms.ElementAt(platformIndex));

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
            // Ensure we have already created platforms
            if (platforms != null)
            {
                if (platforms.ElementAt(platformIndex) != null)
                {
                    // Find environment parts after scene change
                    envHider.FindEnvironment();

                    Log("hide environment for platform");
                    envHider.HideObjectsForPlatform(platforms.ElementAt(platformIndex));
                }
            }
        }

        /// <summary>
        /// Loads AssetBundles and populates the platforms array with CustomPlatform objects
        /// </summary>
        private void CreateAllPlatforms()
        {
            // Find AssetBundles in our CustomPlatforms directory
            string[] allBundlePaths = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, customFolder), "*.plat");

            platforms = new List<CustomPlatform>();
            bundlePaths = new List<string>();

            // Create a dummy CustomPlatform for the original platform
            CustomPlatform defaultPlatform = new GameObject("Default Platform").AddComponent<CustomPlatform>();
            defaultPlatform.transform.parent = transform;
            defaultPlatform.platName = "Default Platform";
            defaultPlatform.platAuthor = "Beat Saber";
            platforms.Add(defaultPlatform);
            bundlePaths.Add("");

            // Populate the platforms array
            for (int i = 0; i < allBundlePaths.Length; i++)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(allBundlePaths[i]);
                
                CustomPlatform newPlatform = CreatePlatform(bundle);

                if(newPlatform != null)
                {
                    platforms.Add(newPlatform);
                    bundlePaths.Add(allBundlePaths[i]);
                    Log("Loaded: " + newPlatform.name);
                }
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
                platforms.ElementAt(platformIndex).gameObject.SetActive(false);

                // Increment index
                platformIndex = (platformIndex + 1) % platforms.Count;

                // Save path into PlayerPrefs
                PlayerPrefs.SetString("CustomPlatformPath", bundlePaths.ElementAt(platformIndex));

                CustomPlatform newPlaform = platforms.ElementAt(platformIndex);

                // Show new platform
                newPlaform.gameObject.SetActive(true);



                // Hide environment for new platform
                envHider.HideObjectsForPlatform(newPlaform);
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
            if (customPlatform == null)
            {
                // Check for old platform 
                global::CustomPlatform legacyPlatform = newPlatform.GetComponent<global::CustomPlatform>();
                if(legacyPlatform != null)
                {
                    Log("legacy version of customPlatform detected, updating");
                    // Replace legacyplatform component with up to date one
                    customPlatform = newPlatform.AddComponent<CustomPlatform>();
                    customPlatform.platName = legacyPlatform.platName;
                    customPlatform.platAuthor = legacyPlatform.platAuthor;
                    customPlatform.hideDefaultPlatform = true;
                    // Remove old platform data
                    Destroy(legacyPlatform);
                }
                else
                {
                    // no customplatform component, abort
                    Log("loaded object had no customplatform attached, skipping");
                    Destroy(newPlatform);
                    return null;
                }
            }

            newPlatform.name = customPlatform.platName + " by " + customPlatform.platAuthor;
            
            // TODO display name on screen ?
            
            // Replace materials for this object
            matSwapper.ReplaceMaterialsForGameObject(newPlatform);

            newPlatform.SetActive(false);
            
            return customPlatform;
        }
        
        private static void Log(string s)
        {
            Console.WriteLine("[PlatLoader] " + s);
        }
    }
}