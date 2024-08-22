using UnityEngine;

public class playerStateClass : MonoBehaviour
{
    //None
}

public enum playerState: short
{
    Default     = 0,
    Exploration = 1,
    Atack       = 2,

    //Cards
    OpenCards   = 3,

}

public enum PlayerWeapon : short
{
    None = 0,
    Sword = 1,
    Hammer = 2,
    Axe = 3
}