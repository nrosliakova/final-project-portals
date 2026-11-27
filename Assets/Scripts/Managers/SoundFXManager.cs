using UnityEngine;

namespace Managers
{
    public class SoundFXManager : MonoBehaviour
    {
        public static SoundFXManager instance;
        
        [SerializeField] private AudioSource soundFXObject;

        private void Awake()
        {
            if (instance == null) instance = this;
        }

        public void PlaySoundFXClip(AudioClip adioClip, Transform spawnPoint, float volume)
        {
            AudioSource audioSource = Instantiate(soundFXObject, spawnPoint.position, Quaternion.identity);
            
            audioSource.clip = adioClip;
            audioSource.volume = volume;
            
            audioSource.Play();
            
            float clipLength = audioSource.clip.length;
            
            Destroy(audioSource.gameObject, clipLength);
        }
    }
}