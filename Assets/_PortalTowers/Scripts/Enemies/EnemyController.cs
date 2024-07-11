using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyAnimator), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private Transform target;
    private NavMeshAgent navMesh;
    private EnemyAnimator animator;
    [Header("Settings")] 
    [SerializeField] private float effectiveDistance;
    private bool follow;
    private bool canBeDestroyed;

    public bool CanBeDestroyed
    {
        get => canBeDestroyed;
        set => canBeDestroyed = value;
    }

    private void Awake()
    {
        animator = GetComponent<EnemyAnimator>();
        navMesh = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        navMesh.SetDestination(Vector3.zero);
        StartCoroutine(StopInitialNavigation());
        EventsManager.Instance.ActionEnemyKilled += OnHit;
    }

    IEnumerator StopInitialNavigation()
    {
        yield return new WaitForSeconds(0.01f);
        navMesh.isStopped = true;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionEnemyKilled -= OnHit;
    }

    private void Update()
    {
        if(target == null) return;
        float currentDistanceFromTarget = Vector3.Distance(transform.position, target.position);
        if (currentDistanceFromTarget <= effectiveDistance && !follow)
        {
            follow = true;
            animator.PlayRunAnimation();
        }
            
        if (Vector3.Distance(transform.position, target.position) >= navMesh.stoppingDistance && follow)
                navMesh.SetDestination(target.position);
    }


    private void OnHit(GameObject enemyContainer)
    {
        if (enemyContainer.name.Equals(gameObject.name))
            StartCoroutine(DestroyEnemy());
    }

    IEnumerator DestroyEnemy()
    {
        yield return new WaitUntil( () => canBeDestroyed);
        Destroy(gameObject);
    }
}