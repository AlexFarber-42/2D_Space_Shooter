using TMPro;
using UnityEngine;

public class Accountant : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI savingsText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI endVText;

    private void Awake()
    {
        UpdateSavings();
    }

    public void ShowCostAndEnd(int cost)
    {
        costText.text = cost.ToString("Cost     00000");
        endVText.text = (BankSystem.Savings - cost).ToString("         00000");
    }

    public void UpdateSavings()
    {
        savingsText.text = BankSystem.Savings.ToString("Savings  00000");
        costText.text = "Cost     00000";
        endVText.text = "         00000";
    }
}
