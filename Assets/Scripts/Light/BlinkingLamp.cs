using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BlinkingLamp : MonoBehaviour
{
    void Start()
    {
        if(GlobalLampBlinks.Instance == null) return;
        
        GlobalLampBlinks.Instance.AddLamp(GetComponent<Animator>());
    }
}
