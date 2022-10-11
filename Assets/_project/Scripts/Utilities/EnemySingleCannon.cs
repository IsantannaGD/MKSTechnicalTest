using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySingleCannon : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _enemy.OnFireCannon += ShootCannon;
    }

    private void ShootCannon()
    {
        ObjectPooler.Instance.SpawnFromPool("EnemyCannonBall", transform.position, transform.rotation);
    }
}
