using UnityEngine;
using UnityEngine.Pool;

public class EntityPool
{
    private GameObject prefab;
    private ObjectPool<GameObject> pool;

    private Transform parentTrans;
    private byte defaultSize;
    private byte maxSize;

    public GameObject PrefabCheck
    {
        get => prefab;
    }

    public string PoolName
    {
        get => prefab.name;
    }

    public EntityPool(GameObject prefab, Transform parentTrans, byte defaultSize = 32, byte maxSize = 255)
    {
        this.prefab         = prefab;
        this.parentTrans    = parentTrans;
        this.defaultSize    = defaultSize;
        this.maxSize        = maxSize;

        pool = new ObjectPool<GameObject>
            (
                OnCreateEntity,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyEntity,
                true,
                defaultSize,
                maxSize
            );
    }

    private GameObject OnCreateEntity()
    {
        GameObject newEntity = GameObject.Instantiate(prefab, parentTrans);
        return newEntity;
    }

    private void OnTakeFromPool(GameObject entity)
    {
        entity.SetActive(true);
    }

    private void OnReturnedToPool(GameObject entity)
    {
        entity.SetActive(false);
    }

    private void OnDestroyEntity(GameObject entity)
    {
        GameObject.Destroy(entity);
    }

    public GameObject OnRetrieveEntity(Vector3 position)
    {
        GameObject entity = pool.Get();
        entity.transform.position = position;
        return entity;
    }

    public void OnReleaseEntity(GameObject entity)
    {
        pool.Release(entity);
    }
}
