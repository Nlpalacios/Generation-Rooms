using System;
using UnityEngine;

public class PlayerBasicEnums : MonoBehaviour
{
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
public enum TypeCombat
{
    None,
    Melee,
    Ranged
}

[Serializable]
public enum PlayerItems
{
    None,

    MiddleHeart,
    NewHeart
}


[Serializable]
public enum TypeItem
{
    None,
    Weapon,
    player
}
