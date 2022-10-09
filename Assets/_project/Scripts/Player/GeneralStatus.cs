using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GeneralStats", menuName = "ScriptableObjects/GeneralStats", order = 2)]
public class GeneralStatus : ScriptableObject
{
    public float MaxHealth;
    public float MovementSpeed;
    public float RotationSpeed;
}
