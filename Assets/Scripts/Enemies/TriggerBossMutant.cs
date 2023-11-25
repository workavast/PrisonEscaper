using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Character
{
    public class TriggerBossMutant : MonoBehaviour
    {
        private bool isFirst = true;
        private Player player;
        [SerializeField] private BossMutantArena boss;
        [SerializeField] private CameraFollow cameraFollow;
        private void Start()
        {
            player = Player.Instance;
        }

        IEnumerator WaitBossAnim()
        {
            yield return StartCoroutine(boss.StartBossFight());
            player.enabled = true;
            cameraFollow.Target = player.transform;
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (isFirst && other.CompareTag("Player"))
            {
                isFirst = false;
                player.enabled = false;
                cameraFollow.Target = boss.transform;
                StartCoroutine(WaitBossAnim());


            }
        }
    }
}
