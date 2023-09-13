using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveByArrows : MonoBehaviour
{

    [SerializeField] private float _speed;
    private Vector3 _direction;
    
    void Update()
    {
        _direction.x = Input.GetAxis("Horizontal") * _speed;
        _direction.y = Input.GetAxis("Vertical") * _speed;

        transform.position += _direction * Time.deltaTime;
    }
}
