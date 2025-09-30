using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class VendorSignaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum SignalType
    {
        Junk,
        Dispenser,
        Upgrade
    }

    [SerializeField] private SignalType signal;

    private Accountant accountant;

    private void Start()
    {
        accountant = FindFirstObjectByType<Accountant>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (signal)
        {
            case SignalType.Junk:
                JunkSelector junkComp = GetComponent<JunkSelector>();
                Vendor.Instance.UpdateVendorText(junkComp.RetrieveVendorText());
                break;
            case SignalType.Dispenser:
                Dispenser dispComp = transform.parent.GetComponent<Dispenser>();
                Vendor.Instance.UpdateVendorText(dispComp.DispenseVendorText());
                break;
            case SignalType.Upgrade:
                UpgradeSelector upgradeComp = transform.parent.GetComponent<UpgradeSelector>();
                Vendor.Instance.UpdateVendorText(upgradeComp.RetrieveVendorText());
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        accountant.UpdateSavings();
        Vendor.Instance.ResetVendorText();
    }
}
