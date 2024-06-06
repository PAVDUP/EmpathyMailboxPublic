using UnityEngine;
using UnityEngine.Audio;

namespace Client
{
    public class AudioSettingManager : MonoBehaviour
    {
        public AudioMixer masterMixer;
    
        public void OnBGMVolumeChanged(bool isOn)
        {
            if (isOn)
            {
                masterMixer.SetFloat("BGMVolume", 0);
            }
            else
            {
                masterMixer.SetFloat("BGMVolume", -80);
            }
        }
    
        public void OnSFXVolumeChanged(bool isOn)
        {
            if (isOn)
            {
                masterMixer.SetFloat("SFXVolume", 0);
            }
            else
            {
                masterMixer.SetFloat("SFXVolume", -80);
            }
        }
    }
}
