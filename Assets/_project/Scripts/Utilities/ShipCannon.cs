using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ShipCannon : MonoBehaviour
{
    [SerializeField] private Player _player;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _player.OnShootCannon += ShootCannon;
    }

    private void ShootCannon()
    {
        ObjectPooler.Instance.SpawnFromPool("CannonBall", transform.position, transform.rotation);
    }
}
