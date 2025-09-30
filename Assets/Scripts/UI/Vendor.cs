using TMPro;
using UnityEngine;

public class Vendor : MonoBehaviour
{
    public static Vendor Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI vendorText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void UpdateVendorText(string newString)
    {
        // TODO ---> Add a printing effect to the text

        vendorText.text = newString;
        // TODO ---> Do further modification to include rich text for the rarity aspect of the strings
    }

    [SerializeField] private string[] generalVendorPhrasesAndTips;

    public void ResetVendorText()
        => UpdateVendorText(generalVendorPhrasesAndTips[Random.Range(0, generalVendorPhrasesAndTips.Length)]);
}
