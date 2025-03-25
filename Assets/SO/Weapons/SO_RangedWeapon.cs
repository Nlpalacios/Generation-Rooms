using UnityEngine;

[CreateAssetMenu(fileName = "SO_RangedWeapon", menuName = "SO_WEAPONS/SO_NewRangedWeapon")]
public class SO_RangedWeapon : SO_WeaponProperties
{
    [Space, Space]
    public GameObject bulletPrefab;

    [Space]
    public int totalBullets = 10;
    public bool isOnlyOneTrigger = false;
}