using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(HealthControl))]
[RequireComponent(typeof(AnimationControl))]
[RequireComponent(typeof(CombatSystem))]
public class Player : MonoBehaviour
{
    [Header("Player Attributes")]
    [SerializeField] [Range(0, 10)] private float playerSpeed = 6;

    //Private Variables

    //Default PLAYER direction
    Vector2 actualDirection = Vector2.down;

    //Input
    private InputAction IA_playerMove;
    private InputAction IA_PlayerAttack;

    //Move player
    private Vector2 vectorPlayerMove;

    //CUSTOM COMPONENTS
    private HealthControl healthControl;
    private AnimationControl animator;
    private CombatSystem combatSystem;

    //UNITY COMPONENTS
    private Rigidbody2D rbPlayer;


    public Vector2 GetDirection { get => actualDirection; }

    #region Enable / Disable
    private void OnEnable()
    {
        //Initialize components
        rbPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<AnimationControl>();
        combatSystem = GetComponent<CombatSystem>();
        healthControl = GetComponent<HealthControl>();

        healthControl.OnDeathCharacter += Death;

        if (InputManager.Instance )
        {
            IA_playerMove = InputManager.Instance.GetInputActionMovement();
            IA_PlayerAttack = InputManager.Instance.GetInputActionAttack();

            // Subscribe to the performed and canceled events
            IA_playerMove.Enable();
            IA_playerMove.performed += CharacterMove;
            IA_playerMove.canceled += CharacterMove;

            IA_PlayerAttack.Enable();
            IA_PlayerAttack.started += PlayerAttack;
        }
    }

    private void OnDisable()
    {
        if (IA_playerMove == null || IA_PlayerAttack == null) return;
        
        IA_playerMove.performed -= CharacterMove;
        IA_playerMove.Disable();

        IA_PlayerAttack.started += PlayerAttack;
        IA_PlayerAttack.Disable();

    }

    #endregion

    void Start()
    {
        if (!InputManager.Instance || !GameManager.Instance)
        {
            Debug.LogWarning($"NO MANAGERS IN -{transform.name}-");
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q)) {
        //    GameManager.Instance.Resolve<IStateManagment>().SetPlayerState(playerState.OpenCards);
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    GameManager.Instance.Resolve<IStateManagment>().SetPlayerState(playerState.Exploration);
        //}
    }

    private void FixedUpdate()
    {
        //Moving player
        Vector2 movePlayer = vectorPlayerMove * playerSpeed * Time.deltaTime;
        rbPlayer.velocity = movePlayer;
        transform.position += (Vector3)movePlayer;

        GetLastDirection();
    }

    #region Move
    void CharacterMove(InputAction.CallbackContext context)
    {
        vectorPlayerMove = context.ReadValue<Vector2>().normalized;
    }

    #endregion

    #region Attack

    void PlayerAttack(InputAction.CallbackContext context)
    {
        combatSystem.CharacterAttack(context);
    }

    #endregion

    #region Getters

     public void GetLastDirection()
     {
        if (rbPlayer.velocity.x != 0 && rbPlayer.velocity.y == 0)
        {
            actualDirection = rbPlayer.velocity.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (rbPlayer.velocity.y != 0 && rbPlayer.velocity.x == 0)
        {
            actualDirection = rbPlayer.velocity.y > 0 ? Vector2.up : Vector2.down;
        }
        else if (rbPlayer.velocity.x != 0)
        {
            actualDirection = rbPlayer.velocity.x > 0 ? Vector2.right : Vector2.left;
        }
        else if (rbPlayer.velocity.y != 0)
        {
            actualDirection = rbPlayer.velocity.y > 0 ? Vector2.up : Vector2.down;
        }
     }

    #endregion


    //Death -------------------------------------------
    void Death()
    {
        Debug.Log("Murio");
    }
}