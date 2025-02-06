using System;
using UnityEngine;

public class PlayerBasicEnums : MonoBehaviour
{
    //None
}

public enum playerState: short
{
    Default     = 0,
    Exploration = 1,
    Attack      = 2,

    //Cards
    OpenCards   = 3,
    Inspection  = 4
}

[Serializable]
public enum PlayerWeapon
{
    None,
    Weapon_Sword,
    Weapon_Hammer,
    Weapon_Axe,

    // Distance Weapons
    AnimationDistance,
    Weapon_Boomerang,
}

[Serializable]
public enum TypeBullet
{
    None,
    Boomerang
    //Add more
}

[Serializable]
public enum ItemsToUnlock
{
    Unlock_Axe,
    Unlock_Sword,
    Unlock_Hammer,
    Unlock_Boomerang,

    Unlock_NewHeart
}

[Serializable]
public enum TypeCombat
{
    None,
    Melee,
    Ranged,
    Ability
}