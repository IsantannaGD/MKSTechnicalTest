using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _playerShip;
    [SerializeField] private Sprite[] _shipCondition;
    [SerializeField] private GameObject _fireOnTheShip;
    [SerializeField] private Image _healthBar;
    [SerializeField] private int _healthCounter;
    [SerializeField] private bool _isDead;

    void Start()
    {
        Subscribe();
        _playerShip.sprite = _shipCondition[_healthCounter];
    }

    private void UpdatePlayerHealth(float value)
    {
        if (_isDead)
        {
            return;
        }

        float fillAmount = 1f / 3 * value;
        _healthBar.DOFillAmount(fillAmount, 0.5f);
        _healthCounter++;
        
        ChangeSpriteShip();
    }

    private void ChangeSpriteShip()
    {
        _playerShip.sprite = _shipCondition[_healthCounter];

        if (_healthCounter >= 2)
        {
            _fireOnTheShip.SetActive(true);
        }
    }

    private void PlayerDeadExplosion()
    {
        ObjectPooler.Instance.SpawnFromPoolWithReturn("DeathExplosion", transform.position, Quaternion.identity);
    }

    private void Subscribe()
    {
        GameController.Instance.OnPlayerGetDamage += UpdatePlayerHealth;
        GameController.Instance.OnPlayerDead += PlayerDeadExplosion;
        GameController.Instance.OnPlayerDead += () => _isDead = true;
    }

    private void UnSubscribe()
    {
        GameController.Instance.OnPlayerGetDamage -= UpdatePlayerHealth;
        GameController.Instance.OnPlayerDead -= PlayerDeadExplosion;
        GameController.Instance.OnPlayerDead -= () => _isDead = true;
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}
