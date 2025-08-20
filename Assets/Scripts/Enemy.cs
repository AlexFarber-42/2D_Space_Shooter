using UnityEngine;

public class Enemy : Entity
{
    private Rigidbody2D rb;

    public override void Damage(int damage)
    {
        base.Damage(damage);

        if (health <= 0)
        {
            // Handle enemy death (e.g., play animation, point calculations, etc.)
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Projectile proj) && !proj.IsHostileProjectile)
        {
            Damage(proj.Damage);
            Destroy(proj.gameObject);
        }
    }
}
