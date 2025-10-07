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

    private List<EntityPool> pools = new List<EntityPool>();

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
        if (pools.Count is 0 || !pools.Any(pool => pool.PrefabCheck == objectPrefab))
        {
            GameObject newTransform = new GameObject($"{objectPrefab.name} Pool");
            newTransform.transform.SetParent(poolTransforms[(int)type]);

            pools.Add(new EntityPool(objectPrefab, newTransform.transform));
        }

        EntityPool pool = pools.First(p => p.PrefabCheck == objectPrefab);

        return pool.OnRetrieveEntity(loc);
    }

    public void RemoveObject(GameObject objectToRemove)
    {
        try
        {
            EntityPool pool = pools.First(p => objectToRemove.name.Contains(p.PoolName));

            pool.OnReleaseEntity(objectToRemove);
        }
        catch (InvalidOperationException)
        {
            // Invalid Operation Exceptions are prevalent and likely mean an object was released and freed from a pool rapidly that causes a hiccup
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Exception caught in RemoveObject(GameObject) in Pools.cs\nException: {ex}");
        }
    }
}
