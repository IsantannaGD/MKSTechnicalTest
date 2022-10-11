using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private GameObject _target;
    [SerializeField] private Vector3 _startDistance;

    private void Start()
    {
        _startDistance = transform.position - _target.transform.position;
    }

    private void Update()
    {
        transform.position = _target.transform.position + _startDistance;
    }
}
