using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PistolController : MonoBehaviour
{
    [SerializeField] private float cadency;
    [SerializeField] private Transform spawnPoint;
    private string bulletPoolTag = "Bullet";
    private bool onEnemyOnRange;
    private GameObject bullet;
    private Vector3 enemyPosition;
    private Transform enemyTransform;
    private float timer = 0.8f;


    private void Start()
    {

        EventsManager.Instance.ActionEnemyOnRange += OnEnemyOnRange;
        EventsManager.Instance.ActionEnemyOutOfRange += OnEnemyOutOfRange;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionEnemyOnRange -= OnEnemyOnRange;
        EventsManager.Instance.ActionEnemyOutOfRange -= OnEnemyOutOfRange;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (onEnemyOnRange && timer <= 0)
        {

            timer = 0.8f;
            Shoot();
        }
        

    }



    private void OnEnemyOnRange(Transform targetTransform)
    {
        if(onEnemyOnRange) return;
        onEnemyOnRange = true;
        enemyTransform = targetTransform;
        enemyPosition = targetTransform.position;
        // StartCoroutine(ShootCoroutine());

    }

    IEnumerator ShootCoroutine()
    {
        while (onEnemyOnRange)
        {
            yield return new WaitForSeconds(cadency);
            Shoot();
        }
    }


    private void OnEnemyOutOfRange()
    {
        onEnemyOnRange = false;
    }

    private void Shoot()
    {

        if(!onEnemyOnRange) return;
        Vector3 direction = enemyTransform.position - transform.position;
        bullet = BulletPool.Instance.SpawnFromPool(bulletPoolTag, spawnPoint.position, quaternion.identity);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
         bulletRb.AddForce(direction.normalized * 20, ForceMode.VelocityChange);
    }
}