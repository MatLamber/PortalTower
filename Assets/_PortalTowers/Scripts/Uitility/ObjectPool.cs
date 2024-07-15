using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    [SerializeField] private int poolSize = 10;
    private Dictionary<GameObject, Queue<GameObject>> poolDictonary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        
    }
    
    public GameObject GetObjet(GameObject poolKey)
    {
        if (!poolDictonary.ContainsKey(poolKey))
            InitializeNewPool(poolKey);
        if (poolDictonary[poolKey].Count == 0)
            CreateNewObject(poolKey);
        GameObject objectToGet = poolDictonary[poolKey].Dequeue();
        objectToGet.SetActive(true);
        objectToGet.transform.parent = null;
        return objectToGet;
    }

    public void ReturnObject(GameObject objectToReturn, float delay)
    {
        StartCoroutine(DelayReturn(delay, objectToReturn));
    }
    
    private IEnumerator DelayReturn(float delay, GameObject objectToReturn)
    {
        yield return new WaitForSeconds(delay);
        ReturnObjectToPool(objectToReturn);
    }
    
    private void ReturnObjectToPool(GameObject objectToReturn)
    {
        objectToReturn.SetActive(false);
        GameObject originalPrefab = objectToReturn.GetComponent<PoolObjectIdentifier>().originalPrefab;
        objectToReturn.transform.parent = transform;
        objectToReturn.transform.localPosition = Vector3.zero;
        poolDictonary[originalPrefab].Enqueue(objectToReturn);
    }
    private void InitializeNewPool(GameObject prefab)
    {
        poolDictonary[prefab] = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject(prefab);
        }
    }


    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab,transform);
        newObject.AddComponent<PoolObjectIdentifier>().originalPrefab = prefab;
        newObject.SetActive(false);
        poolDictonary[prefab].Enqueue(newObject);
    }
}
