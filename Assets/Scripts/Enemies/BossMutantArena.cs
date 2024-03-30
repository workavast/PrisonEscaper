using System;
using System.Collections;
using Character;
using Projectiles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemies
{
    public class BossMutantArena : Enemy
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private GameObject roarAnim;
        [SerializeField] private Transform[] arenaPlatforms;
        [SerializeField] private AudioClip roarSound, jumpSound, hitSound, bossFightSound;

        private bool isFightStart = false;
        private int lastPlatform = -1;
        private float lastAttackTime = 0f;
        private float jumpWaiting = 0f;
        private float platformOffsetX = 0f, platformOffsetY = 3f;
        private float jumpDelay = 5f;
        private float defaultSoundVolume;

        public event Action OnStartBossBattle;
        public event Action OnBossDie;

        private void FixedUpdate()
        {
            HandleFixedUpdate(Time.fixedDeltaTime);
        }

        private void PlaySound(AudioClip curSound)
        {
            //  if (_source.isPlaying) return;

            _source.clip = curSound;
            _source.loop = false;
            _source.Play();
        }
        
        private IEnumerator RoarAnimation()
        {
            PlaySound(roarSound);
            roarAnim.SetActive(true);
            yield return new WaitForSeconds(1f);
            roarAnim.SetActive(false);
        }

        IEnumerator ShakeCamera(float duration, float intensity)
        {
            Camera camera = Camera.main;
            Vector3 originalPosition = camera.transform.position;
            float startTime = Time.time;

            while (Time.time - startTime < duration)
            {
                Vector3 offset = Random.insideUnitSphere * intensity;
                camera.transform.position = originalPosition + offset;
                yield return null;
            }

            camera.transform.position = originalPosition;
        }

        public IEnumerator StartBossFight()
        {
            target = Player.Instance.transform;
            yield return new WaitForSeconds(1f);

            defaultSoundVolume = _source.volume;

            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            roarAnim.transform.localRotation = Quaternion.Euler(0f, 0f, -20f); ;
            StartCoroutine(RoarAnimation());
            StartCoroutine(ShakeCamera(0.5f, 0.2f));
            yield return StartCoroutine(RoarAnimation());

            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            roarAnim.transform.localPosition = new Vector3(roarAnim.transform.localPosition.x, 1.4f, roarAnim.transform.localPosition.z);
            roarAnim.transform.localRotation = Quaternion.Euler(0f, 0f, 20f); ;

            StartCoroutine(ShakeCamera(0.5f, 0.2f));
            yield return StartCoroutine(RoarAnimation());
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            roarAnim.transform.localRotation = Quaternion.Euler(0f, 0f, 0f); ;
            roarAnim.transform.localPosition = new Vector3(5.52f, 0.5f, roarAnim.transform.localPosition.z);
            roarAnim.transform.localScale = new Vector3(4f, 4f, 1f);


            _source.volume *= 2f;
            StartCoroutine(ShakeCamera(1f, 0.4f));
            yield return StartCoroutine(RoarAnimation());
            yield return new WaitForSeconds(.5f);
            _source.volume = defaultSoundVolume;

            roarAnim.transform.localPosition = new Vector3(3.05f, 0.12f, roarAnim.transform.localPosition.z);
            roarAnim.transform.localScale = new Vector3(2f, 2f, 1f);
            lastPlatform = Random.Range(0, arenaPlatforms.Length);
            Transform curTarget = arenaPlatforms[lastPlatform];

            //yield return StartCoroutine(JumpToTarget(new Vector2(curTarget.position.x+ platformOffsetX, curTarget.position.y + platformOffsetY)));

            jumpWaiting = Time.time - jumpDelay;
            lastAttackTime = Time.time - attackCooldown;
            isFightStart = true;
            OnStartBossBattle?.Invoke();
        }


        public override void ThrowWeapon()
        {
            if (IsDead) return;

            int numWeapons = 20;
            float angleStep = 360f / numWeapons;

            for (int i = 0; i < numWeapons; i++)
            {
                float angle = i * angleStep;
                Quaternion rotation = Quaternion.Euler(0, 0, angle);

                float distanceFromCenter = 4f; // Расстояние от центра кольца

                Vector3 spawnPosition = transform.position + rotation * new Vector3(distanceFromCenter, 0);

                var projectileBase = _projectileFactory.Create(projectileId, spawnPosition, rotation);
                
                Vector2 direction = rotation * Vector2.right;

                ThrowableProjectile projectile = projectileBase.GetComponent<ThrowableProjectile>();
                projectile.speed = 14;
                projectile.Init(direction);
                projectile.isPlayerWeapon = false;
                projectile.owner = transform;
                projectile.name = "ThrowableWeapon";
            }
        }


        IEnumerator JumpToTarget(Vector2 targetPos, bool isShot = false)
        {
            PlaySound(jumpSound);
            float timeToJump = 0.6f, jumpHeight = 5f;
            Vector2 startPos = transform.position; // Начальная позиция объекта

            float elapsedTime = 0f;
            bool hasThrownWeapon = false;

            while (elapsedTime < timeToJump)
            {
                float t = elapsedTime / timeToJump;

                // Формула для движения по дугообразной траектории
                float height = Mathf.Sin(Mathf.PI * t) * jumpHeight;

                // Обновление позиции объекта
                transform.position = Vector2.Lerp(startPos, targetPos, t) + Vector2.up * height;

                // Если isShot=true и еще не было броска оружия, и прошла половина времени прыжка
                if (isShot && !hasThrownWeapon && elapsedTime >= timeToJump / 2)
                {
                    // Вызов функции броска оружия
                    ThrowWeapon();
                    hasThrownWeapon = true; // Помечаем, что бросок оружия уже выполнен
                }

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            //transform.position = targetPos;
            yield return null;
        }

        private IEnumerator JumpAttack()
        {
            animator.Play("Jump");
            StartCoroutine(JumpToTarget(target.position));
            yield return new WaitForSeconds(1f);
            base.Attack();
            animator.Play("Jump Attack");

        }

        protected override void Patrol()
        {
            return;
        }

        protected override void Follow()
        {
            return;
        }

        protected override void Move()
        {
            if(target == null) return;
        
            float distanceToTargetX = Mathf.Abs(target.position.x - transform.position.x);
            if (distanceToTargetX < 2.5f)
            {
                attackPoint.localPosition = new Vector3(1.25f,attackPoint.localPosition.y,attackPoint.localPosition.z);
            }
            else
            {
                attackPoint.localPosition = new Vector3(3.88f, attackPoint.localPosition.y, attackPoint.localPosition.z);
            }

            if (!isFightStart) return;

            if (jumpWaiting + jumpDelay < Time.time)
            {
                int newPlatform;
                do
                {
                    newPlatform = Random.Range(0, arenaPlatforms.Length);
                } while (newPlatform == lastPlatform);
                lastPlatform = newPlatform;

                Transform curTarget = arenaPlatforms[newPlatform];
                bool isLeft = Random.Range(0, 2) == 0;
                StartCoroutine(JumpToTarget(new Vector2(curTarget.position.x + platformOffsetX + (isLeft ? 2f:-2f), curTarget.position.y + platformOffsetY), true));
                jumpWaiting = Time.time + jumpDelay + Random.Range(0, jumpDelay/3);
            }

            int isPlayerLeft = target.position.x > transform.position.x ? 1 : -1;
            transform.localScale = new Vector3(isPlayerLeft, transform.localScale.y, transform.localScale.z);

            base.Move();

        }


        protected override void TakeDamageCont()
        {
            base.TakeDamageCont();
            PlaySound(hitSound);
        }


        protected override IEnumerator Die()
        {
            yield return StartCoroutine(base.Die());
            OnBossDie?.Invoke();
        }



        protected override void Attack()
        {
            if (lastAttackTime + attackCooldown > Time.time) return;

            lastAttackTime = Time.time;

            float distanceToTargetX = Mathf.Abs(target.position.x - transform.position.x);

            if (distanceToTargetX<2.5f)
            {
                base.Attack();
                animator.Play("Punch");
                return;
            }

            int attackType = Random.Range(1, 3);
            _attacking = true;
            switch(attackType)
            {
                case 1:
                    StartCoroutine(JumpAttack()); // 20%
                    break;
                default:
                    base.Attack();
                    StartCoroutine(RoarAnimation()); // 80%
                    break;
            }
        }
    }
}
