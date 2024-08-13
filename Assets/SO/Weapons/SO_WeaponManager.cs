using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WeaponManager", menuName = "Scriptable Objects/SO_WeaponManager")]
public class SO_WeaponManager : ScriptableObject
{
    [Header("Type")]
    public PlayerWeapon type;

    [Header("Value")]
    public int damage;
    public float delay;
    public float maxScope;

    [Header("Animations")]
    public AnimationClip attack_Left;
    public AnimationClip attack_Right;
    public AnimationClip attack_Up;
    public AnimationClip attack_Down;
}