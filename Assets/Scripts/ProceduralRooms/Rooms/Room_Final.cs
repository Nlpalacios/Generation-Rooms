using UnityEngine;

public class Room_Final : RoomSettings
{
    [Space]
    [SerializeField] private GameObject finalDoor;
    [SerializeField] private Vector3 scaleDoor;

    private void OnEnable()
    {
        if (finalDoor == null) return;

        GameObject doorObject = Instantiate(finalDoor, this.transform);

        doorObject.transform.localScale = scaleDoor;
        doorObject.transform.position = ((Vector2)this.transform.localPosition);
    }

    public override void NewUpdate()
    {
    }
}
