using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Collider collider => GetComponent<Collider>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            Die();
        }
    }

    private void Die()
    {
        collider.tag = "Untagged";
        EventsManager.Instance.OnEnemyDeath(transform);
        Destroy(gameObject);
    }
}
