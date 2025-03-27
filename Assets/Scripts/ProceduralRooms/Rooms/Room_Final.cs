using UnityEngine;

public class Room_Final : RoomSettings
{
    [Space]
    [SerializeField] private GameObject prefabDoor;
    [SerializeField] private Vector3 scaleDoor;
    private FinalDoor finalDoor;

    bool IsFirstLoop() => GameManager.Instance.GetCurrentLoop == 1;

    private void OnEnable()
    {
        if (prefabDoor == null) return;

        if (IsFirstLoop())
        {
            InstantiateAllEnemies();
        }
        GameObject doorObject = Instantiate(prefabDoor, this.transform);

        doorObject.transform.localScale = scaleDoor;
        doorObject.transform.position = ((Vector2)this.transform.localPosition);
        finalDoor = doorObject.GetComponent<FinalDoor>();
    }

    public override void OnPlayerEnter()
    {
        if (IsFirstLoop())
        {
            ActiveEnemies();
        }
    }

    public override void NewUpdate() { }
    public override void OnCloseDoor()
    {
        if (finalDoor)
            finalDoor.OpenDoor(false);
    }
    public override void OnOpenDoor()
    {
        if (finalDoor)
            finalDoor.OpenDoor(true);
    }
}
