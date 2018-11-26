using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomFloorPlugin
{
    public class SpectrogramAnimationStateManager : MonoBehaviour
    {
        List<SpectrogramAnimationState> animationStates;

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            UpdateSpectrogramDataProvider();
        }

        private void OnEnable()
        {
            BSSceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;
            UpdateSpectrogramDataProvider();
        }

        private void OnDisable()
        {
            BSSceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
        }

        public void UpdateAnimationStates()
        {
            if (animationStates == null) animationStates = new List<SpectrogramAnimationState>();

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
