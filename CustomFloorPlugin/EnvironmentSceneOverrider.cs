using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomFloorPlugin
{
    public class EnvironmentSceneOverrider
    {
        public static EnvOverrideMode overrideMode = EnvOverrideMode.Off;
        
        private static SceneInfo defaultSceneInfo;
        private static SceneInfo niceSceneInfo;
        private static SceneInfo bigMirrorSceneInfo;
        private static SceneInfo triangleSceneInfo;

        private const string DEFAULT = "DefaultEnvironment";
        private const string NICE = "NiceEnvironment";
        private const string BIGMIRROR = "BigMirrorEnvironment";
        private const string TRIANGLE = "TriangleEnvironment";
        
        public static void GetSceneInfos()
        {
            var sceneInfos = Resources.FindObjectsOfTypeAll<SceneInfo>();
            if (defaultSceneInfo == null) defaultSceneInfo = sceneInfos.First(x => x.name == DEFAULT + "SceneInfo");
            if (niceSceneInfo == null) niceSceneInfo = sceneInfos.First(x => x.name == NICE + "SceneInfo");
            if (bigMirrorSceneInfo == null) bigMirrorSceneInfo = sceneInfos.First(x => x.name == BIGMIRROR + "SceneInfo");
            if (triangleSceneInfo == null) triangleSceneInfo = sceneInfos.First(x => x.name == TRIANGLE + "SceneInfo");
        }

        public static void OverrideEnvironmentScene()
        {
            // Disabled Scene overriding until SongLoader fixes Scene loading - sorry!
            return;

            if(overrideMode == EnvOverrideMode.Off)
            {
                SetSceneName(defaultSceneInfo, DEFAULT);
                SetSceneName(niceSceneInfo, NICE);
                SetSceneName(bigMirrorSceneInfo, BIGMIRROR);
                SetSceneName(triangleSceneInfo, TRIANGLE);
            }
            else
            {
                string sceneName = "";
                switch (overrideMode)
                {
                    case EnvOverrideMode.Default:
                        sceneName = DEFAULT;
                        break;
                    case EnvOverrideMode.Nice:
                        sceneName = NICE;
                        break;
                    case EnvOverrideMode.BigMirror:
                        sceneName = BIGMIRROR;
                        break;
                    case EnvOverrideMode.Triangle:
                        sceneName = TRIANGLE;
                        break;
                    case EnvOverrideMode.Random:
                        sceneName = new RandomObjectPicker<string>(new string[] { DEFAULT, NICE, BIGMIRROR, TRIANGLE }, 0).PickRandomObject();
                        break;
                }

                SetSceneName(defaultSceneInfo, sceneName);
                SetSceneName(niceSceneInfo, sceneName);
                SetSceneName(bigMirrorSceneInfo, sceneName);
                SetSceneName(triangleSceneInfo, sceneName);

            }

        }

        public static float[] OverrideModes()
        {
            return new float[]
            {
                (float)EnvironmentSceneOverrider.EnvOverrideMode.Off,
                (float)EnvironmentSceneOverrider.EnvOverrideMode.Default,
                (float)EnvironmentSceneOverrider.EnvOverrideMode.Nice,
                (float)EnvironmentSceneOverrider.EnvOverrideMode.BigMirror,
                (float)EnvironmentSceneOverrider.EnvOverrideMode.Triangle,
                (float)EnvironmentSceneOverrider.EnvOverrideMode.Random
            };
        }

        private static void SetSceneName(SceneInfo sceneInfo, string newName)
        {
            sceneInfo.SetPrivateField("_sceneName", newName);
        }
        
        public static string Name(EnvOverrideMode mode)
        {
            switch (mode)
            {
                case EnvOverrideMode.Off:
                    return "Off";
                case EnvOverrideMode.Default:
                    return "Default";
                case EnvOverrideMode.Nice:
                    return "Nice";
                case EnvOverrideMode.BigMirror:
                    return "Big Mirror";
                case EnvOverrideMode.Triangle:
                    return "Triangle";
                case EnvOverrideMode.Random:
                    return "Random";
                default:
                    return "?";
            }
        }
        
        public enum EnvOverrideMode
        {
            Off,
            Default,
            Nice,
            BigMirror,
            Triangle,
            Random
        };

    }
}
