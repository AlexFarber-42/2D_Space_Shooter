using UnityEngine;
using System.Collections;

public class Entity : MonoBehaviour
{
    [Header("Entity Properties")]
    [SerializeField] protected int health = 3;
    [SerializeField] protected float moveSpeed = 2f;
    
    [SerializeField]
    [Tooltip("This value represents the LIMIT at which the randomization can go to when determining pause between firing. " +
    "A HIGHER value will lead to higher averages in pauses between firing while a SMALLER value will lead to much more frequent firing")] 
    protected float fireRate = 2f;

    [SerializeField] protected GameObject startingProj;

    [Header("Particle Effect Settings")]
    [SerializeField] protected GameObject[] deathEffects;
    [SerializeField] protected Vector2 scaleOverride = Vector2.one;
    [SerializeField] protected Color colorOverride = Color.white;

    protected int projs_Fired = 0;
    private float minLimit;

    protected virtual void Awake()
    {
        minLimit = moveSpeed > 10f ? .15f : (moveSpeed > 6f ? .4f : .75f);
    }

    public virtual void Damage(int damage)
    {
        health -= damage;
    }

    protected virtual IEnumerator Fire()
    {
        while (true)
        {
            float pauseBetweenFire = UnityEngine.Random.Range(minLimit, fireRate);

            yield return new WaitForSeconds(pauseBetweenFire);

            if (projs_Fired is 0)
                minLimit = fireRate <= .4f ? 0f : .4f;

            GameObject projInstance = Pools.Instance.SpawnObject(Pools.PoolType.Projectile, startingProj, transform.position, startingProj.transform.rotation);
            Projectile proj = projInstance.GetComponent<Projectile>();

            proj.IsHostileProjectile = true;
            proj.FireProjectile();
            ++projs_Fired; // TODO --> To be used as a fun data point for the user to check total projectiles that were fired at them at the end of a round
        }
    }


}
