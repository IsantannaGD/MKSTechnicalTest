using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryCannon : MonoBehaviour
{
    [SerializeField] private Player _player;

    private void Start()
    {
        Initialization();
    }

    private void Initialization()
    {
        _player.OnShootArtillery += ShootArtilleryCannon;
    }

    private void ShootArtilleryCannon()
    {
        ObjectPooler.Instance.SpawnFromPoolWithReturn("ArtilleryCannonBall", transform.position, transform.rotation);
    }
}
