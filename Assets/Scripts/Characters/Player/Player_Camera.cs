using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("Player reference")]
    [SerializeField] Player player;

    //Private variables
    private GameManager manager;

    private void Start()
    {
        manager = GameManager.Instance;
    }

    public void ChangePositionCamera(GameObject room)
    {
        if (manager.GetCurrentRoom == room ) return;

        StopAllCoroutines();

        //Save actual room
        manager.SaveActualRoom(room);

        StartCoroutine(MoveCamera(room.transform.position));
        //Debug.Log($"Camera centered on {room.name}");
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, -10);
        float duration = .8f; // Duration of the lerp in seconds
        float elapsed = 0f;

        //Event move camera
        manager.EventCameraMoving();

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;

        //Event stop camera
        manager.EventCameraMoving(false);
    }

}
