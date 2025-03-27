using UnityEngine;

public class Room_Initial : RoomSettings
{
    [Space]
    [SerializeField] private byte countInitialItems = 2;

    private void OnEnable()
    {
        if (ItemManager.Instance != null && ItemManager.Instance.hasBoomerang)
        {
            SpawnItem((byte)(countInitialItems - 1));
            return;
        }

        PlayerWeapon[] weapons = { PlayerWeapon.Weapon_Boomerang };
        SpawnItem(countInitialItems, weapons);
    }

    public override void NewUpdate(){}
    public override void OnPlayerEnter(){}

    public override void OnCloseDoor() {}
    public override void OnOpenDoor() {}
}
