using System;
using System.Collections.Generic;
using UnityEngine;

public class JunkGenerator : MonoBehaviour
{
    [SerializeField] private JunkSelector[] junkSelections;

    private JunkSO[] availableJunk;
    private readonly string junkPath = "ScriptableObjects/Junk";

    private void Awake()
    {
        JunkSO[] availJunk = Resources.LoadAll<JunkSO>(junkPath);
        availableJunk = new JunkSO[availJunk.Length];
        Array.Copy(availJunk, availableJunk, availJunk.Length);
    }

    private void Start()
    {
        ResetJunk();
    }

    private readonly Dictionary<JunkSO.JunkRarity, int> Chance = new Dictionary<JunkSO.JunkRarity, int>()
    {
        { JunkSO.JunkRarity.Common, 100 },
        { JunkSO.JunkRarity.Uncommon, 75 },
        { JunkSO.JunkRarity.Rare, 45 },
        { JunkSO.JunkRarity.Epic, 20 },
        { JunkSO.JunkRarity.Exotic, 5 },
    };


    private void ResetJunk(int index = -1)
    {
        if (index is -1)    // Indicates no specific index to switch out junk, means all needs to be switched out
        {
            for (int i = 0; i < junkSelections.Length; ++i)
                GenerateJunk(i);
        }
        else                // If index is available, means a specific piece of junk needs to be switched out
            GenerateJunk(index);
    }

    private void GenerateJunk(int selectorIndex)
    {
        JunkSO chosenJunk = null;

        // Select junk
        do
        {
            JunkSO pickedJunk = availableJunk[UnityEngine.Random.Range(0, availableJunk.Length)];

            JunkSO.JunkRarity rarity = pickedJunk.junkRarity;
            int roll = UnityEngine.Random.Range(0, 101);

            if (roll <= Chance[rarity]) // Add, otherwise pick another junk and roll again
                chosenJunk = pickedJunk;
        }
        while (chosenJunk == null);

        junkSelections[selectorIndex].InjectJunk(chosenJunk);
    }
}
