using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomFloorPlugin
{
    class MaterialSwapper
    {
        Material dark;
        Material glow;
        Material mirror;

        string darkReplaceMatName = "_dark_replace (Instance)";
        string glowReplaceMatName = "_glow_replace (Instance)";

        public void GetMaterials()
        {
            // This object should be created in the Menu Scene
            // Grab materials from Menu Scene objects
            dark = new Material(GameObject.Find("Column").GetComponent<Renderer>().material);
            glow = new Material(GameObject.Find("Neon").GetComponent<Renderer>().material);
        }

        public void ReplaceMaterialsForGameObject(GameObject go)
        {
            ReplaceAllMaterialsForGameObjectChildren(go, dark, darkReplaceMatName);
            ReplaceAllMaterialsForGameObjectChildren(go, glow, glowReplaceMatName);
        }

        public void ReplaceAllMaterialsForGameObjectChildren(GameObject go, Material mat, string matToReplaceName = "")
        {
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>(true))
            {
                ReplaceAllMaterialsForGameObject(r.gameObject, mat, matToReplaceName);
            }
        }

        public void ReplaceAllMaterialsForGameObject(GameObject go, Material mat, string matToReplaceName = "")
        {
            Renderer r = go.GetComponent<Renderer>();
            Material[] materialsCopy = r.materials;
            bool materialsDidChange = false;

            for (int i = 0; i < r.materials.Length; i++)
            {
                if (materialsCopy[i].name.Equals(matToReplaceName) || matToReplaceName == "")
                {
                    Color oldCol;
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
