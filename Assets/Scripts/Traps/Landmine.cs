using System.Collections;
using Character;
using UnityEngine;
using UniversalStatsSystem;

public class Landmine : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats = new AttackStats(10);
    [SerializeField] private float explosionCooldown = .2f;
    [SerializeField] private float explosionRange;
    [SerializeField] private AudioClip _exploisionSound;
    
    private Animator _animator;
    private AudioSource source;
    private Player _player;

    void Start()
    {
        _animator = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _player = other.gameObject.GetComponent<Player>();
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(explosionCooldown);
        _animator.SetTrigger("Explode");
        
        source.clip = _exploisionSound;
        source.Play();
        source.loop = false;
        
        if (Vector2.Distance(transform.position, _player.gameObject.transform.position) <= explosionRange)
            _player.TakeDamage(attackStats, transform.position);
    }

    private void DestroyObject()
    {
        Destroy(transform.parent.gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRange);
    }
}
