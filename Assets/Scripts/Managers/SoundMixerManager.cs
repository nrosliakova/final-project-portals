using UnityEngine;
using UnityEngine.Audio;

namespace Managers
{
    public class SoundMixerManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;

        public void SetGeneralVolume(float level)
        {
            mixer.SetFloat("GeneralVolume", Mathf.Log10(level) * 20f);
        }
        
        public void SetMusicVolume(float level)
        {
            mixer.SetFloat("MusicVolume", Mathf.Log10(level) * 20f);
        }
        
        public void SetSFXVolume(float level)
        {
            mixer.SetFloat("SFXVolume", Mathf.Log10(level) * 20f);
        }
        
        public void SetUIVolume(float level)
        {
            mixer.SetFloat("UIVolume", Mathf.Log10(level) * 20f);
        }
    }
}