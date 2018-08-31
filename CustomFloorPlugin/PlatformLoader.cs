using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using IllusionPlugin;

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
        public EnvironmentHider envHider;

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
            
            BSSceneManager.activeSceneChanged += BSSceneManagerOnActiveSceneChanged;
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            envHider = new EnvironmentHider();
            matSwapper = new MaterialSwapper();
            matSwapper.GetMaterials();

            CreateAllPlatforms();

            // Retrieve saved path from player prefs if it exists
            if (ModPrefs.HasKey(CustomFloorPlugin.PluginName, "CustomPlatformPath"))
            {
                string savedPath = ModPrefs.GetString(CustomFloorPlugin.PluginName, "CustomPlatformPath");
                // Check if this path was loaded and update our platform index
                for (int i = 0; i < bundlePaths.Count; i++)
                {
                    if (savedPath == bundlePaths.ElementAt(i))
                    {
                        platformIndex = i;
                    }
                }
            }
            PlatformUI.OnLoad();
            HideEnvironment();
        }

        public void OnApplicationQuit()
        {
            BSSceneManager.activeSceneChanged -= BSSceneManagerOnActiveSceneChanged;
        }
        
        /// <summary>
        /// Handles active scene change. Hides Objects as required for the current platform
        /// </summary>
        /// <param name="arg0">Previous Active Scene</param>
        /// <param name="arg1">New Active Scene</param>
        private void BSSceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            if (arg1.name == "Menu")
            {
                PlatformUI.OnLoad();
            }
            
            // The scene was loaded normally, hide environment as usual
            HideEnvironment();
            
        }

        private void HideEnvironment()
        {
            if (platforms.ElementAt(platformIndex) != null)
            {
                // Find environment parts after scene change
                envHider.FindEnvironment();

                HideEnvironmentForCurrentPlatform();
            }
        }

        public void HideEnvironmentForCurrentPlatform()
        {
            envHider.HideObjectsForPlatform(platforms.ElementAt(platformIndex));
        }

        /// <summary>
        /// Loads AssetBundles and populates the platforms array with CustomPlatform objects
        /// </summary>
        private void CreateAllPlatforms()
        {
            
            string customPlatformsFolderPath = Path.Combine(Environment.CurrentDirectory, customFolder);

            // Create the CustomPlatforms folder if it doesn't already exist
            if (!Directory.Exists(customPlatformsFolderPath))
            {
                Directory.CreateDirectory(customPlatformsFolderPath);
            }

            // Find AssetBundles in our CustomPlatforms directory
            string[] allBundlePaths = Directory.GetFiles(customPlatformsFolderPath, "*.plat");

            platforms = new List<CustomPlatform>();
            bundlePaths = new List<string>();

            // Create a dummy CustomPlatform for the original platform
            CustomPlatform defaultPlatform = new GameObject("Default Platform").AddComponent<CustomPlatform>();
            defaultPlatform.transform.parent = transform;
            defaultPlatform.platName = "Default Environment";
            defaultPlatform.platAuthor = "Beat Saber";
            defaultPlatform.icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "InsaneCover").FirstOrDefault();
            platforms.Add(defaultPlatform);
            bundlePaths.Add("");

            // Populate the platforms array
            for (int i = 0; i < allBundlePaths.Length; i++)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(allBundlePaths[i]);

                Log("Loading: " + Path.GetFileName(allBundlePaths[i]));

                CustomPlatform newPlatform = LoadPlatform(bundle);

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
            platforms.ElementAt(platformIndex).gameObject.SetActive(true);
            
            if (Input.GetKeyDown(KeyCode.P))
            {
                NextPlatform();
            }
        }

        public void NextPlatform()
        {
            ChangeToPlatform(platformIndex + 1);
        }

        public void PrevPlatform()
        {
            ChangeToPlatform(platformIndex - 1);
        }

        public int GetPlatformIndex()
        {
            return platformIndex;
        }

        public void ChangeToPlatform(int index)
        {
            // Hide current Platform
            platforms.ElementAt(platformIndex).gameObject.SetActive(false);

            // Increment index
            platformIndex = index % platforms.Count;

            // Save path into ModPrefs
            ModPrefs.SetString(CustomFloorPlugin.PluginName, "CustomPlatformPath", bundlePaths.ElementAt(platformIndex));

            CustomPlatform newPlaform = platforms.ElementAt(platformIndex);

            // Show new platform
            newPlaform.gameObject.SetActive(true);

            // Hide environment for new platform
            envHider.HideObjectsForPlatform(newPlaform);

            // Update lightSwitchEvent TubeLight references
            TubeLightManager.UpdateEventTubeLightList();
        }

        public List<CustomPlatform> GetPlatforms()
        {
            return platforms;
        }

        public CustomPlatform GetPlatform(int i)
        {
            return platforms.ElementAt(i);
        }

        /// <summary>
        /// Instantiate a platform from an assetbundle.
        /// </summary>
        /// <param name="bundle">An AssetBundle containing a CustomPlatform</param>
        /// <returns></returns>
        public CustomPlatform LoadPlatform(AssetBundle bundle)
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
                    Log("Loaded object had no customplatform attached, skipping");
                    Destroy(newPlatform);
                    return null;
                }
            }

            newPlatform.name = customPlatform.platName + " by " + customPlatform.platAuthor;

            if (customPlatform.icon == null) customPlatform.icon = Resources.FindObjectsOfTypeAll<Sprite>().Where(x => x.name == "InsaneCover").FirstOrDefault();

            newPlatform.SetActive(false);

            AddManagers(newPlatform);

            return customPlatform;
        }

        private void AddManagers(GameObject go)
        {
            AddManagers(go, go);
        }

        private void AddManagers(GameObject go, GameObject root)
        {
            // Replace materials for this object
            matSwapper.ReplaceMaterialsForGameObject(go);

            // Add a tube light manager if there are tube light descriptors
            if (go.GetComponentInChildren<TubeLight>(true) != null)
            {
                TubeLightManager tlm = root.GetComponent<TubeLightManager>();
                if(tlm == null) tlm = root.AddComponent<TubeLightManager>();
                tlm.CreateTubeLights(go);
            }

            // Rotation effect manager
            if (go.GetComponentInChildren<RotationEventEffect>(true) != null)
            {
                RotationEventEffectManager rotManager = root.GetComponent<RotationEventEffectManager>();
                if(rotManager == null) rotManager = root.AddComponent<RotationEventEffectManager>();
                rotManager.CreateEffects(go);
            }

            // Add a trackRing controller if there are track ring descriptors
            if (go.GetComponentInChildren<TrackRings>(true) != null)
            {
                foreach (TrackRings trackRings in go.GetComponentsInChildren<TrackRings>(true))
                {
                    GameObject ringPrefab = trackRings.trackLaneRingPrefab;

                    // Add managers to prefabs (nesting)
                    AddManagers(ringPrefab, root);
                }

                TrackRingsManagerSpawner trms = root.GetComponent<TrackRingsManagerSpawner>();
                if(trms == null) trms = root.AddComponent<TrackRingsManagerSpawner>();
                trms.CreateTrackRings(go);
            }

            // Add spectrogram manager
            if (go.GetComponentInChildren<Spectrogram>(true) != null)
            {
                foreach (Spectrogram spec in go.GetComponentsInChildren<Spectrogram>(true))
                {
                    GameObject colPrefab = spec.columnPrefab;

                    AddManagers(colPrefab, root);
                }

                SpectrogramColumnManager specManager = go.GetComponent<SpectrogramColumnManager>();
                if(specManager == null) specManager = go.AddComponent<SpectrogramColumnManager>();
                specManager.CreateColumns(go);
            }

            // Add spectrogram materials manager
            SpectrogramMaterialManager specMatManager = go.GetComponent<SpectrogramMaterialManager>();
            if (specMatManager == null) specMatManager = go.AddComponent<SpectrogramMaterialManager>();
            specMatManager.UpdateMaterials();

            // Add spectrogram animation state manager
            SpectrogramAnimationStateManager specAnimManager = go.GetComponent<SpectrogramAnimationStateManager>();
            if (specAnimManager == null) specAnimManager = go.AddComponent<SpectrogramAnimationStateManager>();
            specAnimManager.UpdateAnimationStates();

            // Add Song event manager
            if (go.GetComponentInChildren<SongEventHandler>(true) != null)
            {
                foreach (SongEventHandler handler in go.GetComponentsInChildren<SongEventHandler>())
                {
                    SongEventManager manager = handler.gameObject.AddComponent<SongEventManager>();
                }
            }

            // Add EventManager 
            if (go.GetComponentInChildren<EventManager>(true) != null)
            {
                foreach (EventManager em in go.GetComponentsInChildren<EventManager>())
                {
                    em.gameObject.AddComponent<PlatformEventManager>();
                }
            }
        }
        
        private static void Log(string s)
        {
            Console.WriteLine("[PlatLoader] " + s);
        }
    }
}