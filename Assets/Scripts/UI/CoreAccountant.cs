using TMPro;
using UnityEngine;

public class CoreAccountant : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coreAvailText;

    public void UpdateText()
    {
        coreAvailText.text = $"{BankSystem.Core_Savings} Cores Available!";
    }
}
