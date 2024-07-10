using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private string enemyTag = "Enemy";
    private void OnTriggerEnter(Collider other)
    {
        if (!other.tag.Equals(enemyTag)) return;
        GameObject enemyContainer = other.transform.parent.gameObject;
        ResetBullet();
        EventsManager.Instance.OnEnemyKilled(enemyContainer);
    }
    

    private void ResetBullet()
    {
        gameObject.SetActive(false);
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.rotation = quaternion.identity;
    }
}
