using UnityEngine;

public class Room_Initial : RoomSettings
{
    [Space]
    [SerializeField] private int countInitialItems = 2;

    private void OnEnable()
    {
        PlayerWeapon[] weapons = { PlayerWeapon.Weapon_Boomerang };
        SpawnItem(2, weapons);
    }

    public override void NewUpdate(){}
}
