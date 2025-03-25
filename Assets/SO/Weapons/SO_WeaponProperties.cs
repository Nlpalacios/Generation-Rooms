using System;
using UnityEngine;

public abstract class SO_WeaponProperties : ScriptableObject
{
    [Header("Properties")]
    public PlayerWeapon typeWeapon;
    public TypeCombat typeCombat;

    [Header("Values")]
    public Values basicValues;

    [Header("Animations")]
    public Animations totalAnimations;

    [Serializable]
    public class Animations
    {
        public AnimationClip attack_Left;
        public AnimationClip attack_Right;
        [Space]
        public AnimationClip attack_Up;
        public AnimationClip attack_Down;
    }

    [Serializable]
    public class Values
    {
        public int damage;
        public float delay;
        public float maxScope;
    }
}