using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class EnvironmentArranger
    {
        public static Arrangement arrangement;

        public static void RearrangeEnvironment()
        {
            switch (arrangement)
            {
                case Arrangement.Default:
                    return;
                case Arrangement.Classic:
                    RearrangeClassic();
                    return;
            }
        }

        private static void RearrangeClassic()
        {
            Console.WriteLine("rearranging");
            SceneDumper.DumpScene2();
            TryMove("RotatingLaserLeft0", new Vector3(-8, 0, 45));
            TryMove("RotatingLaserLeft1", new Vector3(-8, 0, 40));
            TryMove("RotatingLaserLeft2", new Vector3(-8, 0, 35));
            TryMove("RotatingLaserLeft3", new Vector3(-8, 0, 30));

            TryMove("RotatingLaserRight0", new Vector3(8, 0, 45));
            TryMove("RotatingLaserRight1", new Vector3(8, 0, 40));
            TryMove("RotatingLaserRight2", new Vector3(8, 0, 35));
            TryMove("RotatingLaserRight3", new Vector3(8, 0, 30));

            TryHide("Light (1)");
            TryHide("Light (2)");
            TryHide("Light (3)");
            TryHide("Light (6)");
        }

        private static void TryMove(string name, Vector3 pos)
        {
            GameObject toMove = GameObject.Find(name);
            if (toMove != null)
                toMove.transform.position = pos;
        }

        private static void TryHide(string name)
        {
            GameObject toHide = GameObject.Find(name);
            if (toHide != null)
                toHide.SetActive(false);
        }

        public enum Arrangement { Default, Classic };
        
        public static string Name(Arrangement mode)
        {
            switch (mode)
            {
                case Arrangement.Default:
                    return "Default";
                case Arrangement.Classic:
                    return "Classic";
                default:
                    return "?";
            }
        }

        public static float[] RepositionModes()
        {
            return new float[]
            {
                (float)Arrangement.Default,
                (float)Arrangement.Classic
            };
        }
    }
}
