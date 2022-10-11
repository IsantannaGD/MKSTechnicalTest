using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDestroyableObject
{
    public void GetDamage(float damage);
    public CannonBallOwnerType Owner();
}
