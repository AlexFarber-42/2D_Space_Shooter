using UnityEngine;

public class MeterPoint : MonoBehaviour
{
    [SerializeField] private Sprite deactivatedSprite;
    [SerializeField] private Sprite activatedSprite;

    public void Activate()   => GetComponent<SpriteRenderer>().sprite = activatedSprite;
    public void Deactivate() => GetComponent<SpriteRenderer>().sprite = deactivatedSprite;
}
