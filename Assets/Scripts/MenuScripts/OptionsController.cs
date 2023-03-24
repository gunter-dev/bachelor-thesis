using System.Collections.Generic;
using UnityEngine;

namespace MenuScripts
{
    public class OptionsController : MonoBehaviour
    {
        private Resolution[] _availableResolutions;

        public TMPro.TMP_Dropdown resolutionDropdown;
    
        void Start()
        {
            FillResolutionDropdown();
        }

        private void FillResolutionDropdown()
        {
            _availableResolutions = Screen.resolutions;
            int currentResolutionIndex = 0;

            List<string> resolutionOptions = new List<string>();
            for (int i = 0; i < _availableResolutions.Length; i++)
            {
                Resolution res = _availableResolutions[i];

                string resolution = GetStringFromResolution(res);
                resolutionOptions.Add(resolution);
            
                if (res.width == Screen.width && res.height == Screen.height)
                    currentResolutionIndex = i;
            }

            resolutionDropdown.ClearOptions();
            resolutionDropdown.AddOptions(resolutionOptions);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        private string GetStringFromResolution(Resolution resolution)
        {
            return resolution.width + "x" + resolution.height + "@" + resolution.refreshRate + "hz";
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _availableResolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }
}
