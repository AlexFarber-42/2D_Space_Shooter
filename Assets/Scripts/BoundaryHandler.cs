using UnityEngine;

public class BoundaryHandler : MonoBehaviour
{
    public static BoundaryHandler Instance { get; private set; }

    [SerializeField]
    private ExitPoint[] exitPoints;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public Transform RetrieveTransform(Vector2Int dir)
    {
        foreach (ExitPoint point in exitPoints)
        {
            if (point.dir == dir)
                return point.t;
        }

        // No Match found
        return null;
    }
}

[System.Serializable]
public struct ExitPoint
{
    public Transform t;
    public Vector2Int dir;
}
