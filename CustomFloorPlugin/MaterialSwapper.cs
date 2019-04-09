using System.Linq;
using UnityEngine;

namespace CustomFloorPlugin
{
    static class MaterialSwapper
    {
        public static Material dark { get; private set; }
        public static Material glow { get; private set; }
        public static Material opaqueGlow { get; private set; }

        const string darkReplaceMatName = "_dark_replace (Instance)";
        const string glowReplaceMatName = "_transparent_glow_replace (Instance)";
        const string opaqueGlowReplaceMatName = "_glow_replace (Instance)";

        public static void GetMaterials()
        {
            // This object should be created in the Menu Scene
            // Grab materials from Menu Scene objects
            var materials = Resources.FindObjectsOfTypeAll<Material>();

            dark = new Material(materials.First(x => x.name == "DarkEnvironment1"));
            //opaqueGlow = new Material(materials.First(x => x.name == "EnvLightOpaque"));
            opaqueGlow = new Material(materials.First(x => x.name == "DarkEnvironment1"));
            glow = new Material(materials.First(x => x.name == "EnvLight"));
        }

        public static void ReplaceMaterialsForGameObject(GameObject go)
        {
            if (dark == null || glow == null) GetMaterials();
            ReplaceAllMaterialsForGameObjectChildren(go, dark, darkReplaceMatName);
            ReplaceAllMaterialsForGameObjectChildren(go, glow, glowReplaceMatName);
            ReplaceAllMaterialsForGameObjectChildren(go, opaqueGlow, opaqueGlowReplaceMatName);
        }

        public static void ReplaceAllMaterialsForGameObjectChildren(GameObject go, Material mat, string matToReplaceName = "")
        {
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
            {
                ReplaceAllMaterialsForGameObject(r.gameObject, mat, matToReplaceName);
            }
        }

        public static void ReplaceAllMaterialsForGameObject(GameObject go, Material mat, string matToReplaceName = "")
        {
            Renderer r = go.GetComponent<Renderer>();
            Material[] materialsCopy = r.materials;
            bool materialsDidChange = false;

            for (int i = 0; i < r.materials.Length; i++)
            {
                if (materialsCopy[i].name.Equals(matToReplaceName) || matToReplaceName == "")
                {
                    materialsCopy[i] = mat;
                    materialsDidChange = true;
                }
            }
            if (materialsDidChange)
            {
                r.materials = materialsCopy;
            }
        }
    }
}
