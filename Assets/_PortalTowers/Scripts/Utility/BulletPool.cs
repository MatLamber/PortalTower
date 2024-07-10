using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> pools;
    
    public Dictionary<string, Queue<GameObject>> bulletPoolDictonary;


    public static BulletPool Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        bulletPoolDictonary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            bulletPoolDictonary.Add(pool.tag,objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        if (!bulletPoolDictonary.ContainsKey(tag)) return null;
        GameObject objectToSpawn = bulletPoolDictonary[tag].Dequeue();
       
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;
        
        bulletPoolDictonary[tag].Enqueue(objectToSpawn);
        
        return objectToSpawn;

    }
    
}
