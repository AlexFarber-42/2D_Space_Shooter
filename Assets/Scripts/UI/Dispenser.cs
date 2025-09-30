using UnityEngine;

public class Dispenser : MonoBehaviour
{
    public enum DispenserType
    {
        Health   = 0,
        Powerup  = 1,
        _Unnamed = 2,
    }

    public DispenserType dispenseType;

    private readonly int HealthCost  = 20;
    private readonly int PowerupCost = 50;
    private readonly int _Unnamed    = 0;

    private readonly string PowerupPath = "Prefabs/Pickups/Powerups";
    private Powerup[] powerups;

    private void Awake()
    {
        // Load all of the powerups for the user's chance
        if (dispenseType is DispenserType.Powerup)
            powerups = Resources.LoadAll<Powerup>(PowerupPath);
    }

    private Vendor vendorComp;
    private Accountant accountant;
    private Player playerComp;

    private void Start()
    {
        vendorComp = Vendor.Instance;
        playerComp = FindFirstObjectByType<Player>();
        accountant = FindFirstObjectByType<Accountant>();
    }

    public void DispenseHealth()
    {
        if (BankSystem.TryPay(HealthCost))
        {
            if (!playerComp.IsDamaged)
            {
                vendorComp.UpdateVendorText("I'd take your money... but you are already at maximum capacity.");
                return;
            }

            BankSystem.Pay(HealthCost);
            ScoreManager.IncreaseScore(ScoreManager.HealthPointScore);
            playerComp.Heal(1);
            vendorComp.UpdateVendorText("Enjoy life for just a little bit longer... Thank ye kindly.");
            accountant.UpdateSavings();
        }
        else 
            vendorComp.UpdateVendorText("You do not have enough for a health point...");
    }

    public void DispensePowerup()
    {
        // TODO ---> take away from money
        if (BankSystem.TryPay(PowerupCost))
        {
            BankSystem.Pay(PowerupCost);
            ScoreManager.IncreaseScore(ScoreManager.PowerupPointScore);
            Powerup powerup = powerups[Random.Range(0, powerups.Length)];
            vendorComp.UpdateVendorText($"Ahhh, you rolled a {powerup.Name} capsule...");
            playerComp.UpdateProjectile(powerup.gameObject);
            accountant.UpdateSavings();
        }
        else
            vendorComp.UpdateVendorText("You do not have enough for a chance to roll a powerup...");
    }

    public string DispenseVendorText()
    {
        string dispenseText = "...";

        switch (dispenseType)
        {
            case DispenserType.Health:
                accountant.ShowCostAndEnd(HealthCost);
                dispenseText = $"That dispenser provides a health point for ${HealthCost}";
                break;
            case DispenserType.Powerup:
                accountant.ShowCostAndEnd(PowerupCost);
                dispenseText = $"That dispenser provides a random powerup for ${PowerupCost}";
                break;
            case DispenserType._Unnamed:
            default:
                break;
        }

        return dispenseText;
    }
}
