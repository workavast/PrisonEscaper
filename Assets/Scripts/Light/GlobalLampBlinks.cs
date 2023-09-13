using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GlobalLampBlinks : MonoBehaviour
{
    public static GlobalLampBlinks Instance { get; private set; }
    
    [SerializeField] [Range(0,1)] private float frequency;
    [SerializeField] [Range(0,1)] private float minLampBlinkPercent;
    [SerializeField] [Range(0,1)] private float maxLampBlinkPercent;

    
    private List<Animator> _lamps = new List<Animator>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (Random.value < frequency * Time.deltaTime)
            Blink();
    }

    private void Blink()
    {
        int randomLampCount = Random.Range((int)(_lamps.Count * minLampBlinkPercent), (int)(_lamps.Count * maxLampBlinkPercent));

        List<int> blinkLamps = new List<int>();
        for (int i = 0; i < randomLampCount; i++)
        {
            int lampIndex = Random.Range(0, _lamps.Count);
            if (blinkLamps.Contains(lampIndex))
            {
                i--;
                continue;
            }
            
            _lamps[lampIndex].SetTrigger("Blink");
            blinkLamps.Add(lampIndex);
        }
    }

    public void AddLamp(Animator newAnimator)
    {
        _lamps.Add(newAnimator);
    }

    public void Clear()
    {
        _lamps = new List<Animator>();
    }
}
