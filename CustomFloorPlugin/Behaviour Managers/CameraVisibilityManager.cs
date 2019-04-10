using UnityEngine;

namespace CustomFloorPlugin
{
    public class CameraVisibilityManager
    {
        public const int OnlyInThirdPerson = 3;
        public const int OnlyInHeadset = 4;
        
        public static void SetCameraMasks()
        {
            Camera.main.cullingMask &= ~(1 << OnlyInThirdPerson);
            Camera.main.cullingMask |= 1 << OnlyInHeadset;
        }
    }
}