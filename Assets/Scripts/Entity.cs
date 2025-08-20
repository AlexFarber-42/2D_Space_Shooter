using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity Properties")]
    [SerializeField] protected int health = 3;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float fireRate = 2f;

    public virtual void Damage(int damage)
    {
        health -= damage;
    }
}
