using System;
using UnityEngine;

public class TypeEvents 
{

}

[Serializable]
public enum GameWorldEvents
{
    OnChangeState,
    OnCameraShake
}

[Serializable]
public enum PlayerEvents
{
    OnStopMovement,
    OnReceiveDamage,
    OnChangeHealth
}

[Serializable]
public enum RoomsEvents
{
    OnLockDoors,
    OnUnlockDoors
}


[Serializable]
public enum EnemiesEvents
{
    OnReceiveDamage,
    OnUnlockDamage
}

