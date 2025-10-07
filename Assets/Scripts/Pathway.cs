using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] private Transform[] wavePoints;

    [Header("Variable Wave Limits")]
    [SerializeField] private Vector3 minArea;
    [SerializeField] private Vector3 maxArea;

    public Transform[] GetWavePoints() => wavePoints;
    public Vector3[] GetPositionPoints()
    {
        Vector3[] posPoints = new Vector3[wavePoints.Length];

        // If the path is detected to be only 2, aka a travel across screen, then randomize the instantiation location 
        if (transform.childCount is 2)
        {
            Transform startPath = transform.GetChild(0);
            Transform endPath   = transform.GetChild(1);

            // Default is 0, 0
            Vector3 posOfSpawnOfPath;

            if (startPath.position.x is 0)  // Vertical Movement
                posOfSpawnOfPath = new Vector3(Random.Range(minArea.x, maxArea.x), 0f, 0f);
            else                            // Horizontal Movements
                posOfSpawnOfPath = new Vector3(0f, Random.Range(minArea.y, maxArea.y), 0f);

            return new Vector3[2] { startPath.position + posOfSpawnOfPath, endPath.position + posOfSpawnOfPath };
        }
        else
        {
            for (int i = 0; i < wavePoints.Length; ++i)
                posPoints[i] = wavePoints[i].position;

            return posPoints;
        }
    }
}
