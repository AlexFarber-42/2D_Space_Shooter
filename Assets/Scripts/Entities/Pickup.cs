using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Money,
        Health,
        Charge
    }

    // The amount of time a pickup will remain on the screen before disappearing
    // TODO ---> Remove the serialization after testing for a good duration value
    [SerializeField] private float lingerDuration = 4f;

    [Header("Pickup Settings")]
    [SerializeField] private PickupType pickupType;
    [SerializeField] private int value;

    private float timeActive = 0f;
    protected bool activated = false;

    private void Update()
    {
        TickLingerDuration();
    }

    private void TickLingerDuration()
    {
        // Guard Clause
        if (activated)
            return;

        if (timeActive >= lingerDuration)
            Pools.Instance.RemoveObject(gameObject);
        else
            timeActive += Time.deltaTime;
    }

    public void ActivatePickup(Player playerRef)
    {
        activated = true;

        switch (pickupType)
        {
            case PickupType.Money:
                BankSystem.AddMoney(value);

                ScoreManager.IncreaseScore(value * ScoreManager.MoneyPointScore);
                break;
            case PickupType.Health:
                playerRef.Heal(value);

                ScoreManager.IncreaseScore(value * ScoreManager.HealthPointScore);
                break;
            case PickupType.Charge:
                // TODO ---> Add functionality to the bar underneath the healthbar for special weapons and shieldings
                break;
            default:
                Debug.LogWarning("Pickup type not recognized.");
                break;
        }
    }
}

[System.Serializable]
public struct Drop
{
    public GameObject pickupPrefab;
    public int chance;
}
