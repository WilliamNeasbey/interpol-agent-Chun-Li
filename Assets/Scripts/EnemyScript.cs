using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class EnemyScript : MonoBehaviour
{
    //Declarations
    private Animator animator;
    private CombatScript playerCombat;
    private EnemyManager enemyManager;
    private EnemyDetection enemyDetection;
    private CharacterController characterController;
    public Collider enemyHitbox; // Reference to the enemy's hitbox collider

    [Header("Stats")]
    public int health = 3;
    private float moveSpeed = 1;
    private Vector3 moveDirection;

    [Header("States")]
    [SerializeField] private bool isPreparingAttack;
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isRetreating;
    [SerializeField] private bool isLockedTarget;
    [SerializeField] private bool isStunned;
    [SerializeField] private bool isWaiting = true;

    [Header("Polish")]
    [SerializeField] private ParticleSystem counterParticle;

    private Coroutine PrepareAttackCoroutine;
    private Coroutine RetreatCoroutine;
    private Coroutine DamageCoroutine;
    private Coroutine MovementCoroutine;

    //Events
    public UnityEvent<EnemyScript> OnDamage;
    public UnityEvent<EnemyScript> OnStopMoving;
    public UnityEvent<EnemyScript> OnRetreat;

    private CombatScript combatScriptReference; // Reference to the CombatScript component

    void Start()
    {
        enemyManager = GetComponentInParent<EnemyManager>();

        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        playerCombat = FindObjectOfType<CombatScript>();
        enemyDetection = playerCombat.GetComponentInChildren<EnemyDetection>();

        playerCombat.OnHit.AddListener((x) => OnPlayerHit(x));
        playerCombat.OnCounterAttack.AddListener((x) => OnPlayerCounter(x));
        playerCombat.OnTrajectory.AddListener((x) => OnPlayerTrajectory(x));

        MovementCoroutine = StartCoroutine(EnemyMovement());

        counterParticle.Clear();
        counterParticle.Stop();
        AssignCombatScriptReference(playerCombat);
    }
    public void AssignCombatScriptReference(CombatScript combatScript)
    {
        combatScriptReference = combatScript;
    }
    IEnumerator EnemyMovement()
    {
        //Waits until the enemy is not assigned to no action like attacking or retreating
        yield return new WaitUntil(() => isWaiting == true);

        int randomChance = Random.Range(0, 2);

        if (randomChance == 1)
        {
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            isMoving = true;
        }
        else
        {
            StopMoving();
        }

        yield return new WaitForSeconds(1);

        MovementCoroutine = StartCoroutine(EnemyMovement());
    }

    void Update()
    {
        //Constantly look at player
        transform.LookAt(new Vector3(playerCombat.transform.position.x, transform.position.y, playerCombat.transform.position.z));

        //Only moves if the direction is set
        MoveEnemy(moveDirection);

    }

    //Listened event from Player Animation
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHitCollider")) // Modify the tag as needed
        {
            // Call the OnPlayerHit method directly, passing the current enemy as the target
            OnPlayerHit(this);
        }
    }

    private void OnPlayerHit(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            DamageCoroutine = StartCoroutine(HitCoroutine());

            enemyDetection.SetCurrentTarget(null);
            isLockedTarget = false;
            OnDamage.Invoke(this);

            health--;

            if (health <= 0)
            {
                Death();
                return;
            }

            animator.SetTrigger("Hit");
            transform.DOMove(transform.position - (transform.forward / 2), .3f).SetDelay(.1f);

            StopMoving();
        }
    }

    IEnumerator HitCoroutine()
    {
        isStunned = true;
        yield return new WaitForSeconds(.5f);
        isStunned = false;
        
        if (combatScriptReference != null)
        {
            combatScriptReference.hitCounterLightningKicks++; // Increment the lightning kicks hit count
            combatScriptReference.hitCount++; // Increment the normal hit count

            int hitCount = combatScriptReference.hitCount;
            int hitCounterLightningKicks = combatScriptReference.hitCounterLightningKicks;

            // Do something with the hit counts
            Debug.Log("Normal Hit Count: " + hitCount);
            Debug.Log("Lightning Kicks Hit Count: " + hitCounterLightningKicks);
        }
        else
        {
            Debug.LogWarning("CombatScript reference is not assigned.");
        }
    }

    void OnPlayerCounter(EnemyScript target)
    {
        if (target == this)
        {
            PrepareAttack(false);
        }
    }

    void OnPlayerTrajectory(EnemyScript target)
    {
        if (target == this)
        {
            StopEnemyCoroutines();
            isLockedTarget = true;
            PrepareAttack(false);
            StopMoving();
        }
    }

    void Death()
    {
        StopEnemyCoroutines();

        this.enabled = false;
        characterController.enabled = false;
        animator.SetTrigger("Death");
        enemyManager.SetEnemyAvailiability(this, false);
        DisableHitbox();
    }

    public void SetRetreat()
    {
        StopEnemyCoroutines();

        RetreatCoroutine = StartCoroutine(PrepRetreat());

        IEnumerator PrepRetreat()
        {
            yield return new WaitForSeconds(1.4f);
            OnRetreat.Invoke(this);
            isRetreating = true;
            moveDirection = -Vector3.forward;
            isMoving = true;
            yield return new WaitUntil(() => Vector3.Distance(transform.position, playerCombat.transform.position) > 4);
            isRetreating = false;
            StopMoving();

            //Free 
            isWaiting = true;
            MovementCoroutine = StartCoroutine(EnemyMovement());
        }
    }

    public void SetAttack()
    {
        isWaiting = false;

        PrepareAttackCoroutine = StartCoroutine(PrepAttack());

        IEnumerator PrepAttack()
        {
            PrepareAttack(true);
            yield return new WaitForSeconds(.2f);
            moveDirection = Vector3.forward;
            isMoving = true;
        }
    }


    void PrepareAttack(bool active)
    {
        isPreparingAttack = active;

        if (active)
        {
            counterParticle.Play();
        }
        else
        {
            StopMoving();
            counterParticle.Clear();
            counterParticle.Stop();
        }
    }

    void MoveEnemy(Vector3 direction)
    {
        //Set movespeed based on direction
        moveSpeed = 1;

        if (direction == Vector3.forward)
            moveSpeed = 5;
        if (direction == -Vector3.forward)
            moveSpeed = 2;

        //Set Animator values
        animator.SetFloat("InputMagnitude", (characterController.velocity.normalized.magnitude * direction.z) / (5 / moveSpeed), .2f, Time.deltaTime);
        animator.SetBool("Strafe", (direction == Vector3.right || direction == Vector3.left));
        animator.SetFloat("StrafeDirection", direction.normalized.x, .2f, Time.deltaTime);

        //Don't do anything if isMoving is false
        if (!isMoving)
            return;

        Vector3 dir = (playerCombat.transform.position - transform.position).normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir; //Vector perpendicular to direction
        Vector3 movedir = Vector3.zero;

        Vector3 finalDirection = Vector3.zero;

        if (direction == Vector3.forward)
            finalDirection = dir;
        if (direction == Vector3.right || direction == Vector3.left)
            finalDirection = (pDir * direction.normalized.x);
        if (direction == -Vector3.forward)
            finalDirection = -transform.forward;

        if (direction == Vector3.right || direction == Vector3.left)
            moveSpeed /= 1.5f;

        movedir += finalDirection * moveSpeed * Time.deltaTime;

        characterController.Move(movedir);

        if (!isPreparingAttack)
            return;

        if (Vector3.Distance(transform.position, playerCombat.transform.position) < 2)
        {
            StopMoving();
            if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
                Attack();
            else
                PrepareAttack(false);
        }
    }


    private void Attack()
    {
        // Enable the enemy's hitbox collider during the attack animation
        enemyHitbox.enabled = true;

        transform.DOMove(transform.position + (transform.forward / 1), .5f);
        animator.SetTrigger("Punch");
    }

    // This function is called at the end of the attack animation to disable the hitbox collider
    private void DisableHitbox()
    {
        enemyHitbox.enabled = false;
    }
    
    // This function is called whenever the enemy's state changes (e.g., animation changes)
    private void OnStateChanged(AnimatorStateInfo stateInfo)
    {
        // If the enemy's state is not the attack animation, disable the hitbox collider
        if (!stateInfo.IsName("Punch"))
        {
            enemyHitbox.enabled = false;
        }
    }

    public void HitEvent()
    {
        if (!playerCombat.isCountering && !playerCombat.isAttackingEnemy)
            playerCombat.DamageEvent();

        PrepareAttack(false);
    }

    public void StopMoving()
    {
        isMoving = false;
        moveDirection = Vector3.zero;
        if (characterController.enabled)
            characterController.Move(moveDirection);
    }

    void StopEnemyCoroutines()
    {
        PrepareAttack(false);

        if (isRetreating)
        {
            if (RetreatCoroutine != null)
                StopCoroutine(RetreatCoroutine);
        }

        if (PrepareAttackCoroutine != null)
            StopCoroutine(PrepareAttackCoroutine);

        if (DamageCoroutine != null)
            StopCoroutine(DamageCoroutine);

        if (MovementCoroutine != null)
            StopCoroutine(MovementCoroutine);
    }

    public void MoveTowards(Vector3 targetPosition, float pullForce)
    {
        Vector3 moveDirection = (targetPosition - transform.position).normalized * pullForce * Time.deltaTime;
        characterController.Move(moveDirection);
    }


    #region Public Booleans

    public bool IsAttackable()
    {
        return health > 0;
    }

    public bool IsPreparingAttack()
    {
        return isPreparingAttack;
    }

    public bool IsRetreating()
    {
        return isRetreating;
    }

    public bool IsLockedTarget()
    {
        return isLockedTarget;
    }

    public bool IsStunned()
    {
        return isStunned;
    }

    #endregion
}
