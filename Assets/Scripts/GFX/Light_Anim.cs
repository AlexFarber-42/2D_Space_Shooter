using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Light_Anim : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Light2D spriteLight;

    private void Awake()
    {
        spriteLight    = GetComponent<Light2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        try
        {
            // Simply updates the light cookie to the current renderer sprite
            if (spriteRenderer.sprite != spriteLight.lightCookieSprite)
                spriteLight.lightCookieSprite = spriteRenderer.sprite;
        }
        catch (Exception ex)
        {
            Debug.Log($"Components not present within {this.name} for Light_Anim. Deactivating object.\nException:\n{ex}");
            gameObject.SetActive(false);
        }
    }
}
