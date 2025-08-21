using System;
using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Enemy Properties")]
    [SerializeField] private int scoreValue = 1;

    private Transform[] wavePoints;
    private Transform currentPoint;
    private int index;
    private bool beginPathTraversal = false;

    public void SetPath(Transform[] path)
    {
        wavePoints  = path;
        index       = 0;

        currentPoint        = wavePoints[index];
        beginPathTraversal  = true;
        
        if (projectile != null)
            StartCoroutine(Fire(true));
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
            ScoreManager.IncreaseScore(scoreValue);

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
