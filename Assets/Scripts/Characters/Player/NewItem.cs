using System.Collections;
using UnityEngine;

public class NewItem : MonoBehaviour
{
    [Header("Type Item")]
    public ItemsToUnlock item;

    [Header("Animation")]
    [SerializeField] private float distanceUp = 2f;
    [SerializeField] private float timeAnimation = 0.5f;

    private Vector3 finalPosition = Vector3.zero;
    private Vector3 initialPosition = Vector3.zero;

    private bool takeThisItem = false;

    private void Update()
    {
        if (GameManager.Instance.GetCurrentState == playerState.Inspection && takeThisItem)
        {
            if (Input.anyKeyDown)
            {
                GameManager.Instance.SetPlayerState(playerState.Exploration);
                this.gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.TryGetComponent(out Player player))
        {
            EventManager.Instance.TriggerEvent(PlayerEvents.OnUnlockItem, (NewItem)this);
            EventManager.Instance.TriggerEvent(GameWorldEvents.OnChangeState, playerState.Inspection);

            initialPosition = GameManager.Instance.GetPlayer.transform.position;
            finalPosition = new Vector3(initialPosition.x, initialPosition.y + distanceUp, 0);

            transform.position = initialPosition;
            StartCoroutine(UpAnimation());
        }
    }

    private IEnumerator UpAnimation()
    {
        float elapsedTime = 0;

        while (elapsedTime < timeAnimation)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / timeAnimation);

            yield return null;
        }

        takeThisItem = true;
        transform.position = finalPosition;
    }
}