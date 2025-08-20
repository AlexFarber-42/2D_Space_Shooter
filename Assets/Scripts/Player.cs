using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Entity
{
    public GameObject projPrefabTest;

    public InputActionAsset PlayerInput;

    private InputAction inputMovement;
    private InputAction inputFire;

    private Vector2 movementDir;

    private Rigidbody2D rb;
    private Coroutine isFiring = null;

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
    }

    private void Update()
    {
        // Movement Direction
        movementDir = inputMovement.ReadValue<Vector2>();

        // Firing Logic
        if (inputFire.IsPressed() && isFiring is null)
            isFiring = StartCoroutine(Fire());
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

    private IEnumerator Fire()
    {
        while (true)
        {
            // TODO ---> Add Pooling
            GameObject projInstance = Instantiate(projPrefabTest, transform.position, Quaternion.identity);
            Projectile proj = projInstance.GetComponent<Projectile>();

            proj.IsHostileProjectile = false;
            proj.FireProjectile();
            yield return new WaitForSeconds(fireRate);
        }
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        if (health <= 0)
        {
            // Handle player death (e.g., reload scene, show game over screen, etc.)
            Debug.Log("Player has died.");
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Projectile proj) && proj.IsHostileProjectile)
        {
            Damage(proj.Damage);
            Destroy(proj.gameObject);
        }
    }
}
