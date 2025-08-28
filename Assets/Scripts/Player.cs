using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    [Header("Player Settings")]
    [SerializeField] private int maxHealth = 3;

    public InputActionAsset PlayerInput;

    private InputAction inputMovement;
    private InputAction inputFire;

    private Vector2 movementDir;

    private Rigidbody2D rb;
    private Coroutine isFiring = null;

    public int MaxHealth { get => maxHealth; }

    private void OnEnable()
    {
        PlayerInput.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        PlayerInput.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        inputMovement   = InputSystem.actions.FindAction("Move");
        inputFire       = InputSystem.actions.FindAction("Fire");

        rb              = GetComponent<Rigidbody2D>();
        health          = maxHealth;
    }

    private void Update()
    {
        // Movement Direction
        movementDir = inputMovement.ReadValue<Vector2>();

        // Firing Logic
        if (inputFire.IsPressed() && isFiring is null)
            isFiring = StartCoroutine(Fire(false));
        else if (!inputFire.IsPressed() && isFiring is not null)
        {
            StopCoroutine(isFiring);
            isFiring = null;
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

    public override void Damage(int damage)
    {
        base.Damage(damage);
        PlayerGUIManager.Instance.DecreaseHealthBar(damage);

        if (health <= 0)
        {
            // Handle player death (e.g., reload scene, show game over screen, etc.)
            Debug.Log("Player has died.");
            Destroy(gameObject);
        }
    }

    public void Heal(int amount)
    {
        int healAmount = (health + amount) > maxHealth ? (maxHealth - health) : amount;
        
        if (healAmount <= 0)
            return;

        health += healAmount;
        PlayerGUIManager.Instance.IncreaseHealthBar(healAmount);
    }

    private void UpdateProjectile(GameObject newProj)
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject colObj = collision.gameObject;

        if (colObj.TryGetComponent(out Projectile proj) && proj.IsHostileProjectile)
        {
            Damage(proj.Damage);
            Destroy(colObj);
        }
        else if (colObj.TryGetComponent(out Pickup pickup))
        {
            if (pickup is Powerup powerup)
                UpdateProjectile(powerup.RetrieveProjData());
            else
                pickup.ActivatePickup(this);

            Destroy(colObj);
        }
    }
}
