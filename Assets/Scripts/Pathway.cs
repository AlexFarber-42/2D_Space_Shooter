using UnityEngine;

public class Pathway : MonoBehaviour
{
    [SerializeField] private Transform[] wavePoints;

    public Transform[] GetWavePoints() => wavePoints;
}
