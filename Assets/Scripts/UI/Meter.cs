using System.Collections.Generic;
using UnityEngine;

public class Meter : MonoBehaviour
{
    public enum MeterType
    {
        FireRate,
        MoveSpeed,
        Damage,
        Health,
        ChargeRate
    }

    [SerializeField] private MeterType meterType;

    private int statPointLevIndex = 1;
    private const int MaxPoints = 20;

    private readonly Dictionary<int, float> FireRateMap = new Dictionary<int, float>()
    {
        {0, 2f},
        {1, .1f},
        {2, .1f},
        {3, .1f},
        {4, .1f},
        {5, .1f},
        {6, .1f},
        {7, .1f},
        {8, .1f},
        {9, .1f},
        {10, .1f},
        {11, .1f},
        {12, .1f},
        {13, .1f},
        {14, .1f},
        {15, .1f},
        {16, .1f},
        {17, .1f},
        {18, .1f},
        {19, .1f}
    };

    private readonly Dictionary<int, float> MoveSpeedMap = new Dictionary<int, float>()
    {
        {0, 2f},
        {1, .05f},
        {2, .05f},
        {3, .05f},
        {4, .125f},
        {5, .05f},
        {6, .05f},
        {7, .05f},
        {8, .05f},
        {9,  .15f},
        {10, .05f},
        {11, .05f},
        {12, .05f},
        {13, .05f},
        {14, .175f},
        {15, .05f},
        {16, .05f},
        {17, .05f},
        {18, .05f},
        {19, .2f}
    };

    private readonly Dictionary<int, int> DamageMap = new Dictionary<int, int>()
    {
        {0, 0},
        {1, 1},
        {2, 1},
        {3, 1},
        {4, 1},
        {5, 1},
        {6, 1},
        {7, 1},
        {8, 1},
        {9,  1},
        {10, 1},
        {11, 1},
        {12, 1},
        {13, 1},
        {14, 1},
        {15, 1},
        {16, 1},
        {17, 1},
        {18, 1},
        {19, 1}
    };

    private readonly Dictionary<int, int> HealthMap = new Dictionary<int, int>()
    {
        {0, 3},
        {1, 1},
        {2, 1},
        {3, 1},
        {4, 1},
        {5, 1},
        {6, 1},
        {7, 1},
        {8, 1},
        {9,  1},
        {10, 1},
        {11, 1},
        {12, 1},
        {13, 1},
        {14, 1},
        {15, 1},
        {16, 1},
        {17, 1},
        {18, 1},
        {19, 1}
    };

    private readonly Dictionary<int, float> ChargeRateMap = new Dictionary<int, float>()
    {
        {1, 0f},
        {2, .1f},
        {3, .1f},
        {4, .1f},
        {5, .1f},
        {6, .1f},
        {7, .1f},
        {8, .1f},
        {9, .1f},
        {10, .25f},
        {11, .1f},
        {12, .1f},
        {13, .1f},
        {14, .1f},
        {15, .1f},
        {16, .1f},
        {17, .1f},
        {18, .1f},
        {19, .1f},
        {20, .5f}
    };

    private CoreAccountant accountant;

    private void OnEnable()
    {
        // TODO ---> take from loading functionality to gather the current point allotment
        // Ie. load statPointLevIndex to PlayerPrefs or some save file and load it here
        
        
        if (accountant == null)
            accountant = FindFirstObjectByType<CoreAccountant>();

        int indexHolder = 0;
        foreach (Transform child in transform)
        {
            if (indexHolder < statPointLevIndex)
                child.GetComponent<MeterPoint>().Activate();
            else
                child.GetComponent<MeterPoint>().Deactivate();

            indexHolder++;
        }
    }

    private void OnDisable()
    {
        // TODO ---> Save point allotment with saving functionality here
        // Ie. save statPointLevIndex to PlayerPrefs or some save file and load it here
    }

    public void IncreaseStat()
    {
        // Guard Clause for going past the max points allowed to be alloted
        if (statPointLevIndex == MaxPoints - 1)
            return;

        transform.GetChild(statPointLevIndex).GetComponent<MeterPoint>().Activate();

        statPointLevIndex++;
        
        float value = meterType switch
        {
            MeterType.FireRate      => FireRateMap[statPointLevIndex],
            MeterType.MoveSpeed     => MoveSpeedMap[statPointLevIndex],
            MeterType.Damage        => DamageMap[statPointLevIndex],
            MeterType.Health        => HealthMap[statPointLevIndex],
            MeterType.ChargeRate    => ChargeRateMap[statPointLevIndex],
            _                       => 0f,
        };

        Player.Instance.ModifyBaseStat(meterType, value);
        BankSystem.PayCore(1);
        accountant.UpdateText();
    }

    public void DecreaseStat()
    {
        // Guard Clause for if going below the base minimum points allowed to be alloted
        if (statPointLevIndex == 1)
            return;

        transform.GetChild(statPointLevIndex).GetComponent<MeterPoint>().Deactivate();

        float value = meterType switch
        {
            MeterType.FireRate      => FireRateMap[statPointLevIndex],
            MeterType.MoveSpeed     => MoveSpeedMap[statPointLevIndex],
            MeterType.Damage        => DamageMap[statPointLevIndex],
            MeterType.Health        => HealthMap[statPointLevIndex],
            MeterType.ChargeRate    => ChargeRateMap[statPointLevIndex],
            _                       => 0f,
        };

        Player.Instance.ModifyBaseStat(meterType, -value);
        BankSystem.AddCore(1);
        accountant.UpdateText();

        statPointLevIndex--;
    }
}
