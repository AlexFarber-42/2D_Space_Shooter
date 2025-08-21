using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    [Header("Entity Properties")]
    [SerializeField] protected int health = 3;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float fireRate = 2f;
    [SerializeField] protected GameObject projectile;

    public virtual void Damage(int damage)
    {
        health -= damage;
    }

    protected IEnumerator Fire(bool isHostile)
    {
        while (true)
        {
            // TODO ---> Add Pooling
            GameObject projInstance = Instantiate(projectile, transform.position, projectile.transform.rotation);
            Projectile proj = projInstance.GetComponent<Projectile>();

            proj.IsHostileProjectile = isHostile;
            proj.FireProjectile();
            yield return new WaitForSeconds(fireRate);
        }
    }
}
