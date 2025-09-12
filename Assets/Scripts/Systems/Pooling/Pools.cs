using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Pools : MonoBehaviour
{
    public enum PoolType
    {
        Enemy       = 0,
        Projectile  = 1,
        Pickup      = 2,
        Hazards     = 3,
        Particle    = 4,
    }

    public static Pools Instance { get; private set; }

    [SerializeField] private Transform[] poolTransforms;

    private Dictionary<EntityPool, Transform> pools = new Dictionary<EntityPool, Transform>();

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

    public GameObject SpawnObject(PoolType type, GameObject objectPrefab, Vector3 loc, Quaternion quatDir)
    {
        if (pools.Count is 0 || !pools.Any(pool => pool.Key.PrefabCheck == objectPrefab))
        {
            GameObject newTransform = new GameObject($"{objectPrefab.name} Pool");
            newTransform.transform.SetParent(poolTransforms[(int)type]);

            pools.Add(new EntityPool(objectPrefab, newTransform.transform), newTransform.transform);
        }

        EntityPool pool = pools.First(p => p.Key.PrefabCheck == objectPrefab).Key;

        return pool.OnRetrieveEntity(loc);
    }

    public void RemoveObject(GameObject objectToRemove)
    {
        try
        {
            EntityPool pool = pools.First(p => objectToRemove.name.Contains(p.Key.PoolName)).Key;

            pool.OnReleaseEntity(objectToRemove);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Exception occurred when attempting to release an enemy {objectToRemove.name} in RemoveEnemy(GameObject) within EnemyPools.cs\nException:\n{ex}");
            Debug.LogWarning("Destroying object instead...");
            Destroy(objectToRemove);
        }
    }
}
