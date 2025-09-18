using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Properties")]
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 1;

    private float lifeTimer = 0f;
    private Rigidbody2D rb;
    
    private bool hostileProj;
    public bool IsHostileProjectile
    {
        get => hostileProj;
        set => hostileProj = value;
    }

    public int Damage
    {
        get => damage;
    }

    private void Awake()
    {
        // Set rigidbody
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDisable()
    {
        // This resets the life timer when being reused from a pool
        lifeTimer = 0f;
    }

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
            Pools.Instance.RemoveObject(gameObject);
    }

    /// <summary>
    /// General firing method for projectiles that will fire in a set direction based on if the projectile is hostile or not.
    /// 
    /// Very linear/vertical movement
    /// </summary>
    public void FireProjectile()
    {
        int dir = hostileProj ? -1 : 1;

        GetComponent<SpriteRenderer>().flipY = hostileProj;

        rb.AddForceY(projectileSpeed * dir, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Secondary firing method for projectiles that will fire in a specified direction.
    /// 
    /// Can be used for 8 directional firing
    /// </summary>
    public void FireProjectile(Vector3 rotAdjustment, Vector3 dir)
    {
        // Sets the sprite
        transform.eulerAngles += rotAdjustment;

        // Then fires in that direction
        rb.AddForce(projectileSpeed * dir, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Secondary firing method for projectiles that will fire in the direction of a specified Transform.
    /// 
    /// Used for omnidirectional firing
    /// </summary>
    /// <param name="forDir">The transform that is being tracked</param>
    public void FireProjectile(Transform forDir)
    {
        transform.up = forDir.position - transform.position;
        rb.AddForce(projectileSpeed * transform.up, ForceMode2D.Impulse);
    }
}
