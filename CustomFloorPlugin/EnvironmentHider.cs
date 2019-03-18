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
    public class EnvironmentHider
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
            if (feet != null) SetCollectionHidden(feet, (platform.hideDefaultPlatform && !showFeetOverride));
            if (originalPlatform != null) SetCollectionHidden(originalPlatform, platform.hideDefaultPlatform);
            if (smallRings != null) SetCollectionHidden(smallRings, platform.hideSmallRings);
            if (bigRings != null) SetCollectionHidden(bigRings, platform.hideBigRings);
            if (visualizer != null) SetCollectionHidden(visualizer, platform.hideEQVisualizer);
            if (towers != null) SetCollectionHidden(towers, platform.hideTowers);
            if (highway != null) SetCollectionHidden(highway, platform.hideHighway);
            if (backColumns != null) SetCollectionHidden(backColumns, platform.hideBackColumns);
            if (backLasers != null) SetCollectionHidden(backLasers, platform.hideBackLasers);
            if (doubleColorLasers != null) SetCollectionHidden(doubleColorLasers, platform.hideDoubleColorLasers);
            if (rotatingLasers != null) SetCollectionHidden(rotatingLasers, platform.hideRotatingLasers);
            if (trackLights != null) SetCollectionHidden(trackLights, platform.hideTrackLights);
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
                if(go!=null) go.SetActive(!hidden);
            }
        }

        /// <summary>
        /// Finds a GameObject by name and adds it to the provided ArrayList
        /// </summary>
        /// <param name="name">The name of the desired GameObject</param>
        /// <param name="alist">The ArrayList to be added to</param>
        private bool FindAddGameObject(string name, ArrayList alist)
        {
            GameObject go = GameObject.Find(name);
            if (go != null)
            {
                alist.Add(go);
                return true;
            }
            return false;
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
            FindAddGameObject("Static/PlayersPlace", originalPlatform);//
            FindAddGameObject("MenuPlayersPlace", originalPlatform);
            FindAddGameObject("NeonLight (13)", originalPlatform);
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
            // KDA
            FindAddGameObject("TentacleLeft", smallRings);
            FindAddGameObject("TentacleRight", smallRings);
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
            FindAddGameObject("Spectrograms", visualizer);
        }
        
        private void FindTowers()
        {
            towers = new ArrayList();
            // Song Environments
            FindAddGameObject("Buildings", towers);//

            // Menu
            FindAddGameObject("NearBuildingRight (1)", towers);
            FindAddGameObject("NearBuildingLeft (1)", towers);
            FindAddGameObject("NearBuildingLeft", towers);//
            FindAddGameObject("NearBuildingRight", towers);//

            // Monstercat
            FindAddGameObject("MonstercatLogoL", towers);//
            FindAddGameObject("MonstercatLogoR", towers);//

            // KDA
            FindAddGameObject("FloorL", towers);
            FindAddGameObject("FloorR", towers);
            if (FindAddGameObject($"GlowLine", towers))
            {
                for (int i = 0; i < 100; i++)
                {
                    FindAddGameObject($"GlowLine ({i})", towers);
                }
            }

            FindAddGameObject("NeonLight (19)", towers);
            FindAddGameObject("NeonLight (20)", towers);
        }
        
        private void FindHighway()
        {
            highway = new ArrayList();
            FindAddGameObject("Frame", highway);
            FindAddGameObject("Stripes", highway);
            FindAddGameObject("Environment/Static/Floor", highway);//
            FindAddGameObject("FloorConstruction", highway); //
            FindAddGameObject("FrontColumns", highway);
            FindAddGameObject("Construction", highway);//
            FindAddGameObject("TrackConstruction", highway);//
            FindAddGameObject("TrackMirror", highway);//

            FindAddGameObject($"Cube", highway);
            for (int i = 1; i <= 10; i++)
            {
                FindAddGameObject($"Cube ({i})", highway);//
            }

            //Menu
            FindAddGameObject("LeftSmallBuilding", highway);
            FindAddGameObject("RightSmallBuilding", highway);
            FindAddGameObject("NeonLight (17)", highway);
            FindAddGameObject("NeonLight (18)", highway);
        }
        
        private void FindBackColumns()
        {
            backColumns = new ArrayList();
            FindAddGameObject("BackColumns", backColumns);//
            FindAddGameObject("BackColumns (1)", backColumns);//
            FindAddGameObject("CeilingLamp", backColumns);
        }

        private void FindRotatingLasers()
        {
            rotatingLasers = new ArrayList();
            // Default, BigMirror, Triangle
            FindAddGameObject("RotatingLasersPair (6)", rotatingLasers);
            FindAddGameObject("RotatingLasersPair (5)", rotatingLasers);
            FindAddGameObject("RotatingLasersPair (4)", rotatingLasers);
            FindAddGameObject("RotatingLasersPair (3)", rotatingLasers);
            FindAddGameObject("RotatingLasersPair (2)", rotatingLasers);
            FindAddGameObject("RotatingLasersPair (1)", rotatingLasers);
            FindAddGameObject("RotatingLasersPair", rotatingLasers);

            // Nice Env
            FindAddGameObject("RotatingLasersLeft0", rotatingLasers);
            FindAddGameObject("RotatingLasersLeft1", rotatingLasers);
            FindAddGameObject("RotatingLasersLeft2", rotatingLasers);
            FindAddGameObject("RotatingLasersLeft3", rotatingLasers);
            FindAddGameObject("RotatingLasersRight0", rotatingLasers);
            FindAddGameObject("RotatingLasersRight1", rotatingLasers);
            FindAddGameObject("RotatingLasersRight2", rotatingLasers);
            FindAddGameObject("RotatingLasersRight3", rotatingLasers);
        }

        private void FindDoubleColorLasers()
        {
            doubleColorLasers = new ArrayList();

            // Default, BigMirror, Nice, 
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
            FindAddGameObject("FrontLights", backLasers);
            
        }

        private void FindTrackLights()
        {
            trackLights = new ArrayList();
            FindAddGameObject("GlowLineR", trackLights);
            FindAddGameObject("GlowLineL", trackLights);
            FindAddGameObject("GlowLineR2", trackLights);
            FindAddGameObject("GlowLineL2", trackLights);
            FindAddGameObject("GlowLineFarL", trackLights);
            FindAddGameObject("GlowLineFarR", trackLights);
            
            //KDA
            FindAddGameObject("GlowLineLVisible", trackLights);
            FindAddGameObject("GlowLineRVisible", trackLights);
            
            //KDA, Monstercat
            FindAddGameObject("Laser", trackLights);
            for (int i = 0; i < 15; i++)
            {
                FindAddGameObject($"Laser ({i})", trackLights);
            }
            FindAddGameObject("GlowTopLine", trackLights);
            for (int i = 0; i < 10; i++)
            {
                FindAddGameObject($"GlowTopLine ({i})", trackLights);
            }

            // Monstercat
            FindAddGameObject("GlowLineLHidden", trackLights);
            FindAddGameObject("GlowLineRHidden", trackLights);
        }
    }
}
