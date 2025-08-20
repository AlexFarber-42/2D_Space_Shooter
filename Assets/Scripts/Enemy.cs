using System;
using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Enemy Properties")]
    [Tooltip("A range that is a percentage of the fire rate, with waiting for the full fire rate being at 0 and spontaneous randomness from 0 seconds to the full fire rate at 1")]
    [Range(0f, 1f)]
    [SerializeField] private float fireRateRandomness = 0f;

    private Rigidbody2D rb;
    private Transform[] wavePoints;
    private Transform currentPoint;
    private int index;
    private bool beginPathTraversal = false;

    public void SetPath(Transform[] path)
    {
        wavePoints = path;
        index = 0;
        currentPoint = wavePoints[index];
        beginPathTraversal = true;
    }

    private void FollowPath()
    {
        transform.position = Vector2.MoveTowards(transform.position, currentPoint.position, moveSpeed * Time.deltaTime);
    }

    private void Update()
    {
        if (beginPathTraversal)
        {
            float distance = Vector2.Distance(currentPoint.position, transform.position);

            if (distance > 0 + float.Epsilon)
                FollowPath();
            else if (index < wavePoints.Length)
            {
                ++index;

                try
                {
                    currentPoint = wavePoints[index];
                }
                catch (IndexOutOfRangeException)
                {
                    beginPathTraversal = false;
                    Destroy(gameObject);
                }
            }
            else
            {
                // Reached the end of the path, handle accordingly (e.g., destroy enemy, etc.)
                beginPathTraversal = false;
                Destroy(gameObject);
            }
        }
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        if (health <= 0)
        {
            // Handle enemy death (e.g., play animation, point calculations, etc.)
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Projectile proj) && !proj.IsHostileProjectile)
        {
            Damage(proj.Damage);
            Destroy(proj.gameObject);
        }
    }
}
