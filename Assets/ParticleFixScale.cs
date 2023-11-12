using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFixScale : MonoBehaviour
{
    private void Start()
    {

    }
    void Update()
    {
     //   float parentScaleX = transform.parent.localScale.x;
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
}
