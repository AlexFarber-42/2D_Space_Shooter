using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    [Header("Player Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Debug/Cheat Settings")]
    [SerializeField] private bool isInvincible = false;
    [SerializeField] private bool infiniteMoney = false;

    public InputActionAsset PlayerInput;

    private InputAction inputMovement;
    private InputAction inputFire;
    private InputAction inputTurn;
    private InputAction inputPause;

    private Vector2 movementDir;
    private int turnState = 0; // -1 is left, 1 is right

    private Rigidbody2D rb;
    private Coroutine isFiring = null;
    private GameObject currentProjectile;

    public int MaxHealth { get => maxHealth; }
    public bool IsDamaged { get => health < MaxHealth; }
    public bool IsDisabled { get => SceneManager.GetActiveScene().buildIndex != 1; } // Not moving or firing if not in the play scene

    private PauseMenu pauseObject;
    private ProgressBar progressBar;

    private void OnEnable()
    {
        PlayerInput.FindActionMap("Player").Enable();
        currentProjectile = startingProj;
        oldFireRate = fireRate;

        if (pauseObject == null)
        {
            pauseObject = FindFirstObjectByType<PauseMenu>();
            progressBar = FindFirstObjectByType<ProgressBar>();

            pauseObject.DeactivatePause();
            isPaused = false;
        }

        if (infiniteMoney)
            BankSystem.AddMoney(999999);
    }

    private void OnDisable()
    {
        PlayerInput.FindActionMap("Player").Disable();
    }

    protected override void Awake()
    {
        inputMovement   = InputSystem.actions.FindAction("Move");
        inputFire       = InputSystem.actions.FindAction("Fire");
        inputTurn       = InputSystem.actions.FindAction("Turn");
        inputPause      = InputSystem.actions.FindAction("Pause");

        rb              = GetComponent<Rigidbody2D>();
        health          = maxHealth;
    }

    private void Start()
    {
        PlayerGUIManager.Instance.UpdatePowerup(currentProjectile);
    }

    private bool turnInc = false;

    private void Update()
    {
        // Player wants to pause/unpause the game
        if (inputPause.triggered)
            PauseToggle();

        if (IsDisabled)
            return;

        // Movement Direction
        movementDir = inputMovement.ReadValue<Vector2>();

        // Turn Direction
        if (inputTurn.IsPressed() && !turnInc)
        {
            turnInc = true;
            PlayerTurn();
        }
        else if (!inputTurn.IsPressed() && turnInc)
            turnInc = false;

        // Firing Logic
        if (inputFire.IsPressed() && isFiring is null)
            isFiring = StartCoroutine(Fire());
        else if (!inputFire.IsPressed() && isFiring is not null)
        {
            StopCoroutine(isFiring);
            isFiring = null;
        }
    }

    private bool isPaused = false;

    private void PauseToggle()
    {
        if (!isPaused)
            PauseGame();
        else
            UnpauseGame();
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        pauseObject.ActivatePause();
        isPaused = true;

        try
        {
            progressBar.GetComponent<DrawerGUIMovement>().TakeOutDrawer();
        }
        catch
        {
            // Skip if not available
        }
    }

    // This method is public for the button to unpause within the main menu of the pause screen
    public void UnpauseGame()
    {
        try
        {
            progressBar.GetComponent<DrawerGUIMovement>().PushInDrawer();
        }
        catch
        {
            // Skip if not available
        }

        pauseObject.DeactivatePause();
        Time.timeScale = 1f;
        isPaused = false;
    }

    private int maxTurnDegree = 20;
    private int degreeShift = 10;
    private int currentShift = 0;

    private void PlayerTurn()
    {
        turnState = (int)inputTurn.ReadValue<float>();

        if (turnState is 0)
            return;
        else
        {
            int shift = turnState is 1 ? currentShift - degreeShift : currentShift + degreeShift;
            currentShift = Mathf.Clamp(shift, -maxTurnDegree, maxTurnDegree);
            transform.rotation = Quaternion.Euler(0f, 0f, currentShift);
        }
    }

    private void FixedUpdate()
    {
        PlayerMovement();
    }

    private void PlayerMovement()
    {
        rb.MovePosition(rb.position + moveSpeed * Time.deltaTime * movementDir);
    }

    protected override IEnumerator Fire()
    {
        while (true)
        {
            // TODO ---> Add Pooling
            Quaternion startingRot = transform.rotation * currentProjectile.transform.rotation;
            GameObject projInstance = Pools.Instance.SpawnObject(Pools.PoolType.Projectile, currentProjectile, transform.position, startingRot);
            Projectile proj = projInstance.GetComponent<Projectile>();

            // TODO ---> Will be unique in modifying the projectile based on the player's upgrades or the projectile itself

            proj.IsHostileProjectile = false;
            proj.FireProjectile(transform.eulerAngles, transform.up);
            ++projs_Fired; // TODO ---> This is different then the other version in Fire as a data collection tool to determine a player's accuracy at the end of a round

            yield return new WaitForSeconds(fireRate);
        }
    }

    public override void Damage(int damage)
    {
        // Cheat for no damage
        if (isInvincible)
            return;

        base.Damage(damage);
        PlayerGUIManager.Instance.DecreaseHealthBar(damage);

        if (health <= 0)
        {
            // Disable the player's controls
            PlayerInput.FindActionMap("Player").Disable();

            // Handle player death (e.g., reload scene, show game over screen, etc.)
            StartCoroutine(KillPlayer());
        }
    }

    private IEnumerator KillPlayer()
    {
        int effectLimitPreBig = 5;
        Vector3 randomRot;

        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        float xDelta = sr.bounds.size.x;
        float yDelta = sr.bounds.size.y;

        Vector3 randomPositioning;

        // Smaller explosions and electrical effects
        for (int i = 0; i < effectLimitPreBig; ++i)
        {
            randomRot = new Vector3(0, 0, Random.Range(0, 360));
            randomPositioning = new Vector3(transform.position.x + Random.Range(-xDelta / 2, xDelta / 2), transform.position.y + Random.Range(-yDelta / 2, yDelta / 2), 0f);
            Instantiate(deathEffects[Random.Range(1, deathEffects.Length)], randomPositioning, Quaternion.Euler(randomRot));
            yield return new WaitForSeconds(.4f);
        }

        // Big explosion in center
        randomRot = new Vector3(0, 0, Random.Range(0, 360));
        GameObject finalExplo = Instantiate(deathEffects[0], transform.position, Quaternion.Euler(randomRot));
        Animator anim = finalExplo.GetComponent<Animator>();
        float delay = anim.runtimeAnimatorController.animationClips.FirstOrDefault().length;
        yield return new WaitForSeconds(delay);
        Debug.Log("Player has died.");
        Destroy(gameObject);
    }

    public void Heal(int amount)
    {
        int healAmount = (health + amount) > maxHealth ? (maxHealth - health) : amount;
        
        if (healAmount <= 0)
            return;

        health += healAmount;
        PlayerGUIManager.Instance.IncreaseHealthBar(healAmount);
    }

    private float oldFireRate;

    public void UpdateProjectile(GameObject newProj)
    {
        currentProjectile = newProj;

        if (newProj.name.Contains("Minigun"))
        {
            oldFireRate = fireRate;
            fireRate = fireRate / 4;
        }
        else
        {
            fireRate = oldFireRate;
        }

        // Modify the GUI 
        PlayerGUIManager.Instance.UpdatePowerup(newProj);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colObj = collision.gameObject;

        if (colObj.TryGetComponent(out Projectile proj) && proj.IsHostileProjectile)
        {
            Damage(proj.Damage);

            if (proj.FullyBrokenThrough)
                proj.EndProjLife();
        }
        else if (colObj.TryGetComponent(out Pickup pickup))
        {
            if (pickup is Powerup powerup)
            {
                ScoreManager.IncreaseScore(ScoreManager.PowerupPointScore);                // Get 100 points when a power up is acquired
                UpdateProjectile(powerup.RetrieveProjData());
            }
            else
                pickup.ActivatePickup(this);

            Pools.Instance.RemoveObject(colObj);
        }
        else if (colObj.TryGetComponent(out Hazard hazard))
        {
            if (hazard.MarkEntity(gameObject))
                Damage(hazard.Damage);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject colObj = collision.gameObject;

        if (colObj.TryGetComponent(out Enemy enemyComp))
        {
            int damageTaken = enemyComp.ContactedPlayer();
            Damage(damageTaken);
            WaveManager.Instance.IncrementKilled();
            Pools.Instance.RemoveObject(colObj);
        }
    }
}
