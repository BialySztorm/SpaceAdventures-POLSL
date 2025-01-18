using UnityEngine;

namespace Player
{
    public class AudioManager : MonoBehaviour
    {
        private AudioSource _musicSource;
        private AudioSource _thrustersSfxSource;
        private AudioSource _rotationSfxSource;
        private AudioSource _interactionSfxSource;
        private AudioSource _emptySfxSource;
        
        private void Awake()
        {
            _musicSource = transform.Find("BackgroundMusic").GetComponent<AudioSource>();
            _thrustersSfxSource = transform.Find("ThrustersSfx").GetComponent<AudioSource>();
            _rotationSfxSource = transform.Find("RotationSfx").GetComponent<AudioSource>();
            _interactionSfxSource = transform.Find("InteractionSfx").GetComponent<AudioSource>();
            _emptySfxSource = transform.Find("EmptySfx").GetComponent<AudioSource>();
        }
        
        public void PlayMusic()
        {
            _musicSource.Play();
        }
        
        public void StopMusic()
        {
            _musicSource.Stop();
        }
        
        public void PlayThrustersSfx()
        {
            _thrustersSfxSource.Play();
        }
        
        public void PlayRotationSfx()
        {
            _rotationSfxSource.Play();
        }
        
        public void PlayInteractionSfx()
        {
            _interactionSfxSource.Play();
        }
        
        public void PlayEmptySfx(AudioClip clip, bool loop = false)
        {
            _emptySfxSource.clip = clip;
            _emptySfxSource.loop = loop;
            _emptySfxSource.Play();
        }
        
        public void StopEmptySfx()
        {
            _emptySfxSource.Stop();
        }
    }
}