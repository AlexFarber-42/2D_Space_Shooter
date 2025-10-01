using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpgradeGenerator : MonoBehaviour
{
    public static UpgradeGenerator Instance { get; private set; }

    [SerializeField] private GameObject upgradeSelectObject;
    [SerializeField] private Transform upgradeContentTransform;

    private List<UpgradeSO> upgrades;

    // The pool represents the upgrades loaded with a flag if they have been taken or not
    private Dictionary<UpgradeSO, bool> upgradePool;

    private readonly string upgradePath = "ScriptableObjects/Upgrades";

    private void Awake()
    {
        Instance = this;

        upgrades = new List<UpgradeSO>();
        upgrades = Resources.LoadAll<UpgradeSO>(upgradePath).ToList();

        upgradePool = new Dictionary<UpgradeSO, bool>();
    }

    public void LoadAnUpgrade()
    {
        UpgradeSO newUpgrade = upgrades[Random.Range(0, upgrades.Count)];
        upgrades.Remove(newUpgrade);

        upgradePool.Add(newUpgrade, false);
        ResetShowcase();
    }

    public void ResetShowcase()
    {
        foreach (Transform child in upgradeContentTransform)
            Destroy(child.gameObject);

        foreach (KeyValuePair<UpgradeSO, bool> upgrade in upgradePool)
        {
            GameObject selector = Instantiate(upgradeSelectObject, upgradeContentTransform);
            UpgradeSelector upgComp = selector.GetComponent<UpgradeSelector>();
            upgComp.InjectUpgrade(upgrade.Key);

            if (upgrade.Value)
                upgComp.DeactivateUpgrade();
        }
    }
}
