using System;
using System.Collections;
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

    private GameObject hazardObj;

    private WaveManager wm;

    public int GetScoreValue() => scoreValue;

    protected override void Awake()
    {
        base.Awake();

        wm = WaveManager.Instance;

        // Retrieves the hazard object that activates upon detonation or elimination
        if (explodesOnContact)
            hazardObj = transform.GetChild(0).gameObject;
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
        Vector3 vecBetween = currentPoint.position - transform.position;
        transform.SetPositionAndRotation(Vector2.MoveTowards(transform.position, currentPoint.position, moveSpeed * Time.deltaTime), 
                                         Quaternion.LookRotation(Vector3.forward, moveSpeed * Time.deltaTime * -vecBetween));
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
                    wm.IncrementLeft();
                    Destroy(gameObject);
                }
            }
            else
            {
                // Reached the end of the path, handle accordingly (e.g., destroy enemy, etc.)
                beginPathTraversal = false;
                wm.IncrementLeft();
                Destroy(gameObject);
            }
        }
    }

    public override void Damage(int damage)
    {
        base.Damage(damage);

        if (health <= 0)
        {
            try
            {
                Vector3 randomRot = new Vector3(0f, 0f, UnityEngine.Random.Range(1f, 360f));
                GameObject deathEffect = Instantiate(deathEffects[UnityEngine.Random.Range(0, deathEffects.Length)], transform.position, Quaternion.Euler(randomRot));

                if (scaleOverride != Vector2.one)
                    deathEffect.transform.localScale = scaleOverride;
                if (colorOverride != Color.white)
                {
                    bool coinFlip = UnityEngine.Random.Range(1, 3) is 1;

                    if (coinFlip)
                        deathEffect.GetComponent<SpriteRenderer>().color = colorOverride;
                }
            }
            catch
            {
                Debug.Log($"Bypassing death effect for {name}");
            }

            // If a hazardous object is detected from this entities corpse, activate it (i.e. boomer flis exploding regardless of contact or not)
            if (hazardObj != null)
                TriggerHazard();

            if (TryGetComponent(out Spawner spawner) && spawner.SpawnsRemaining > 0)
            {
                for (int i = 0; i < spawner.SpawnsRemaining; ++i)
                {
                    wm.IncrementKilled();
                    ScoreManager.IncreaseScore(spawner.EnemyScoreCheck);
                }
            }

            ScoreManager.IncreaseScore(scoreValue);
            TryDropItem();
            wm.IncrementKilled();

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
        GameObject colObj = collision.gameObject;

        if (colObj.TryGetComponent(out Projectile proj) && !proj.IsHostileProjectile)
        {
            Damage(proj.Damage);
            Destroy(proj.gameObject);
        }
        else if (colObj.TryGetComponent(out Hazard hazard))
        {
            if (hazard.MarkEntity(gameObject))
                Damage(hazard.Damage);
        }
    }

    public int ContactedPlayer()
    {
        if (hazardObj != null)
        {
            contactDmg++;
            TriggerHazard();
        }

        return contactDmg;
    }

    private void TriggerHazard()
    {
        // First restructure the hierarchy to be the parent of this object while retaining all prior positions
        hazardObj.transform.SetParent(transform.parent, true);
        transform.SetParent(hazardObj.transform, true);

        hazardObj.SetActive(true);
        hazardObj.GetComponent<Hazard>().CreateHazard();
    }
}
