using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(HealthControl))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBasicStates : MonoBehaviour
{
    [Header("SO - Basic parameters")]
    [SerializeField] public SO_EnemyParameters basicParameters;

    //Components
    [HideInInspector] public bool isCanMove = true;
    [HideInInspector] public HealthControl currentHealt;

    //Private variables - States
    private IEnemyState ChaseState = new ChaseState();
    private IEnemyState AttackState = new AttackState();

    //This instance
    private IEnemyState IcurrentState;

    public NavMeshAgent NMA_agent { get; private set; }
    public GameManager gameManager { get; private set; }
    public Animator enemyAnim { get; private set; }

    //Conditionals
    public bool useFlip = false;
    public bool haveAttackState = true;

    //Public parameters
    [HideInInspector] public Vector3 playerPosition;
    [HideInInspector] public bool canAttack = true; // delay for attack
    private bool coroutineDelayActive = false;

    //private parameters
    private bool facingRight = true;

    //INTERFACE
    public EnemyStates CurrentState => IcurrentState != null ? (EnemyStates)System.Enum.Parse(typeof(EnemyStates),
                                           IcurrentState.GetType().Name.Replace("State", "")) : EnemyStates.Idle;
    public SO_EnemyParameters parameters => basicParameters;

    //Events 
    public Action FinalizeDaley;

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

        ResetNavMeshValues();

        currentHealt.SetMaxHeart = basicParameters.maxHearts;
        currentHealt.OnDeathCharacter += OnDeath;
        currentHealt.OnHealthChanged += OnHealthChanged;
    }

    public void InitEnemy()
    {
        InitVariables();

        SwitchToState(EnemyStates.Chase);
    }

    #endregion

    #region Events of Healt
    public void OnDeath()
    {
        //Add animation death
        this.gameObject.SetActive(false);
    }

    public abstract void OnHealthChanged(int damage);

    public void AttackPlayer()
    {
        EventManager.Instance.TriggerEvent(PlayerEvents.OnReceiveDamage, basicParameters.damage);
    }

    #endregion

    private void Update()
    {
        playerPosition = GameManager.Instance.GetPlayer.transform.position;
        IcurrentState?.UpdateState(this);

        if (useFlip)
            Flip();

        if (canAttack || coroutineDelayActive) return;
        StartCoroutine(CoroutineDelayAttack());
    }

    #region Attack

    IEnumerator CoroutineDelayAttack()
    {
        coroutineDelayActive = true;
        yield return new WaitForSeconds(basicParameters.timeDelay);

        canAttack = true;
        coroutineDelayActive = false;
        FinalizeDaley?.Invoke();
    }

    #endregion

    #region State Machine
    public void SetState(IEnemyState newState)
    {
        IcurrentState?.ExitState(this);
        IcurrentState = newState;
        IcurrentState?.EnterState(this);
    }

    //Add more for new states
    public void SwitchToState(EnemyStates state)
    {
        switch (state)
        {
            case EnemyStates.Idle:
                break;

            case EnemyStates.Attack:
                StartAttack();
                SetState(AttackState);
                break;

            case EnemyStates.Chase:
                SetState(ChaseState);
                break;
        }
    }

    public abstract void StartAttack();
    public abstract void Chase();
    public abstract void Attack();

    #endregion

    #region NavMesh

    public void ResetNavMeshValues()
    {
        NMA_agent.updateRotation = false;
        NMA_agent.updateUpAxis = false;

        NMA_agent.speed = basicParameters.speed;
        NMA_agent.stoppingDistance = basicParameters.attackRange;
    }

    public bool isActivateNavMesh()
    {
        return NMA_agent.isActiveAndEnabled;
    }

    public void switchNavMesh()
    {
        NMA_agent.enabled =! NMA_agent.enabled;
    }

    public void DesactivateNavMesh()
    {
        NMA_agent.enabled = false;
    }

    public void ActivateNavMesh()
    {
        NMA_agent.enabled = true;
    }

    #endregion

    public void ChaseBasicPlayer(Action OnEnterRange)
    {
        if (!NMA_agent.isActiveAndEnabled || !isCanMove) return;

        NMA_agent.SetDestination(playerPosition);
        float distancePlayer = Vector3.Distance(transform.position, playerPosition);

        if (distancePlayer < basicParameters.attackRange)
        {
            OnEnterRange?.Invoke();
        }
    }

    public void Flip()
    {
        bool shouldFlip = (facingRight && transform.position.x < playerPosition.x) ||
                      (!facingRight && transform.position.x > playerPosition.x);

        if (shouldFlip)
        {
            facingRight = !facingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

    }


}