using System.Collections;
using TMPro;
using UnityEngine;

public class TextFlickerEffect_Group : TextFlickerEffect
{
    [SerializeField] private TextMeshProUGUI[] textFlickerTexts;

    private void OnEnable()
    {
        StartCoroutine(Flicker());
    }

    protected override IEnumerator Flicker()
    {
        int index = 0;

        while (true)
        {
            Color chosenColor;

            if (linearColorTransition)
            {
                chosenColor = colorFlickers[index];
                ++index;

                if (index == colorFlickers.Count)
                    index = 0;
            }
            else
                chosenColor = colorFlickers[Random.Range(0, colorFlickers.Count)];

            foreach (TextMeshProUGUI textFlickerEffect in textFlickerTexts)
                textFlickerEffect.color = chosenColor;
            
            yield return new WaitForSeconds(flickerInterval);
        }
    }
}
