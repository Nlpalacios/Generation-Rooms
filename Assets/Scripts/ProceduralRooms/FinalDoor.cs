using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    private RoomSettings room;

    private void OnEnable()
    {
        room = GetComponentInParent<RoomSettings>();
        if (room == null)
        {
            Debug.LogWarning("NO FIND ROOM PARENT");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null || room == null) return;

        if (collision.tag == "Player" && room.IsOpenRoom)
        {
            EventManager.Instance.TriggerEvent(GameWorldEvents.OnFinishLoop);
        }
    }
}
