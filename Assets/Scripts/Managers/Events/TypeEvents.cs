using System;

public class TypeEvents 
{

}

[Serializable]
public enum GameWorldEvents
{
    OnChangeState,
    OnCameraShake,
    OnChangeRoom
}

[Serializable]
public enum CombatEvents
{
    OnChangeTypeCombat,
    OnChangeWeapon
}

[Serializable]
public enum PlayerEvents
{
    OnUnlockItem,
    OnStopMovement,
    OnReceiveDamage,
    OnDeath,

    OnChangeExperience,
    OnLevelUp
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
    OnEnableEnemy,
    OnReceiveDamage,
    OnUnlockDamage
}

