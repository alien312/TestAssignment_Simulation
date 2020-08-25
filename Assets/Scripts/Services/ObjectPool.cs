using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectPool
{
    public static ObjectPool Instance => _instance;
    private static ObjectPool _instance = null;


    readonly Dictionary<int, Pool> m_PoolDictionary;
    private int m_DefaultCapacity = 20;

    public ObjectPool()
    {
        if (_instance == null)
            _instance = this;
        else
            Debug.LogError("Попытка повторного создания синглтона - " + GetType().Name);

        m_PoolDictionary = new Dictionary<int, Pool>();
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    public void SetPoolCapacity(int poolId, int capacity)
    {
        if (m_PoolDictionary.ContainsKey(poolId))
        {
            var pool = m_PoolDictionary[poolId];
            pool.Capacity = capacity;
            m_PoolDictionary[poolId] = pool;
        }
        else
        {
            AddNewPool(poolId);
            SetPoolCapacity(poolId, capacity);
        }
    }

    public void AddNewPool(int poolId)
    {
        var objectPool = new List<GameObject>();
        var poolParent = new GameObject("Pool" + poolId);
        var pool = new Pool {Objects = objectPool, Capacity = m_DefaultCapacity, Parent = poolParent.transform};
        m_PoolDictionary.Add(poolId, pool);
    }

    public GameObject SpawnFromPool(GameObject prefab, Vector3 spawnPosition)
    {
        var poolId = prefab.GetHashCode();
        if (m_PoolDictionary.ContainsKey(poolId))
        {
            var poolingObj = m_PoolDictionary[poolId].Objects
                .FirstOrDefault(obj => obj != null && obj.GetComponent<IPoolingObject>().IsFree);
            
            if (poolingObj != null)
            {
                poolingObj.transform.position = spawnPosition;
                poolingObj.SetActive(true);
                poolingObj.GetComponent<IPoolingObject>().OnObjectSpawn();
                return poolingObj;
            }
            else
            {
                if (m_PoolDictionary[poolId].Objects.Count < m_PoolDictionary[poolId].Capacity)
                {
                    return AddInstance(prefab, spawnPosition, poolId);
                }
                else
                {
                    Debug.Log("Пул переполнен, выполнится отчистка от нулевых значений");
                    m_PoolDictionary[poolId].Objects.RemoveAll(o => o == null);
                    return null;
                }
            }
        }
        else
        {
            Debug.Log("Не создан пулл");
            return null;
        }
    }

    GameObject AddInstance(GameObject prefab, Vector3 spawnPosition, int poolId)
    {
        var obj = Object.Instantiate(prefab, spawnPosition, Quaternion.identity, m_PoolDictionary[poolId].Parent);
        m_PoolDictionary[poolId].Objects.Add(obj);
        obj.SetActive(true);
        obj.GetComponent<IPoolingObject>().OnObjectSpawn();
        return obj;
    }

    public void RemoveInstance(GameObject prefab)
    {
        var poolId = prefab.GetHashCode();
        if (m_PoolDictionary.ContainsKey(poolId))
        {
            m_PoolDictionary[poolId].Objects.Remove(prefab);
        }
    }

    void OnSceneChanged(Scene scene1, Scene scene2)
    {
        if (scene1.buildIndex != 1 || scene2.buildIndex != 0) return;
        if(m_PoolDictionary.Count != 0)
            m_PoolDictionary.Clear();
    }
}

public interface IPoolingObject
{
    bool IsFree { get; }
    void OnObjectSpawn();
}

public struct Pool
{
    public List<GameObject> Objects;
    public int Capacity;
    public Transform Parent;
}