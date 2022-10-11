using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserCollisionDetector : MonoBehaviour
{
    public Action<IDestroyableObject, Transform> OnPlayerHit;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.TryGetComponent(out PlayerPhysics player))
        {
            IDestroyableObject i = player.transform.parent.GetComponent<IDestroyableObject>();
            OnPlayerHit?.Invoke(i, col.transform);
        }
    }
}
