using System;

public class TypeEvents 
{

}

[Serializable]
public enum GameWorldEvents
{
    OnChangeState,
    OnCameraShake,
    OnChangeRoom,

    OnGenerateRooms,
    OnUpdateRooms,

    OnFinishLoop
}

[Serializable]
public enum CombatEvents
{
    OnStartPlayerAbility,
    OnStartAbility,
    OnUnlockAbility
}

[Serializable]
public enum PlayerEvents
{
    OnChangeWeapon,
    OnStopMovement,

    OnReceiveDamage,
    OnDeath,

    OnReceiveUpgrade,
    OnUpdateUI,

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

