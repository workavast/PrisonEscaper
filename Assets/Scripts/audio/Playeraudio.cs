using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using Random = UnityEngine.Random;

public class Playeraudio : MonoBehaviour
{
    [SerializeField] private AudioClip _walk;
    [SerializeField] private AudioClip[] _attack;

    private Rigidbody2D _rigidbody;
    
    private AudioSource sours;
    private bool attacking;
    void Start()
    {
        sours = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 && !attacking && sours.clip != _walk && Mathf.Abs(_rigidbody.velocity.y) < 0.001f)
        {
            Debug.Log("Play Run");
            sours.clip = _walk;
            sours.loop = true;
            sours.Play();
        }
        else if (!attacking && (Input.GetAxis("Horizontal") == 0 || attacking || Mathf.Abs(_rigidbody.velocity.y) > 0.001f))
        {
            sours.clip = null;
        }
    }

    public void PlayAttack()
    {
        if(!attacking)
        {
            attacking = true;
            var attack = _attack[Random.Range(0, _attack.Length)];
            sours.clip = attack;
            sours.loop = false;
            sours.Play();
            StartCoroutine(ResetAttackFlag());
        }
    }

    private IEnumerator ResetAttackFlag()
    {
        Debug.Log($"Play attack {sours.clip.length}");
        yield return new WaitForSeconds(sours.clip.length);
        Debug.Log("attack sadf");
        attacking = false;
    }
}
