using System.Collections;
using Character;
using UnityEngine;
using UnityEngine.Serialization;
using UniversalStatsSystem;

public class Saw : MonoBehaviour
{
    [SerializeField] private AttackStats attackStats;
    [SerializeField] private float riseSpeed;
    [SerializeField] private float timeOffset;
    [SerializeField] private bool riseable;
    [SerializeField] private float riseOffset;
    [FormerlySerializedAs("risecCooldown")] [SerializeField] private float riseCooldown;

    [SerializeField] private AudioSource _source;
    
    
    private Vector3 _startPosition;
    private Vector3 _direction;
    private Player _player;
    private float _attackCooldown = 1;
    private bool _sleep, _onCooldown, waitStart = true;
    void Start()
    {
        _startPosition = transform.position;
        _direction = Vector3.down;
        _source = GetComponent<AudioSource>();
        _source.loop = true;
        _source.Stop();
        StartCoroutine(WaitForTimeOffset());
    }
    
    void FixedUpdate()
    {
        if (riseable && !_sleep && !waitStart)
            Move();
    }

    private void Move()
    {
        if (transform.position.y > _startPosition.y || transform.position.y < _startPosition.y - riseOffset)
        {
            _direction *= -1;
            StartCoroutine(Sleep());
            _sleep = true;
        }
        
        transform.position += _direction * (riseSpeed * Time.fixedDeltaTime);
    }

    private IEnumerator Sleep()
    {
        yield return new WaitForSeconds(riseCooldown);
        _sleep = false;
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player") && !_onCooldown)
        {
            _onCooldown = true;
            _player = _player ? _player : col.gameObject.GetComponent<Player>();
            _player.TakeDamage(attackStats, transform.position);
            StartCoroutine(resetCooldown());
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.CompareTag("Player"))
            _source.Play();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
            _source.Stop();
    }


    private IEnumerator resetCooldown()
    {
        yield return new WaitForSeconds(_attackCooldown);
        _onCooldown = false;
    }

    private IEnumerator WaitForTimeOffset()
    {
        yield return new WaitForSeconds(timeOffset);
        waitStart = false;
    }
}
