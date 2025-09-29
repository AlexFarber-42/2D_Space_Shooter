using Mono.Cecil;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffects : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum HoverEffect
    {
        None = 0,
        HoverChildrenColor = 1, // Marker for if the children attached should match their color to the highlighted color of the button component
    }

    [SerializeField] private HoverEffect hoverEffect = HoverEffect.None;

    private void Start()
    {
        switch (hoverEffect)
        {
            case HoverEffect.HoverChildrenColor:
                retentionColor = new Color[transform.childCount];
                break;
            case HoverEffect.None:
            default:
                break;
        }
    }

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

    public void RotateButton()
    {
        StartCoroutine(Rotate());
    }

    private IEnumerator Rotate()
    {
        int maxStep = 5;
        float shift = 90f;
        float deltaTime = .15f;

        int curStep = 0;
        float shiftStep = shift / maxStep;

        while (curStep < maxStep) 
        {
            transform.Rotate(Vector3.forward, shiftStep);

            yield return new WaitForSeconds(deltaTime);
            ++curStep;
        }
    }

    private Color[] retentionColor; // Saves the colors of all of the children sprites if needed

    public void OnPointerEnter(PointerEventData eventData)
    {
        switch (hoverEffect)
        {
            case HoverEffect.HoverChildrenColor:

                int index = 0;

                foreach (Transform child in transform)
                {
                    try
                    {
                        Image imageComp = child.GetComponent<Image>();
                        retentionColor[index] = imageComp.color;

                        imageComp.color = GetComponent<Button>().colors.highlightedColor;
                    }
                    catch
                    {
                        // Skip if neither are available
                    }
                    finally
                    {
                        ++index;
                    }
                }
                break;
            case HoverEffect.None:
            default:
                break;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        switch (hoverEffect)
        {
            case HoverEffect.HoverChildrenColor:

                int index = 0;

                foreach (Transform child in transform)
                {
                    try
                    {
                        child.GetComponent<Image>().color = retentionColor[index];
                    }
                    catch
                    {
                        // Skip if neither are available
                    }
                    finally
                    {
                        ++index;
                    }
                }
                break;
            case HoverEffect.None:
            default:
                break;
        }
    }
}
