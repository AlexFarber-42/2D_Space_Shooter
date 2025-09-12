using UnityEngine;

public class Powerup : Pickup
{
    [Header("Powerup Settings")]
    // The new projectile the player has access to when picking up this powerup
    [SerializeField] private GameObject projPrefab;

    public GameObject RetrieveProjData()
    {
        activated = true;

        // TODO ---> Add player modifications for stats?

        return projPrefab;
    }
}
