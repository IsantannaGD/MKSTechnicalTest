using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour, IDestroyableObject, IPooledObject
{
    public Action OnFireCannon;

    [Header("** Components **")]
    [Space]

    [SerializeField] protected GeneralStatus _status;
    [SerializeField] protected NavMeshAgent _pathFinder;
    [SerializeField] protected NavMeshAgent _enemyAgent;
    [SerializeField] protected DetectionArea _detector;
    [SerializeField] protected List<Transform> _patrolPoints;
    [SerializeField] protected Transform _target;
    [SerializeField] protected SpriteRenderer _shipSprite;
    [SerializeField] protected Sprite[] _shipCondition;
    [SerializeField] protected Image _healthBar;

    [Header("** Variable **")]
    [Space]

    [SerializeField] protected Vector3 _currentPath;
    [SerializeField] protected Vector3 _direction;
    [SerializeField] protected int _patrolPointsCounter;
    [SerializeField] protected float _health;
    [SerializeField] protected int _healthCounter;
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _fireRate;
    [SerializeField] protected float _rotationSpeed;
    [SerializeField] protected float _scoreValue = 1f;
    [SerializeField] protected float _proximityRadius = 0.5f;
    [SerializeField] protected bool _fire;
    [SerializeField] protected bool _reloading;

    public void GetDamage(float damage)
    {
        _health -= damage;

        CheckHealth();
    }

    public CannonBallOwnerType Owner()
    {
        return CannonBallOwnerType.Enemy;
    }

    public void OnObjectSpawn()
    {
        Initialization();
        SingleInitialization();
    }

    public void SetPatrolPoints(params Transform[] points)
    {
        _patrolPoints.Clear();

        foreach (Transform point in points)
        {
            _patrolPoints.Add(point);
        }

        _patrolPointsCounter = Random.Range(0, _patrolPoints.Count);
        _currentPath = _patrolPoints[_patrolPointsCounter].position;
        _pathFinder.SetDestination(_currentPath);
    }

    protected virtual void SingleInitialization(){}
    protected abstract string EnemyType();
    
    private void Start()
    {
        Initialization();
        SingleInitialization();
    }

    protected virtual void Update()
    {
        if (GameController.Instance.GameFinished || GameController.Instance.GamePaused)
        {
            return;
        }

        if (_fire)
        {
           _direction = (_target.position - _enemyAgent.transform.position).normalized;
        }
        else
        {
            _direction = (_pathFinder.transform.position - _enemyAgent.transform.position).normalized;
        }
        

        float resultAngle = Vector3.SignedAngle(_enemyAgent.transform.right, _direction, _enemyAgent.transform.forward);
        _enemyAgent.transform.Rotate(0f,0f, resultAngle * _rotationSpeed * Time.deltaTime);
    }

    protected virtual void LateUpdate()
    {
        if (_fire)
        {
            _enemyAgent.SetDestination(_enemyAgent.transform.position);
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

    private void Initialization()
    {
        _health = _status.MaxHealth;
        _moveSpeed = _status.MovementSpeed;
        _rotationSpeed = _status.RotationSpeed;

        _enemyAgent.updateRotation = false;
        _enemyAgent.updateUpAxis = false;
        _enemyAgent.speed = _moveSpeed;
        _pathFinder.speed = _moveSpeed + 0.3f;
    }

    protected abstract void EnemyAttack();

    private void CheckHealth()
    {
        float fillAmount = 1f / _status.MaxHealth * _health;
        _healthBar.DOFillAmount(fillAmount, 0.5f);
        _healthCounter++;

        if (_health <= 0)
        {
            GameController.Instance.OnKillEnemy.Invoke(_scoreValue);
            ObjectPooler.Instance.SpawnFromPoolWithReturn("DeathExplosion", _enemyAgent.transform.position,
                Quaternion.identity);
            ObjectPooler.Instance.ReturnToPool(EnemyType(), gameObject);
            return;
        }

        ChangeSpriteShip();
    }

    private void ChangeSpriteShip()
    {
        _shipSprite.sprite = _shipCondition[_healthCounter];
    }
}
