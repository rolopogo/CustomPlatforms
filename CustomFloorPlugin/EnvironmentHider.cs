using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    /// <summary> 
    /// Activates and deactivates world geometry in the active scene as required by CustomPlatforms
    /// </summary>
    class EnvironmentHider
    {
        private ArrayList feet;
        private ArrayList originalPlatform;
        private ArrayList smallRings;
        private ArrayList bigRings;
        private ArrayList visualizer;
        private ArrayList towers;
        private ArrayList highway;
        private ArrayList backColumns;
        private ArrayList doubleColorLasers;
        private ArrayList backLasers;
        private ArrayList rotatingLasers;
        private ArrayList trackLights;

        public static bool showFeetOverride = false;
        
        /// <summary>
        /// Hide and unhide world objects as required by a platform
        /// </summary>
        /// <param name="platform">A platform that defines which objects are to be hidden</param>
        public void HideObjectsForPlatform(CustomPlatform platform)
        {
            Console.WriteLine("Hiding env for: " + platform.platName);

            SetCollectionHidden(feet, (platform.hideDefaultPlatform && !showFeetOverride));
            SetCollectionHidden(originalPlatform, platform.hideDefaultPlatform);
            SetCollectionHidden(smallRings, platform.hideSmallRings);
            SetCollectionHidden(bigRings, platform.hideBigRings);
            SetCollectionHidden(visualizer, platform.hideEQVisualizer);
            SetCollectionHidden(towers, platform.hideTowers);
            SetCollectionHidden(highway, platform.hideHighway);
            SetCollectionHidden(backColumns, platform.hideBackColumns);
            SetCollectionHidden(backLasers, platform.hideBackLasers);
            SetCollectionHidden(doubleColorLasers, platform.hideDoubleColorLasers);
            SetCollectionHidden(rotatingLasers, platform.hideRotatingLasers);
            SetCollectionHidden(trackLights, platform.hideTrackLights);
            
            Console.WriteLine("Environment hidden");
        }

        /// <summary>
        /// Finds all GameObjects that make up the default environment
        /// and groups them into array lists
        /// </summary>
        public void FindEnvironment()
        {
            FindFeetIcon();
            FindOriginalPlatform();
            FindSmallRings();
            FindBigRings();
            FindVisualizers();
            FindTowers();
            FindHighway();
            FindBackColumns();
            FindBackLasers();
            FindRotatingLasers();
            FindDoubleColorLasers();
            FindTrackLights();
        }

        /// <summary>
        /// Set the active state of a Collection of GameObjects
        /// </summary>
        /// <param name="arlist">An ArrayList of GameObjects</param>
        /// <param name="hidden">A boolean describing the desired hidden state</param>
        private void SetCollectionHidden(ArrayList arlist, bool hidden)
        {
            if (arlist == null) return;
            foreach (GameObject go in arlist)
            {
                go.SetActive(!hidden);
            }
        }

        /// <summary>
        /// Finds a GameObject by name and adds it to the provided ArrayList
        /// </summary>
        /// <param name="name">The name of the desired GameObject</param>
        /// <param name="alist">The ArrayList to be added to</param>
        private void FindAddGameObject(string name, ArrayList alist)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                alist.Add(go);
            }
        }

        private void FindFeetIcon()
        {
            feet = new ArrayList();
            GameObject feetgo = GameObject.Find("Feet");
            if (feetgo != null)
            {
                feet.Add(feetgo);
                feetgo.transform.parent = null; // remove from original platform 
            }
        }

        private void FindOriginalPlatform()
        {
            originalPlatform = new ArrayList();
            FindAddGameObject("Static/PlayersPlace", originalPlatform);
            FindAddGameObject("MenuPlayersPlace", originalPlatform);
        }

        private void FindSmallRings()
        {
            smallRings = new ArrayList();
            FindAddGameObject("SmallTrackLaneRings", smallRings);
            foreach (TrackLaneRing trackLaneRing in Resources.FindObjectsOfTypeAll<TrackLaneRing>().Where(x => x.name == "TrackLaneRing(Clone)"))
            {
                smallRings.Add(trackLaneRing.gameObject);
            }
            FindAddGameObject("TriangleTrackLaneRings", smallRings); // Triangle Rings from TriangleEnvironment
            foreach (TrackLaneRing trackLaneRing in Resources.FindObjectsOfTypeAll<TrackLaneRing>().Where(x => x.name == "TriangleTrackLaneRing(Clone)"))
            {
                smallRings.Add(trackLaneRing.gameObject);
            }
        }
        
        private void FindBigRings()
        {
            bigRings = new ArrayList();
            FindAddGameObject("BigTrackLaneRings", bigRings);
            foreach (var trackLaneRing in Resources.FindObjectsOfTypeAll<TrackLaneRing>().Where(x => x.name == "TrackLaneRingBig(Clone)"))
            {
                bigRings.Add(trackLaneRing.gameObject);
            }
        }

        private void FindVisualizers()
        {
            visualizer = new ArrayList();
            FindAddGameObject("SpectrogramLeft", visualizer);
            FindAddGameObject("SpectrogramRight", visualizer);
        }
        
        private void FindTowers()
        {
            towers = new ArrayList();
            // Song Environments
            FindAddGameObject("NearBuilding", towers);
            FindAddGameObject("NearBuilding (1)", towers);
            FindAddGameObject("NearBuilding (2)", towers);
            FindAddGameObject("NearBuilding (3)", towers);
            // Menu
            FindAddGameObject("NearBuilding (4)", towers);
            FindAddGameObject("NearBuilding (5)", towers);
            FindAddGameObject("NearBuilding (6)", towers);
            FindAddGameObject("NearBuilding (7)", towers);
        }
        
        private void FindHighway()
        {
            highway = new ArrayList();
            FindAddGameObject("Construction", highway);
            FindAddGameObject("Environment/Static/Floor/Floor", highway);
            FindAddGameObject("FrontColumns", highway);
        }

        private void FindBackColumns()
        {
            backColumns = new ArrayList();
            FindAddGameObject("BackColumns", backColumns);
            FindAddGameObject("BackColumnNeon", backColumns);
            FindAddGameObject("BackColumnNeon (1)", backColumns);
        }

        private void FindRotatingLasers()
        {
            rotatingLasers = new ArrayList();
            FindAddGameObject("RotatingLaserLeft0", rotatingLasers);
            FindAddGameObject("RotatingLaserLeft1", rotatingLasers);
            FindAddGameObject("RotatingLaserLeft2", rotatingLasers);
            FindAddGameObject("RotatingLaserLeft3", rotatingLasers);
            FindAddGameObject("RotatingLaserRight0", rotatingLasers);
            FindAddGameObject("RotatingLaserRight1", rotatingLasers);
            FindAddGameObject("RotatingLaserRight2", rotatingLasers);
            FindAddGameObject("RotatingLaserRight3", rotatingLasers);
        }

        private void FindDoubleColorLasers()
        {
            doubleColorLasers = new ArrayList();
            FindAddGameObject("DoubleColorLaser", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (1)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (2)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (3)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (4)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (5)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (6)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (7)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (8)", doubleColorLasers);
            FindAddGameObject("DoubleColorLaser (9)", doubleColorLasers);
        }

        private void FindBackLasers()
        {
            backLasers = new ArrayList();
            FindAddGameObject("BackLaser", backLasers);
            FindAddGameObject("BackLaser (1)", backLasers);
            FindAddGameObject("BackLaser (2)", backLasers);
            FindAddGameObject("BackLaser (3)", backLasers);
            FindAddGameObject("BackLaser (4)", backLasers);
            FindAddGameObject("BackLaser (5)", backLasers);
        }

        private void FindTrackLights()
        {
            trackLights = new ArrayList();
            FindAddGameObject("GlowLineR", trackLights);
            FindAddGameObject("GlowLineL", trackLights);
            FindAddGameObject("GlowLineL (1)", trackLights);
            FindAddGameObject("GlowLineL (2)", trackLights);
            FindAddGameObject("GlowLineL (3)", trackLights);
            FindAddGameObject("GlowLineL (4)", trackLights);
            FindAddGameObject("GlowLineL (5)", trackLights);
            FindAddGameObject("GlowLineL (6)", trackLights);
            FindAddGameObject("GlowLineL (7)", trackLights);
        }
    }
}
