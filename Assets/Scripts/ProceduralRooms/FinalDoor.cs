using UnityEngine;

public class FinalDoor : MonoBehaviour
{
    [SerializeField] private Animator animator;
    bool isCurrentOpen = false;

    public void OpenDoor(bool isOpen)
    {
        animator.SetBool("OpenDoor", isOpen);
        isCurrentOpen = isOpen;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.tag == "Player" && isCurrentOpen)
        {
            EventManager.Instance.TriggerEvent(GameWorldEvents.OnFinishLoop);
        }
    }
}
