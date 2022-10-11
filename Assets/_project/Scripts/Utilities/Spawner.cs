using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    private const string Shooter = "ShooterEnemy";
    private const string Chaser = "ChaserEnemy";

    [SerializeField] private Transform[] PatrolPoint;
    [SerializeField] private float _respawnTime;
    [SerializeField] private float _firstRespawnDelay;
    [SerializeField] private int _pointsCounter;
    [SerializeField] private int _enemyTotalCount;

    private void Start()
    {
        Initialization();
        Subscription();
    }

    private void Initialization()
    {
        _respawnTime = GameController.Instance.RespawnTime;
        StartCoroutine(GameLoopEnemyRespawn());
    }

    private IEnumerator GameLoopEnemyRespawn()
    {
        yield return new WaitForSeconds(_firstRespawnDelay);

        while (!GameController.Instance.GameFinished)
        {
            if (_enemyTotalCount < 10)
            {
                GameObject enemy = ObjectPooler.Instance.SpawnFromPool(RandomEnemy(), RandomPosition(), Quaternion.identity);
                enemy.GetComponent<Enemy>().SetPatrolPoints(PatrolPoint);
                _enemyTotalCount++;
            }
            
            yield return new WaitForSeconds(_respawnTime);
        }
    }

    private string RandomEnemy()
    {
        int random = Random.Range(0, 10);

        if (random > 5)
        {
            return Shooter;
        }
        else
        {
            return Chaser;
        }
    }

    private Vector3 RandomPosition()
    {
        _pointsCounter = Random.Range(0, PatrolPoint.Length);

        return PatrolPoint[_pointsCounter].position;
    }

    private void Subscription()
    {
        GameController.Instance.OnKillEnemy += value => _enemyTotalCount -= (int)value;
    }

    private void UnSubscription()
    {
        GameController.Instance.OnKillEnemy -= value => _enemyTotalCount -= (int)value;
    }

    private void OnDestroy()
    {
        UnSubscription();
    }
}
