using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
//[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(CombatSystem))]
[RequireComponent(typeof(InputManager))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AnimationControl))]
public class Player : MonoBehaviour
{
    [SerializeField] private PlayerStats playerStats;

    //Default PLAYER direction
    private Vector2 actualDirection = Vector2.down;

    //Input
    private InputAction IA_playerMove;
    private InputAction IA_PlayerAttack;

    //Move player
    private bool isCanMove = true;
    private Vector2 vectorPlayerMove;

    //CUSTOM COMPONENTS
    private InputManager inputManager;
    private AnimationControl animator;
    private CombatSystem combatSystem;

    //UNITY COMPONENTS
    private Rigidbody2D rbPlayer;
    public Vector2 GetDirection { get => actualDirection; }


    #region Enable / Disable

    private void OnEnable()
    {
        //Initialize components
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
        animator = GetComponent<AnimationControl>();
        combatSystem = GetComponent<CombatSystem>();
        inputManager = GetComponent<InputManager>();

        SubscribeEvents();
    }

    private void OnDisable()
    {
        UnsubscribeEvents();
        if (IA_playerMove == null || IA_PlayerAttack == null) return;
           
        IA_playerMove.performed -= CharacterMove;
        IA_playerMove.Disable();

        IA_PlayerAttack.started += PlayerAttack;
        IA_PlayerAttack.Disable();
    }

    #endregion

    #region Events

    private void SubscribeEvents()
    {
        EventManager.Instance.Subscribe(PlayerEvents.OnStopMovement, StopMovement);
        playerStats.OnPlayerDeath += Death;
    }

    private void UnsubscribeEvents()
    {
        EventManager.Instance.Unsubscribe(PlayerEvents.OnStopMovement, StopMovement);
        playerStats.OnPlayerDeath -= Death;
    }

    #endregion

    void Start()
    {
        if (!GameManager.Instance)
        {
            Debug.LogWarning($"NO MANAGERS IN -{transform.name}-");
        }
        if (!inputManager) return;

        IA_playerMove = inputManager.GetInputAction(InputActionsEnum.Movement);
        IA_PlayerAttack = inputManager.GetInputAction(InputActionsEnum.Attack);
        if (IA_PlayerAttack == null || IA_playerMove == null) return;

        // Subscribe to the performed and canceled events
        IA_playerMove.Enable();
        IA_playerMove.performed += CharacterMove;
        IA_playerMove.canceled += CharacterMove;

        IA_PlayerAttack.Enable();
        IA_PlayerAttack.started += PlayerAttack;
        isCanMove = true;
    }

    private void FixedUpdate()
    {
        if (!isCanMove)
        { 
            rbPlayer.velocity = Vector2.zero;
            return; 
        }

        //Moving player
        Vector2 movePlayer = vectorPlayerMove * playerStats.PlayerSpeed * Time.deltaTime;
        rbPlayer.velocity = movePlayer;
        transform.position += (Vector3)movePlayer;

        GetLastDirection();
    }

    #region Move
    void CharacterMove(InputAction.CallbackContext context)
    {
        vectorPlayerMove = context.ReadValue<Vector2>().normalized;
    }

    void StopMovement(object newMovement)
    {
        bool shouldStop = (bool)newMovement;
        isCanMove = !shouldStop;

        if (isCanMove) 
            animator.EnableAnimator(true);
    }

    #endregion

    #region Attack

    void PlayerAttack(InputAction.CallbackContext context)
    {
        if (!isCanMove) return;
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

    #region Damage

    //Death -------------------------------------------
    void Death()
    {
        Debug.LogWarning("PLAYER DEATH");
    }

    #endregion

}