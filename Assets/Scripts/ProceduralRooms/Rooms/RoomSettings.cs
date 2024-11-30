using UnityEngine;

public abstract class RoomSettings : MonoBehaviour
{
    [Space, Header("Doors")]
    [SerializeField] private GameObject LeftDoor;
    [SerializeField] private GameObject RightDoor;
    [SerializeField] private GameObject DownDoor;
    [SerializeField] private GameObject UpDoor;

    [Space, Header("Player Detection")]
    [SerializeField] private Vector2 sizeBox = new Vector2(20,11);
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private bool activeDetector;

    public Vector2 GetArea { get => sizeBox; }

    private void Start()
    {
        InvokeRepeating("PlayerDetection", 1f, 0.2f);
    }

    private void Update()
    {
        NewUpdate();
    }

    public abstract void NewUpdate();

    //DETECTION ------------------------------
    void PlayerDetection()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, GetArea, 0f, playerLayer);
        activeDetector = player ? true : false;

        if (player)
        {
            //Debug
            Debug.DrawLine(transform.position, player.transform.position, Color.red);
            activeDetector = true;

            Camera.main.GetComponent<Player_Camera>().ChangePositionCamera(this.gameObject);
        }

    }

    //DOOR MANAGMENT ------------------------
    public void RemoveDoor(directionDoor door)
    {
        switch (door)
        {
            case directionDoor.Left:
                LeftDoor.SetActive(false);
                break;

            case directionDoor.Right:
                RightDoor.SetActive(false);
                break;

            case directionDoor.Up:
                UpDoor.SetActive(false);
                break;

            case directionDoor.Down:
                DownDoor.SetActive(false);
                break;

            default:
                Debug.Log("No se asigno una puerta correcta");
                break;
        }
    }

    //GIZMOS --------------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, GetArea);
    }
}

