using TMPro;
using UnityEngine;
using System.Collections;

/// <summary>
/// Handles the drawer object on the side of the player's screen for 
/// showing their current savings/money
/// </summary>
public class MoneyTracker : MonoBehaviour
{
    public static MoneyTracker Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI amountText; // ---> 00000 Formatting
    
    private DrawerGUIMovement drawer;
    private Coroutine activeDrawerSlip;

    private readonly float drawerLinger = 3.5f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        drawer              = GetComponent<DrawerGUIMovement>();
        activeDrawerSlip    = null;
        UpdateString(true);
    }

    public void UpdateString(bool isInit = false)
    {
        amountText.text = BankSystem.Savings.ToString("00000");

        // Signal to pull the small window out and start a timer to push it back in
        if (!isInit)
        {
            // Reset the coroutine if money is updated again
            if (activeDrawerSlip != null)
                StopAllCoroutines();
            else
                drawer.ToggleDrawer();

            activeDrawerSlip = StartCoroutine(DrawerSlip());
        }
    }

    private IEnumerator DrawerSlip()
    {
        yield return new WaitForSeconds(drawerLinger);

        // TODO ---> Add some functionality where the drawer image fades
        // in and out depending on the recency of obtaining money

        drawer.ToggleDrawer();
        activeDrawerSlip = null;
    }
}
