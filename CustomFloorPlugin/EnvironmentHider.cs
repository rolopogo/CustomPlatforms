using System;
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
        private ArrayList originalPlatform;
        private ArrayList towers;
        private ArrayList highway;
        private ArrayList visualizer;
        private ArrayList smallRings;
        private ArrayList bigRings;
        
        /// <summary>
        /// Hide and unhide world objects as required by a platform
        /// </summary>
        /// <param name="platform">A platform that defines which objects are to be hidden</param>
        public void HideObjectsForPlatform(CustomPlatform platform)
        {
            SetCollectionActive(originalPlatform, platform.hideDefaultPlatform);
            SetCollectionActive(smallRings, platform.hideSmallRings);
            SetCollectionActive(bigRings, platform.hideBigRings);
            SetCollectionActive(visualizer, platform.hideEQVisualizer);
            SetCollectionActive(towers, platform.hideTowers);
            SetCollectionActive(highway, platform.hideHighway);
        }

        /// <summary>
        /// Finds all GameObjects that make up the default environment
        /// and groups them into array lists
        /// </summary>
        public void FindEnvironment()
        {
            FindOriginalPlatform();
            FindSmallRings();
            FindBigRings();
            FindVisualizers();
            FindTowers();
            FindHighway();
        }

        /// <summary>
        /// Set the active state of a Collection of GameObjects
        /// </summary>
        /// <param name="arlist">An ArrayList of GameObjects</param>
        /// <param name="active">A boolean describing the desired active state</param>
        private void SetCollectionActive(ArrayList arlist, bool active)
        {
            if (arlist == null) return;
            foreach (GameObject go in arlist)
            {
                go.SetActive(active);
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

       

        /// <summary>
        /// Finds all GameObjects that make up the default Platform
        /// and adds them to the originalPlatform collection
        /// </summary>
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
        }

        /// <summary>
        /// Finds all GameObjects that make up the small spinning rings
        /// and adds them to the smallRings collection
        /// </summary>
        private void FindSmallRings()
        {
            FindAddGameObject("SmallTrackLaneRings", smallRings);
        }

        /// <summary>
        /// Finds all GameObjects that make up the large spinning rings
        /// and adds them to the bigRings collection
        /// </summary>
        private void FindBigRings()
        {
            FindAddGameObject("BigTrackLaneRings", bigRings);
        }

        /// <summary>
        /// Finds all GameObjects that make up the EQ visualizers
        /// and adds them to the visualizer collection
        /// </summary>
        private void FindVisualizers()
        {
            FindAddGameObject("SpectrogramLeft", visualizer);
            FindAddGameObject("SpectrogramRight", visualizer);
        }

        /// <summary>
        /// Finds all GameObjects that make up the background towers
        /// and adds them to the tower collection
        /// </summary>
        private void FindTowers()
        {
            FindAddGameObject("NearBuilding", towers);
            FindAddGameObject("NearBuilding (1)", towers);
            FindAddGameObject("NearBuilding (2)", towers);
            FindAddGameObject("NearBuilding (3)", towers);
        }

        /// <summary>
        /// Finds all GameObjects that make up the note highway
        /// and adds them to the highway collection
        /// </summary>
        private void FindHighway()
        {
            FindAddGameObject("Construction", highway);
        }
    }
}
