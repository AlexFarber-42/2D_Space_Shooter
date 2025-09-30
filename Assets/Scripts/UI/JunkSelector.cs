using TMPro;
using System;
using UnityEngine;
using UnityEngine.UI;

public class JunkSelector : MonoBehaviour
{
    [SerializeField] private Image junkSprite;
    [SerializeField] private TextMeshProUGUI priceValue; // Format -> $000

    private JunkSO retainedJunk;

    private Accountant accountant;

    private void Start()
    {
        accountant = FindFirstObjectByType<Accountant>();
    }

    public void InjectJunk(JunkSO junkObject)
    {
        try
        {
            junkSprite.sprite   = junkObject.junkSprite;
            priceValue.text     = junkObject.JunkPrice.ToString("$000"); 

            retainedJunk = junkObject;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"An issue arose when trying to inject ({junkObject.name}) into this JunkSelector ({this.name})\nException\n{ex}");
        }
    }

    
    public string RetrieveVendorText()
    {
        accountant.ShowCostAndEnd(retainedJunk.JunkPrice);
        return retainedJunk.vendorStrings[UnityEngine.Random.Range(0, retainedJunk.vendorStrings.Length)];
    }
}
