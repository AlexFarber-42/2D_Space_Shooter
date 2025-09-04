using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private float parallaxVal;
    [SerializeField] private float startPos = 0;
    [SerializeField] private float endPos = 10;
    [SerializeField] private float intensity = 5;
    [SerializeField] private bool isVertical = false;

    private void Start()
    {
        ResetPosition();
    }

    private void Update()
    {
        float distance;
        Vector2 currentPos = transform.position;

        if (!isVertical)
        {
            distance = intensity * parallaxVal * Time.deltaTime;
            transform.position = new Vector2(currentPos.x + distance, currentPos.y);
        }
        else
        {
            distance = intensity * parallaxVal * Time.deltaTime;
            transform.position = new Vector2(currentPos.x, currentPos.y + distance);
        }

        bool reachedThreshold = isVertical ? currentPos.y >= endPos : currentPos.x >= endPos;

        if (reachedThreshold)
            ResetPosition();
    }

    private void ResetPosition()
    {
        if (!isVertical)
            transform.position = new Vector2(startPos, transform.position.y);
        else
            transform.position = new Vector2(transform.position.x, startPos);
    }
}
