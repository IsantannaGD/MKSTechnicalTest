using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnReturner : MonoBehaviour, IPooledObject
{
    [SerializeField] private string _objectName;
    [SerializeField] private float _returnTime;

    public void OnObjectSpawn()
    {
        StartCoroutine(ObjectPooler.Instance.ReturnToPoolAfterSeconds(_objectName, gameObject, _returnTime));
    }
}
