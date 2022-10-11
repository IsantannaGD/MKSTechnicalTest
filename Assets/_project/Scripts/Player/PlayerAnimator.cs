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
    [SerializeField] private Image _healthBar;
    [SerializeField] private int _healthCounter;

    void Start()
    {
        Subscribe();
        _playerShip.sprite = _shipCondition[_healthCounter];
    }

    private void UpdatePlayerHealth(float value)
    {
        float fillAmount = 1f / 3 * value;
        _healthBar.DOFillAmount(fillAmount, 0.5f);
        _healthCounter++;
        _playerShip.sprite = _shipCondition[_healthCounter];
    }

    private void Subscribe()
    {
        GameController.Instance.OnPlayerGetDamage += UpdatePlayerHealth;
    }

    private void UnSubscribe()
    {
        GameController.Instance.OnPlayerGetDamage -= UpdatePlayerHealth;
    }

    private void OnDestroy()
    {
        UnSubscribe();
    }
}
