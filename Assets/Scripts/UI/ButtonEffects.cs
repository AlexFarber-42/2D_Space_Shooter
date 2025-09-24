using System;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEffects : MonoBehaviour
{
    public void MatchDisabledColorToChild(int index)
    {
        try
        {
            GameObject child = transform.GetChild(index).gameObject;

            child.GetComponent<Image>().color = GetComponent<Button>().colors.disabledColor;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Unable to shift child color to color of script holding object. Exception:\n{ex}");
        }
    }

    public void MatchEnabledColorToChild(int index)
    {
        try
        {
            GameObject child = transform.GetChild(index).gameObject;

            child.GetComponent<Image>().color = GetComponent<Button>().colors.normalColor;
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Unable to shift child color to color of script holding object. Exception:\n{ex}");
        }
    }
}
