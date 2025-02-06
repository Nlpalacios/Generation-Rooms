using UnityEngine;

public class Enemy_Wasp : EnemyBasicStates
{
    [Header("- Custom Properties -")]
    [SerializeField] private WaspAttackConfig config;
    [SerializeField] private AttackPhase currentPhase;

    //Final positions
    private Vector2 targetPosition = Vector2.zero;
    private float timeInitAttack;

    //Progress for movement
    private float movementProgress = 0f;

    [System.Serializable]
    private class WaspAttackConfig
    {
        [Header("Attack Configuration")]
        public float distanceAttack = 3f;
        public float moveThreshold = 0.1f;

        public float overshootDistance = 3f;
        public float stoppingDistance = 1.5f;

        [Header("Timers")]
        public float timeToBack = 6f;
        public float timeToAttack = 3f;
        public float timeAwaitAttack = 0.2f;
        
        [Header("Layers")]
        public LayerMask wallMask;
    }
    private enum AttackPhase
    {
        None,
        MovingBack,
        Waiting,
        Attacking
    }

    private void Start()
    {
        base.useFlip = true;
        base.canAttack = false;
        base.FinalizeDaley += Restart;
    }
    public void Restart()
    {
        base.ResetNavMeshValues();
        currentPhase = AttackPhase.None;
        movementProgress = 0f;
    }

    public override void StartAttack()
    {
        base.DesactivateNavMesh();
        base.enemyAnim.SetBool("Attack", true);

        movementProgress = 0f;
        targetPosition = CalculateBackPosition();
        currentPhase = AttackPhase.MovingBack;
    }

    private Vector2 CalculateBackPosition()
    {
        Vector2 direction = (transform.position - playerPosition).normalized;
        Vector2 finalPoint = (Vector2)transform.position + direction * config.distanceAttack;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, config.distanceAttack, config.wallMask);
        if (hit.collider != null)
        {
            finalPoint = hit.point;
        }

        return finalPoint;
    }
    private Vector2 CalculateAttackPosition()
    {
        Vector2 directionToPlayer = (playerPosition - transform.position).normalized;
        Vector2 overshootPoint = (Vector2)playerPosition + directionToPlayer * config.overshootDistance;

        RaycastHit2D hit = Physics2D.Raycast(playerPosition, directionToPlayer, config.overshootDistance, config.wallMask);
        if (hit.collider != null)
        {
            return hit.point;
        }

        return overshootPoint;
    }

    public override void Attack()
    {
        switch (currentPhase)
        {
            case AttackPhase.MovingBack:
                HandleMovingBackPhase();
                break;

            case AttackPhase.Waiting:
                HandleWaitingPhase();
                break;

            case AttackPhase.Attacking:
                HandleAttackingPhase();
                break;
        }
    }

    private void HandleMovingBackPhase()
    {
        movementProgress = Mathf.Clamp01(movementProgress + Time.deltaTime / config.timeToBack);
        Vector3 newPosition = Vector3.Lerp(transform.position, targetPosition, movementProgress);
        transform.position = newPosition;

        float distanceToTarget = Vector2.Distance(transform.position, targetPosition);

        if (distanceToTarget < config.moveThreshold || movementProgress >= 0.99f)
        {
            transform.position = targetPosition; 
            movementProgress = 0f; 
            StartWaitingPhase();
        }
    }

    private void StartWaitingPhase()
    {
        timeInitAttack = Time.time + config.timeAwaitAttack;

        targetPosition = CalculateAttackPosition();
        currentPhase = AttackPhase.Waiting;
    }

    private void HandleWaitingPhase()
    {
        if (Time.time >= timeInitAttack)
        {
            StartAttackingPhase();
        }
    }

    private void StartAttackingPhase()
    {
        currentPhase = AttackPhase.Attacking;
        movementProgress = 0f;
    }

    private void HandleAttackingPhase()
    {
        movementProgress = Mathf.Clamp01(movementProgress + Time.deltaTime / config.timeToAttack);
        transform.position = Vector3.Lerp(transform.position, targetPosition, movementProgress);

        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            CompletedAttack();
        }

    }

    private void CompletedAttack()
    {
        base.canAttack = false;
        movementProgress = 0f;

        ActivateNavMesh();
        NMA_agent.stoppingDistance = config.stoppingDistance;
        enemyAnim.SetBool("Attack", false);
        SwitchToState(EnemyStates.Chase);
        currentPhase = AttackPhase.None;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CurrentState != EnemyStates.Attack || currentPhase != AttackPhase.Attacking)
            return;

        if (collision != null && collision.gameObject.TryGetComponent(out Player player))
        {
            base.AttackPlayer();
            CompletedAttack();
        }
    }

    public override void Chase() {}
    public override void OnHealthChanged(int damage) {}
    public override void ResetEnemy(){}
}
