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

    private void Update()
    {
        lifeTimer += Time.deltaTime;

        if (lifeTimer >= lifeTime)
            Destroy(gameObject);
    }

    public void FireProjectile()
    {
        rb = GetComponent<Rigidbody2D>();

        int dir = hostileProj ? -1 : 1;
        rb.AddForceY(projectileSpeed * dir, ForceMode2D.Impulse);
    }
}
