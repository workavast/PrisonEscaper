using System.Collections;
using audio;
using UnityEngine;
using UnityEngine.Events;

namespace Character
{
    public class TriggerBossMutant : MonoBehaviour
    {
        [SerializeField] private BossMutantArena boss;
        
        [SerializeField] private Collider2D enterDoorCollider;
        [SerializeField] private Collider2D exitDoorCollider;
        [SerializeField] private GameObject enterSprite;
        [SerializeField] private GameObject exitSprite;
       
        private bool _activated;
        private Player _player;
        private CameraFollow _cameraFollow;
        
        private void Start()
        {
            boss.OnBossDie += SetDisable;
            SetDisable();
            _player = Player.Instance;
            _cameraFollow = CameraFollow.Instance;
        }

        IEnumerator WaitBossAnim()
        {
            _activated = true;
            _player.enabled = false;
            _cameraFollow.Target = boss.transform;
            
            enterDoorCollider.enabled = true;
            exitDoorCollider.enabled = true;
            enterSprite.SetActive(true);
            exitSprite.SetActive(true);
            
            MainMusicController.Instance.StopMusic();
            yield return StartCoroutine(boss.StartBossFight());
            MainMusicController.Instance.SetBossMusic();
            
            _cameraFollow.Target = _player.transform;
            _player.enabled = true;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_activated || 
                !other.CompareTag("Player")) 
                return;
            
            StartCoroutine(WaitBossAnim());
        }

        private void SetDisable()
        {
            MainMusicController.Instance.SetDefaultMusic();
            enterDoorCollider.enabled = false;
            exitDoorCollider.enabled = false;
            enterSprite.SetActive(false);
            exitSprite.SetActive(false);
        }
    }
}
