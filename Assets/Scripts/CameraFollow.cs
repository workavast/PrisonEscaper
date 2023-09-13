using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private Vector3 _offset;

    private void Start()
    {
        _offset.z = transform.position.z;
        transform.position = _target.transform.position  + _offset;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,
            _target.position + _offset,
            Time.deltaTime * _speed);
    }
}
