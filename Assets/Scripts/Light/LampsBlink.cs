using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class LampsBlink : MonoBehaviour
{
    private Animator[] _lamps;
    [SerializeField] [Range(0,1)] private float _frequency;
    
    
    void Start()
    {
        _lamps = new Animator[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            _lamps[i] = child.GetComponent<Animator>();
        }
    }

    void Update()
    {
        if (Random.value  < _frequency * Time.deltaTime)
            Blink();
    }

    private void Blink()
    {
        foreach (var lamp in _lamps)
        {
            lamp.SetTrigger("Blink");
        }
    }
}
