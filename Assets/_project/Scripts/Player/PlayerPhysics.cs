using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerPhysics : MonoBehaviour
{
    [Header("** Components **")]
    [Space]
    
    [SerializeField] private Vector2 _movement;
    [SerializeField] private Rigidbody2D _rigB;

    [Header("** Variable **")]
    [Space]

    [SerializeField] private float _rotationAngle;
    [SerializeField] private float _speedModifier;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    public void Initialize(GeneralStatus playerStatus)
    {
        _moveSpeed = playerStatus.MovementSpeed;
        _rotateSpeed = playerStatus.RotationSpeed;
    }

    private void LateUpdate()
    {
        Movement();
    }

    private void Movement()
    {
        if (GameController.Instance.GamePaused || GameController.Instance.GameFinished)
        {
            return;
        }

        _movement.x = Input.GetAxis("Horizontal");
        _movement.y = Input.GetAxis("Vertical");
        _rigB.velocity = _movement * _moveSpeed;

        if (_movement != Vector2.zero)
        {
            Rotate();
        }
    }

    private void Rotate()
    { 
        _rotationAngle = Mathf.Atan2(_movement.y, _movement.x) * Mathf.Rad2Deg;
        transform.rotation = (Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,0,_rotationAngle), _rotateSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out CoastLand coast))
        {
            _moveSpeed -= _speedModifier;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out CoastLand coast))
        {
            _moveSpeed += _speedModifier;
        }
    }
}
