using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ChaserEnemy : Enemy
{
    [Header("** Chaser Components **")]
    [Space]

    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private ChaserCollisionDetector _collisionDetector;

    [Header("** Chaser Variable **")]
    [Space]

    [SerializeField] private float _sprintSpeed = 1.5f;
    [SerializeField] private float _damage = 1f;
    [SerializeField] private bool _targetLocked;

    protected override void SingleInitialization()
    {
        _collisionDetector.OnPlayerHit += PlayerDamaged;
        _detector.OnPlayerDetected += (target, value) =>
        {
            if (_targetLocked)
            {
                return;
            }

            _target = target;
            _fire = value;
            _targetLocked = true;
        };
    }

    protected override void LateUpdate()
    {
        if (GameController.Instance.GameFinished || GameController.Instance.GamePaused)
        {
            return;
        }

        if (_fire)
        {
            if (_reloading)
            {
                _enemyAgent.SetDestination(_enemyAgent.transform.position);
            }
            else
            {
               EnemyAttack(); 
            }
            
            
            EnemyAttack();
        }
        else
        {
            float distance = Vector3.Distance(_pathFinder.transform.position, _currentPath);

            if (distance <= _proximityRadius)
            {
                _patrolPointsCounter = Random.Range(0, _patrolPoints.Count);
                _currentPath = _patrolPoints[_patrolPointsCounter].position;

                _pathFinder.SetDestination(_currentPath);
            }

            _enemyAgent.SetDestination(_pathFinder.transform.position);
        }
    }
    protected override string EnemyType()
    {
        return "ChaserEnemy";
    }

    private void PlayerDamaged(IDestroyableObject player, Transform playerPos)
    {
        if (_reloading)
        {
            return;
        }
            
        _reloading = true;

        player.GetDamage(_damage);

        GameController.Instance.OnplaySfx(_explosionSound, 1f);
        ObjectPooler.Instance.SpawnFromPoolWithReturn("Explosion", playerPos.position, Quaternion.identity);

        StartCoroutine(ReloadingTime());
    }

    private IEnumerator ReloadingTime()
    {
        yield return new WaitForSeconds(_fireRate);
        _reloading = false;
    }

    protected override void EnemyAttack()
    {
        _enemyAgent.speed = _moveSpeed * _sprintSpeed;
        _enemyAgent.stoppingDistance = 0.4f;
        _enemyAgent.SetDestination(_target.transform.position);
    }

    private void OnDisable()
    {
        _collisionDetector.OnPlayerHit -= PlayerDamaged;
        _detector.OnPlayerDetected -= (target, value) =>
        {
            if (_targetLocked)
            {
                return;
            }

            _target = target;
            _targetLocked = true;
        };
    }
}
