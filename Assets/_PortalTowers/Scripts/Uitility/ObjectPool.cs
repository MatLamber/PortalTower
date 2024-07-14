using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10;
    private Queue<GameObject> bulletsPool = new Queue<GameObject>();

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
        CreateInitialPool();
    }

    public GameObject GetBullet()
    {
        if(bulletsPool.Count == 0)
            CreateNewBullet();
        GameObject bulletToGet = bulletsPool.Dequeue();
        bulletToGet.SetActive(true);
        bulletToGet.transform.parent = null;
        return bulletToGet;
    }

    public void ReturnBullet(GameObject bulletToReturn)
    {
        bulletToReturn.SetActive(false);
        bulletToReturn.transform.parent = transform;
        bulletsPool.Enqueue(bulletToReturn);
    }
    private void CreateInitialPool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewBullet();
        }
    }

    private void CreateNewBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab,transform);
        newBullet.SetActive(false);
        bulletsPool.Enqueue(newBullet);
    }
}
