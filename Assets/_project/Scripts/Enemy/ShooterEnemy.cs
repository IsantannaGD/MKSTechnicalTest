using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    protected override void SingleInitialization()
    {
        _detector.OnPlayerDetected += (target, value) =>
        {
            _target = target;
            _fire = value;
        };
    }

    protected override string EnemyType()
    {
        return "ShooterEnemy";
    }
    protected override void EnemyAttack()
    {
        if (_reloading || GameController.Instance.GamePaused || GameController.Instance.GameFinished)
        {
            return;
        }

        _reloading = true;
        OnFireCannon?.Invoke();
        StartCoroutine(ReloadingTime());
    }

    private IEnumerator ReloadingTime()
    {
        yield return new WaitForSeconds(_fireRate);
        _reloading = false;
    }

    private void OnDisable()
    {
        _detector.OnPlayerDetected -= (target, value) =>
        {
            _target = target;
            _fire = value;
        };
    }
}
