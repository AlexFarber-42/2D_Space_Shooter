using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    [Header("Entity Properties")]
    [SerializeField] protected int health = 3;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float fireRate = 2f;

    [SerializeField] protected GameObject startingProj;

    public virtual void Damage(int damage)
    {
        health -= damage;
    }

    protected IEnumerator Fire(bool isHostile)
    {
        while (true)
        {
            // TODO ---> Add Pooling
            GameObject projInstance = Instantiate(startingProj, transform.position, startingProj.transform.rotation);
            Projectile proj = projInstance.GetComponent<Projectile>();

            proj.IsHostileProjectile = isHostile;
            proj.FireProjectile();
            yield return new WaitForSeconds(fireRate);
        }
    }
}
