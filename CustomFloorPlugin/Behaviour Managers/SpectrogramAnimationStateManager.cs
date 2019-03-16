using CustomFloorPlugin.Util;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomFloorPlugin
{
    public class SpectrogramAnimationStateManager : MonoBehaviour
    {
        List<SpectrogramAnimationState> animationStates;
        
        private void OnEnable()
        {
            BSEvents.gameSceneLoaded += UpdateSpectrogramDataProvider;
            UpdateSpectrogramDataProvider();
        }

        private void OnDisable()
        {
            BSEvents.gameSceneLoaded += UpdateSpectrogramDataProvider;
        }

        public void UpdateAnimationStates()
        {
            animationStates = new List<SpectrogramAnimationState>();

            foreach (SpectrogramAnimationState spec in Resources.FindObjectsOfTypeAll(typeof(SpectrogramAnimationState)))
            {
                animationStates.Add(spec);
            }
        }

        public void UpdateSpectrogramDataProvider()
        {
            BasicSpectrogramData[] datas = Resources.FindObjectsOfTypeAll<BasicSpectrogramData>();
            if (datas.Length == 0) return;
            
            BasicSpectrogramData spectrogramData = datas.First();

            foreach (SpectrogramAnimationState specAnim in animationStates)
            {
                specAnim.setData(spectrogramData);
            }
        }
    }
}
