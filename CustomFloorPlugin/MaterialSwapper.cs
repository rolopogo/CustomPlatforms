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

        string darkReplaceMatName = "_dark_replace";
        string glowReplaceMatName = "_glow_replace";

        public void GetMaterials()
        {
            // This object should be created in the Menu Scene
            // Grab materials from Menu Scene objects
            dark = new Material(GameObject.Find("Column").GetComponent<Renderer>().material);
            glow = new Material(GameObject.Find("Neon").GetComponent<Renderer>().material);
        }

        public void ReplaceMaterialsForGameObject(GameObject go)
        {
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                for (int i = 0; i < r.materials.Length; i++)
                {
                    if (r.materials[i].name == darkReplaceMatName)
                    {
                        r.materials[i] = dark;
                    }
                    else if (r.materials[i].name == glowReplaceMatName)
                    {
                        r.materials[i] = glow;
                    }
                }
            }
        }

        public void ReplaceMaterialsForGameObjectWithDark(GameObject go)
        {
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                for (int i = 0; i < r.materials.Length; i++)
                {
                    r.materials[i] = dark;
                }
            }
        }

        public void ReplaceMaterialsForGameObjectWithGlow(GameObject go)
        {
            foreach (Renderer r in go.GetComponentsInChildren<Renderer>())
            {
                for (int i = 0; i < r.materials.Length; i++)
                {
                    r.materials[i] = glow;
                }
            }
        }
    }
}
