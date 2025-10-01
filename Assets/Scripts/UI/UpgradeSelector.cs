using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeSelector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI upgradeName;
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] private Image spriteImage;

    [Header("Deactivation Components")]
    [SerializeField] private Button disablingButton;
    [SerializeField] private Image upgradeImgPart1;
    [SerializeField] private Image upgradeImgPart2;

    private UpgradeSO upgradeObject;
    private Accountant accountant;
    private Player player;

    private void Start()
    {
        accountant  = FindFirstObjectByType<Accountant>();
        player      = Player.Instance;
    }

    public void InjectUpgrade(UpgradeSO upgrade)
    {
        upgradeName.text        = upgrade.name;
        textField.text          = upgrade.upgradeDescription;
        spriteImage.sprite      = upgrade.upgradeSprite;

        upgradeObject           = upgrade;
    }

    public void PurchaseUpgrade()
    {
        if (!BankSystem.TryPay(upgradeObject.upgradeCost))
        {
            Vendor.Instance.UpdateVendorText($"Ye can't afford this upgrade, come back again when you have the cash.");
            return;
        }
        else 
        {
            Vendor.Instance.UpdateVendorText($"Done. Your ship has been upgraded for the agreed amount.");
            BankSystem.Pay(upgradeObject.upgradeCost);

            player.Upgrade(upgradeObject.upgradeVal);
            DeactivateUpgrade();
        }
    }

    public void DeactivateUpgrade()
    {
        upgradeImgPart1.color        = disablingButton.colors.disabledColor;
        upgradeImgPart2.color        = disablingButton.colors.disabledColor;
        spriteImage.color            = disablingButton.colors.disabledColor;
        disablingButton.interactable = false;
    }

    public string RetrieveVendorText()
    {
        accountant.ShowCostAndEnd(upgradeObject.upgradeCost);
        return upgradeObject.upgradeVendorStrings[Random.Range(0, upgradeObject.upgradeVendorStrings.Length)];
    }
}
