using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextFlickerEffect : MonoBehaviour
{
    [SerializeField] protected List<Color> colorFlickers = new List<Color>();
    [SerializeField][Tooltip("For faster flickering, put a lower value")] protected float flickerInterval = .01f;
    [SerializeField] protected bool linearColorTransition = false;

    private TextMeshProUGUI textComp;
    private bool initialized = false;

    private void OnEnable()
    {
        if (!initialized)
            InitializeComponents();

        StartCoroutine(Flicker());
    }

    private void InitializeComponents()
    {
        textComp = GetComponent<TextMeshProUGUI>();

        initialized = true;
    }

    protected void OnDisable()
    {
        StopAllCoroutines();
    }

    protected virtual IEnumerator Flicker()
    {
        int index = 0;

        while (true)
        {
            if (linearColorTransition)
            {
                Color chosenColor = colorFlickers[index];
                ++index;

                if (index == colorFlickers.Count)
                    index = 0;

                textComp.color = chosenColor;
            }
            else 
                textComp.color = colorFlickers[Random.Range(0, colorFlickers.Count)];

            yield return new WaitForSeconds(flickerInterval);
        }
    }
}
