using UnityEngine;

namespace audio
{
    [RequireComponent(typeof(AudioSource))]
    public class MainMusicController : MonoBehaviour
    {
        public static MainMusicController Instance { get; private set; }

        [SerializeField] private AudioClip main;
        [SerializeField] private float mainVolume = 0.5f;
        [SerializeField] private AudioClip bossFight;
        [SerializeField] private float bossVolume = 0.25f;

        private AudioSource _audioSource;
        
        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            Instance = this;
                
            _audioSource = GetComponent<AudioSource>();
            SetDefaultMusic();
        }

        public void SetBossMusic()
        {
            _audioSource.clip = bossFight;
            _audioSource.volume = bossVolume;
            _audioSource.Play();
        }

        public void SetDefaultMusic()
        {
            _audioSource.clip = main;
            _audioSource.volume = mainVolume;
            _audioSource.Play();
        }

        public void StopMusic()
        {
            _audioSource.Stop();
        }
    }
}