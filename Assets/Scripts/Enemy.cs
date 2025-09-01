using System;
using UnityEngine;

public class Enemy : Entity
{
    [Header("Enemy Properties")]
    [SerializeField] private int scoreValue = 1;
    [SerializeField] private Drop[] drops;
    [SerializeField] private int contactDmg = 1;
    [SerializeField] private bool explodesOnContact = false;

    private Transform[] wavePoints;
    private Transform currentPoint;
    private int index;
    private bool beginPathTraversal = false;

    // TODO ---> Create a different approach that includes exploders
    public int ContactDamage
    {
        get => contactDmg;
    }

    public void SetPath(Transform[] path)
    {
        wavePoints  = path;
        index       = 0;

        currentPoint        = wavePoints[index];
        beginPathTraversal  = true;
        
        if (startingProj != null)
            StartCoroutine(Fire());
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

            if (distance > 0.01 + float.Epsilon)
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
            TryDropItem();

            // TODO ---> Handle enemy death via effect like an explosion of bug guts
            Destroy(gameObject);
        }
    }

    private void TryDropItem()
    {
        foreach (Drop drop in drops)
        {
            int roll = UnityEngine.Random.Range(1, 101);

            // Drop the item
            if (roll <= drop.chance)
            {
                Instantiate(drop.pickupPrefab, transform.position, Quaternion.identity);
                break;
            }
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
