using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    private Collider collider => GetComponent<Collider>();
    private NavMeshAgent navMeshAgent;

    [SerializeField] private Transform target;


    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if(target is not null)
            navMeshAgent.SetDestination(target.position);
    }

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
