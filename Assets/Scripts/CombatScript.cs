using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class CombatScript : MonoBehaviour
{
    private EnemyManager enemyManager;
    private EnemyDetection enemyDetection;
    private MovementInput movementInput;
    private Animator animator;
    private CinemachineImpulseSource impulseSource;
    public AudioSource hitSound;
    public AudioSource painSound;
    public AudioSource SpinningbirdkickSound;
    public AudioSource LightningkickSound;
    private InputAction spinningBirdKickAction;
    private InputAction lightningKicksAction; // New InputAction for Lightning Kicks
    private InputAction kamehamehaInputAction; // New InputAction for Lightning Kicks


    [Header("Target")]
    private EnemyScript lockedTarget;

    [Header("Combat Settings")]
    [SerializeField] private float attackCooldown;

    [Header("States")]
    public bool isAttackingEnemy = false;
    public bool isCountering = false;

    [Header("Public References")]
    [SerializeField] private Transform kickPosition;
    [SerializeField] private ParticleSystemScript kickParticle;
    [SerializeField] private GameObject lastHitCamera;
    [SerializeField] private Transform lastHitFocusObject;
    [SerializeField] private GameObject kamehamehaPrefab;

    //Coroutines
    private Coroutine counterCoroutine;
    private Coroutine attackCoroutine;
    private Coroutine damageCoroutine;

    [Space]

    //Events
    public UnityEvent<EnemyScript> OnTrajectory;
    public UnityEvent<EnemyScript> OnHit;
    public UnityEvent<EnemyScript> OnCounterAttack;

    int animationCount = 0;
    string[] attacks;

    [Header("Health Settings")]
    public int maxHealth = 100;
    private int currentHealth;
    private bool isDead = false;

    [Header("UI Elements")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI hitCountText; // Existing hit count UI
    public TextMeshProUGUI lightningKicksHitCountText; // New hit count UI for Lightning Kicks
    public GameObject deathAnimation;
    public GameObject gameOverUI;

    [Header("Movement Disable")]
    public GameObject DisableMovement;

    //hit counters
    public int hitCount = 0;
    public int hitCounterLightningKicks = 0; // New hit counter for Lightning Kicks
    private int lightningKicksHitRequirement = 44; // New hit requirement for Lightning Kicks



    void Start()
    {
        enemyManager = FindObjectOfType<EnemyManager>();
        animator = GetComponent<Animator>();
        enemyDetection = GetComponentInChildren<EnemyDetection>();
        movementInput = GetComponent<MovementInput>();
        impulseSource = GetComponentInChildren<CinemachineImpulseSource>();
        currentHealth = maxHealth;
        UpdateHealthUI();

        // Get a reference to the Spinning Bird Kick input action
        spinningBirdKickAction = new InputAction("SpinningBirdKick", InputActionType.Button, "<Keyboard>/e");
        spinningBirdKickAction.Enable();

        // Get a reference to the Lightning Kicks input action
        lightningKicksAction = new InputAction("LightningKicks", InputActionType.Button, "<Keyboard>/q");
        lightningKicksAction.Enable();

        kamehamehaInputAction = new InputAction("Kamehameha", InputActionType.Button, "<Keyboard>/x");
        kamehamehaInputAction.Enable();
    }

    void Update()
    {
        LightningKicksInput(); 

        SpinningBirdKickInput(); 
        KamehamehaInput(); 

    }

    // This function gets called whenever the player receives damage

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemyhitbox")) 
        {
            TakeDamage(10); // Assuming the player takes 10 damage on getting hit
            DamageEvent();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0);
        UpdateHealthUI();

        // Play the hit sound
        if (hitSound != null)
            hitSound.Play();

        if (painSound != null)
            painSound.Play();

        // Trigger the death animation if the player's health reaches zero
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            UpdateHealthUI();
            animator.SetBool("IsDead", true);
            isDead = true;
            // Disable the object instead of disabling player movement
            if (deathAnimation != null)
                deathAnimation.SetActive(false);

            // Enable the game over UI
            if (gameOverUI != null)
                gameOverUI.SetActive(true);

            // Wait for 3 seconds before loading a new scene
            StartCoroutine(LoadNewSceneAfterDelay(3f));
        }
    }

    IEnumerator LoadNewSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Load a new scene 
        SceneManager.LoadScene("LoseScreen");
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = "Health: " + currentHealth.ToString();
    }

    //This function gets called whenever the player inputs the kick action
    void AttackCheck()
    {
        if (isAttackingEnemy)
            return;

        //Check to see if the detection behavior has an enemy set
        if (enemyDetection.CurrentTarget() == null)
        {
            if (enemyManager.AliveEnemyCount() == 0)
            {
                Attack(null, 0);
                return;
            }
            else
            {
                lockedTarget = enemyManager.RandomEnemy();
            }
        }

        //If the player is moving the movement input, use the "directional" detection to determine the enemy
        if (enemyDetection.InputMagnitude() > .2f)
            lockedTarget = enemyDetection.CurrentTarget();

        //Extra check to see if the locked target was set
        if (lockedTarget == null)
            lockedTarget = enemyManager.RandomEnemy();

        //AttackTarget
        Attack(lockedTarget, TargetDistance(lockedTarget));
    }

    // Function to update the UI to display the number of hits until the new attack is available
    void UpdateHitCountUI()
    {
        if (hitCountText != null)
            hitCountText.text =  Mathf.Max(20 - hitCount, 0).ToString(); // Update normal hits UI

        if (lightningKicksHitCountText != null)
            lightningKicksHitCountText.text =  Mathf.Max(lightningKicksHitRequirement - hitCounterLightningKicks, 0).ToString(); // Update Lightning Kicks UI
    }

    public void Attack(EnemyScript target, float distance)
    {
        //Types of attack animation
        attacks = new string[] { "AirKick", "AirKick2", "AirKick3", "AirKick4", "AirKick5" };

        //Attack nothing in case target is null
        if (target == null)
        {
            AttackType("Groundkick", .2f, null, 0);
            return;
        }

        if (distance < 15)
        {
            animationCount = (int)Mathf.Repeat((float)animationCount + 1, (float)attacks.Length);
            string attackString = isLastHit() ? attacks[Random.Range(0, attacks.Length)] : attacks[animationCount];
            AttackType(attackString, attackCooldown, target, .65f);
        }
        else
        {
            lockedTarget = null;
            AttackType("GroundKick", .2f, null, 0);
        }

        //Change impulse
        impulseSource.m_ImpulseDefinition.m_AmplitudeGain = Mathf.Max(3, 1 * distance);

    }

    void AttackType(string attackTrigger, float cooldown, EnemyScript target, float movementDuration)
    {
        animator.SetTrigger(attackTrigger);

        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);
        attackCoroutine = StartCoroutine(AttackCoroutine(isLastHit() ? 1.5f : cooldown));

        //Check if last enemy
        if (isLastHit())
            StartCoroutine(FinalBlowCoroutine());

        if (target == null)
            return;

        target.StopMoving();
        MoveTorwardsTarget(target, movementDuration);

        IEnumerator AttackCoroutine(float duration)
        {
            movementInput.acceleration = 0;
            isAttackingEnemy = true;
            movementInput.enabled = false;
            yield return new WaitForSeconds(duration);
            isAttackingEnemy = false;
            yield return new WaitForSeconds(.2f);
            movementInput.enabled = true;
            LerpCharacterAcceleration();
        }

        IEnumerator FinalBlowCoroutine()
        {
            Time.timeScale = .5f;
            lastHitCamera.SetActive(true);
            lastHitFocusObject.position = lockedTarget.transform.position;
            yield return new WaitForSecondsRealtime(2);
            lastHitCamera.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    void MoveTorwardsTarget(EnemyScript target, float duration)
    {
        OnTrajectory.Invoke(target);
        transform.DOLookAt(target.transform.position, .2f);
        transform.DOMove(TargetOffset(target.transform), duration);
    }

    void CounterCheck()
    {
        //Initial check
        if (isCountering || isAttackingEnemy || !enemyManager.AnEnemyIsPreparingAttack())
            return;

        lockedTarget = ClosestCounterEnemy();
        OnCounterAttack.Invoke(lockedTarget);

        if (TargetDistance(lockedTarget) > 2)
        {
            Attack(lockedTarget, TargetDistance(lockedTarget));
            return;
        }

        float duration = .2f;
        animator.SetTrigger("Dodge");
        transform.DOLookAt(lockedTarget.transform.position, .2f);
        transform.DOMove(transform.position + lockedTarget.transform.forward, duration);

        if (counterCoroutine != null)
            StopCoroutine(counterCoroutine);
        counterCoroutine = StartCoroutine(CounterCoroutine(duration));

        IEnumerator CounterCoroutine(float duration)
        {
            isCountering = true;
            movementInput.enabled = false;
            yield return new WaitForSeconds(duration);
            Attack(lockedTarget, TargetDistance(lockedTarget));
            isCountering = false;

        }
    }

    void SpinningBirdKickCheck()
    {
        if (isAttackingEnemy)
            return;

        // Check if the player has hit enemies 10 times
        if (hitCount >= 20)
        {
            // Call the new attack function
            SpinningBirdKick();
        }
        else
        {
            // Normal attack logic
            // ... (existing code for regular attacks)
        }
    }


    private void SpinningBirdKickInput()
    {
        if (!isDead && spinningBirdKickAction.triggered)
        {
            Debug.Log("Spinning Bird Kick input detected!");
            // Check if the player has hit enemies 10 times
            if (hitCount >= 20)
            {
                // Call the new attack function
                SpinningBirdKick();
            }
            else
            {
                // If the player hasn't hit enough times, you can play a sound or display a message indicating the requirement.
                Debug.Log("Need 10 hits before using Spinning Bird Kick!");
            }
        }
    }

    public void SpinningBirdKick()
    {
        // Adjust the values as needed
        float spinningBirdKickRadius = 5f;
        int spinningBirdKickDamage = 20;
        float pullForce = 20f;

        // Get all enemies within the spinningBirdKickRadius
        EnemyScript[] enemiesInRadius = enemyManager.GetEnemiesInRadius(transform.position, spinningBirdKickRadius);

        // Play the spinning bird kick animation
        animator.SetTrigger("SpinningBirdKick");
       
        // Play the  sound
        if (SpinningbirdkickSound != null)
            SpinningbirdkickSound.Play();
        foreach (EnemyScript enemy in enemiesInRadius)
        {
            if (!enemy.gameObject.activeInHierarchy)
                continue;

            // Reduce enemy's health
            enemy.health -= spinningBirdKickDamage;
            if (enemy.health <= 0)
            {
                // Enemy is dead, handle the death here
                // For example, play death animation, spawn particles, etc.
                // Then, disable the enemy object
                //enemy.gameObject.SetActive(false);
            }
            else
            {
                // Calculate the direction from the enemy to the player
                Vector3 directionToPlayer = (transform.position - enemy.transform.position).normalized;

                // Apply a force to pull the enemy towards the player
                enemy.MoveTowards(transform.position, pullForce);
            }
        }

        // Reset the hit count and update the UI
        hitCount = 0;
        UpdateHitCountUI();
    }

    private void LightningKicksInput()
    {
        if (!isDead && lightningKicksAction.triggered)
        {
            Debug.Log("Lightning Kicks input detected!");
            // Check if the player has hit enemies enough times to use Lightning Kicks
            if (hitCounterLightningKicks >= lightningKicksHitRequirement)
            {
                // Call the new attack function
                LightningKicks();
            }
            else
            {
                // If the player hasn't hit enough times, you can play a sound or display a message indicating the requirement.
                Debug.Log("Need " + lightningKicksHitRequirement + " hits before using Lightning Kicks!");
            }
        }
    }

    public void LightningKicks()
    {
        // Play the lightning kicks animation
        animator.SetTrigger("LightningKicks");

        // Play the sound
        if (LightningkickSound != null)
            LightningkickSound.Play();

        // Reset the hit count for Lightning Kicks and update the UI
        hitCounterLightningKicks = 0;
        UpdateHitCountUI();
    }

    private void KamehamehaInput()
    {
        if (!isDead && kamehamehaInputAction.triggered)
        {
            Debug.Log("Kamehameha input detected!");
            // Call the new attack function
            Kamehameha();
        }
    }

    public void Kamehameha()
    {
        // Check if the player is attacking an enemy before performing the kamehameha
        if (isAttackingEnemy)
            return;

        // Play the Kamehameha animation
        animator.SetTrigger("Kamehameha");

        // Wait for a short delay to synchronize with the animation
        StartCoroutine(PerformKamehamehaWithDelay());
    }

    private IEnumerator PerformKamehamehaWithDelay()
    {
        yield return new WaitForSeconds(0.2f); // Adjust the delay as needed

        // Instantiate the kamehameha prefab at the kick position with the same rotation as the player
        GameObject kamehamehaObject = Instantiate(kamehamehaPrefab, kickPosition.position, transform.rotation);
    }

    IEnumerator MoveEnemyOverTime(EnemyScript enemy, Vector3 moveVector)
    {
        float moveDuration = 0.5f; // Adjust the duration as needed
        float elapsedTime = 0f;
        Vector3 startingPosition = enemy.transform.position;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / moveDuration);

            // Move the enemy using a smooth interpolation
            enemy.transform.position = Vector3.Lerp(startingPosition, startingPosition + moveVector, t);

            yield return null;
        }
    }


    float TargetDistance(EnemyScript target)
    {
        return Vector3.Distance(transform.position, target.transform.position);
    }

    public Vector3 TargetOffset(Transform target)
    {
        Vector3 position;
        position = target.position;
        return Vector3.MoveTowards(position, transform.position, .95f);
    }

    public void HitEvent()
    {
        if (lockedTarget == null || enemyManager.AliveEnemyCount() == 0)
            return;

        OnHit.Invoke(lockedTarget);

        //Particle
        kickParticle.PlayParticleAtPosition(kickPosition.position);
        hitSound.Play();
        // Increment the hit count
        hitCount++;

        // Update the UI to display the remaining hits until the new attack is available
        UpdateHitCountUI();

        // Increment the hit count for Lightning Kicks
        hitCounterLightningKicks++;

        // Update the UI to display the remaining hits until the new attack is available
        UpdateHitCountUI();

        // Check if the player pressed the input for Lightning Kicks
        if (!isDead && lightningKicksAction.triggered)
        {
            Debug.Log("Lightning Kicks input detected!");

            // Check if the player has hit enemies enough times to use Lightning Kicks
            if (hitCounterLightningKicks >= lightningKicksHitRequirement)
            {
                // Call the new attack function
                LightningKicks();
            }
            else
            {
                // If the player hasn't hit enough times, you can play a sound or display a message indicating the requirement.
                Debug.Log("Need " + lightningKicksHitRequirement + " hits before using Lightning Kicks!");
            }
        }
    }

    public void DamageEvent()
    {
        if (currentHealth > 0)
        {
            animator.SetTrigger("Hit");

            // Reduce player health by the specified damage amount
            TakeDamage(10); // Assuming the player takes 10 damage on getting hit
        }
        animator.SetTrigger("Hit");

        if (damageCoroutine != null)
            StopCoroutine(damageCoroutine);
        damageCoroutine = StartCoroutine(DamageCoroutine());

        IEnumerator DamageCoroutine()
        {
            movementInput.enabled = false;
            yield return new WaitForSeconds(.5f);
            movementInput.enabled = true;
            LerpCharacterAcceleration();
        }
        
    }

    EnemyScript ClosestCounterEnemy()
    {
        float minDistance = 100;
        int finalIndex = 0;

        for (int i = 0; i < enemyManager.allEnemies.Length; i++)
        {
            EnemyScript enemy = enemyManager.allEnemies[i].enemyScript;

            if (enemy.IsPreparingAttack())
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) < minDistance)
                {
                    minDistance = Vector3.Distance(transform.position, enemy.transform.position);
                    finalIndex = i;
                }
            }
        }

        return enemyManager.allEnemies[finalIndex].enemyScript;

    }

    void LerpCharacterAcceleration()
    {
        movementInput.acceleration = 0;
        DOVirtual.Float(0, 1, .6f, ((acceleration) => movementInput.acceleration = acceleration));
    }

    bool isLastHit()
    {
        if (lockedTarget == null)
            return false;

        return enemyManager.AliveEnemyCount() == 1 && lockedTarget.health <= 1;
    }

    #region Input

    private void OnCounter()
    {
        // Only check for counter if the player is not dead
        if (!isDead)
            CounterCheck();
    }

    private void OnAttack()
    {
        // Only check for attack if the player is not dead
        if (!isDead)
            AttackCheck();
    }

    #endregion

}
