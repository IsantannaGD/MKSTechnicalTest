using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDestroyableObject
{
    public delegate void PlayerEvents();
    public PlayerEvents OnShootCannon;
    public PlayerEvents OnShootArtillery;

    [Header("** Player Environment **")]
    [Space]

    public PlayerPhysics physics;
    public PlayerAnimator anim;
    public GeneralStatus status;

    [Header("** Variable **")]
    [Space]

    [SerializeField] private float _playerHealth;
    [SerializeField] private float _cannonReloadTime = 1f;
    [SerializeField] private float _artilleryReloadTime = 3f;
    [SerializeField] private bool _isAlive = false;
    [SerializeField] private bool _cannonReloading;
    [SerializeField] private bool _artilleryReloading;

    public void GetDamage(float damage)
    {
        _playerHealth -= damage;

        GameController.Instance.OnPlayerGetDamage?.Invoke(_playerHealth);
        CheckPlayerHealth();
    }

    public CannonBallOwnerType Owner()
    {
        return CannonBallOwnerType.Player;
    }

    private void Start()
    {
        physics.Initialize(status);
        _playerHealth = status.MaxHealth;
        _isAlive = true;
    }

    private void Update()
    {
        CannonShoot();
        ArtilleryShoot();
    }

    private void CannonShoot()
    {
        if (_cannonReloading || GameController.Instance.GamePaused || !_isAlive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _cannonReloading = true;
            OnShootCannon?.Invoke();
            StartCoroutine(ReloadCannon());
        }
    }

    private void ArtilleryShoot()
    {
        if (_artilleryReloading || GameController.Instance.GamePaused || !_isAlive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _artilleryReloading = true;
            OnShootArtillery?.Invoke();
            StartCoroutine(ReloadArtillery());
        }
    }

    private IEnumerator ReloadCannon()
    {
        GameController.Instance.OnShootCannon?.Invoke(_cannonReloadTime);
        yield return new WaitForSeconds(_cannonReloadTime);
        _cannonReloading = false;
    }

    private IEnumerator ReloadArtillery()
    {
        GameController.Instance.OnShootArtillery?.Invoke(_artilleryReloadTime);
        yield return new WaitForSeconds(_artilleryReloadTime);
        _artilleryReloading = false;
    }

    private void CheckPlayerHealth()
    {
        if (_playerHealth <= 0)
        {
            _isAlive = false;
            GameController.Instance.OnPlayerDead?.Invoke();
        }
    }
}
