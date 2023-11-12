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
        if (transform.localScale.x <= 0f)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y, transform.localScale.z);
        }
    }
}
