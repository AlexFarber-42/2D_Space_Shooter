using TMPro;
using UnityEngine;

/// <summary>
/// Handles the drawer object on the side of the player's screen for 
/// showing their current savings/money
/// </summary>
public class MoneyTracker : MonoBehaviour
{
    public static MoneyTracker Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI amountText; // ---> 00000 Formatting
    
    private DrawerGUIMovement drawer;
    private int currentSavings;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(this);
            return;
        }

        drawer              = GetComponent<DrawerGUIMovement>();
        currentSavings = BankSystem.Savings;
        UpdateString(true);
    }

    private void Update()
    {
        if (BankSystem.Savings != currentSavings)
            UpdateString();
    }

    public void UpdateString(bool isInit = false)
    {
        amountText.text = BankSystem.Savings.ToString("00000");
        currentSavings = BankSystem.Savings;

        // Signal to pull the small window out and start a timer to push it back in
        if (!isInit)
            drawer.TriggerSlip();
    }
}
