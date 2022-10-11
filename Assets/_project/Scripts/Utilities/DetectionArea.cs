using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{
    public Action<Transform, bool> OnPlayerDetected;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlayerPhysics player))
        {
            OnPlayerDetected?.Invoke(player.transform, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.TryGetComponent(out PlayerPhysics player))
        {
            OnPlayerDetected?.Invoke(null, false);
        }
    }
}
