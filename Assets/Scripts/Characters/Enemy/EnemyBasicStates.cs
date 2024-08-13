using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(HealthControl))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBasicStates : MonoBehaviour
{
    [Header("SO - Basic parameters")]
    [SerializeField] public SO_EnemyParameters enemyParameters;

    //Components
    [HideInInspector] public bool isCanMove = true;
    [HideInInspector] public HealthControl currentHealt;

    //Private variables - States
    private IEnemyState ChaseState = new ChaseState();
    private IEnemyState AttackState = new AttackState();


    //This instance
    private IEnemyState currentState;

    public NavMeshAgent NMA_agent { get; private set; }
    public GameManager gameManager { get; private set; }
    public Animator enemyAnim { get; private set; }
   


    //INTERFACE
    public EnemyStates currentStateEnum => currentState != null ? (EnemyStates)System.Enum.Parse(typeof(EnemyStates),
                                           currentState.GetType().Name.Replace("State", "")) : EnemyStates.Idle;
    public SO_EnemyParameters parameters => enemyParameters;


    #region Start Enemy
    private void InitVariables()
    {
        gameManager = GameManager.Instance;
        NMA_agent = GetComponent<NavMeshAgent>();
        enemyAnim = GetComponent<Animator>();
        currentHealt = GetComponent<HealthControl>();

        if (currentHealt == null) return;
        if (NMA_agent == null || enemyAnim == null)
        {
            Debug.LogWarning("NO ANIMATOR OR AGENT");
            return;
        }

        NMA_agent.updateRotation = false;
        NMA_agent.updateUpAxis = false;

        NMA_agent.speed = enemyParameters.speed;
        NMA_agent.stoppingDistance = enemyParameters.attackRange;

        currentHealt.SetMaxHeart = enemyParameters.maxHearts;
        currentHealt.OnDeathCharacter += OnDeath;
        currentHealt.OnHealthChanged += OnHealthChanged;

        transform.localScale = Vector3.one;
    }

    public void InitEnemy()
    {
        InitVariables();

        SwitchToState(EnemyStates.Chase);
    }

    #endregion

    #region Events for Healt
    public void OnDeath()
    {
        //Add animation death
        this.gameObject.SetActive(false);
    }

    public abstract void OnHealthChanged(int damage);

    #endregion

    private void Update()
    {
        currentState?.UpdateState(this);
    }

    #region State Machine
    public void SetState(IEnemyState newState)
    {
        Debug.Log($"ENEMY STATE: {newState}");

        currentState?.ExitState(this);
        currentState = newState;
        currentState?.EnterState(this);
    }

    //Add more for new states
    public void SwitchToState(EnemyStates state)
    {
        switch (state)
        {
            case EnemyStates.Idle:
                break;

            case EnemyStates.Attack:
                SetState(AttackState);
                break;

            case EnemyStates.Chase:
                SetState(ChaseState);
                break;
        }
    }

    public abstract void Chase();
    public abstract void Attack();

    #endregion
}