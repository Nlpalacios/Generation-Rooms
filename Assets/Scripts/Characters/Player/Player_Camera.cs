using System.Collections;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("Player reference")]
    [SerializeField] Player player;
    //Animation camera
    [SerializeField] private float timeAnimation = .8f;

    [Header("Shake")]
    [SerializeField] private float shakeTime = .5f;
    [SerializeField] private float shakeMagnitude = .005f;
    private Vector3 cameraInitialPosition;
    private bool isCameraShake = false;

    //Private variables
    private GameManager manager;

    private void Start()
    {
        manager = GameManager.Instance;
    }

    private void OnEnable()
    {
        EventManager.Instance.Subscribe(GameWorldEvents.OnCameraShake, CameraShake);
        EventManager.Instance.Subscribe(GameWorldEvents.OnChangeRoom, ChangePositionCamera);
    }

    private void OnDisable()
    {
        EventManager.Instance.Unsubscribe(GameWorldEvents.OnCameraShake, CameraShake);
        EventManager.Instance.Unsubscribe(GameWorldEvents.OnChangeRoom, ChangePositionCamera);
    }

    public void ChangePositionCamera(object call)
    {
        GameObject newRoom = (GameObject)call;
        StopAllCoroutines();

        //Save actual room
        if (newRoom.TryGetComponent(out RoomSettings room))
        {
            StartCoroutine(MoveCamera(room.gameObject.transform.position));
        }
        else
            Debug.LogWarning("The room no contains class");
    }

    private IEnumerator MoveCamera(Vector3 targetPosition)
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(targetPosition.x, targetPosition.y, -10);
        float elapsed = 0f;

        while (elapsed < timeAnimation)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / timeAnimation);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
    }

    #region Camera Shake

    private void CameraShake(object callback)
    {
        if (isCameraShake) return;

        Debug.LogWarning("CAMARA SHAKE");
        cameraInitialPosition = transform.position;

        InvokeRepeating("CameraShaking", 0f, .005f);
        Invoke("StopCameraShaking", shakeTime);
    }

    private void CameraShaking()
    {
        float cameraShakingOffsetX = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        float cameraShakingOffsetY = Random.value * shakeMagnitude * 2 - shakeMagnitude;
        Vector3 cameraIntermediatePos = transform.position;

        cameraIntermediatePos.x += cameraShakingOffsetX;
        cameraIntermediatePos.y += cameraShakingOffsetX;
        transform.position = cameraIntermediatePos;
    }

    private void StopCameraShaking()
    {
        CancelInvoke("CameraShaking");
        transform.position = cameraInitialPosition;
    }

    #endregion
}
