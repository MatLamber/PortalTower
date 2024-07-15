using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyAnimationController))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Enemy data;
    [SerializeField] private GameObject enemyPrefab; 
    private Collider collider => GetComponent<Collider>();
    private NavMeshAgent navMeshAgent => GetComponent<NavMeshAgent>();
    private EnemyAnimationController animator => GetComponent<EnemyAnimationController>();
    private Rigidbody[] enemyRigidbodies => transform.GetComponentsInChildren<Rigidbody>();
    private Collider[] enemyColliders => transform.GetComponentsInChildren<Collider>();

    private Transform target;


    public Transform Target
    {
        get => target;
        set => target = value;
    }
    


    private float aggresionRange;
    private bool follow;
    private int hitPoints;
    
    private void Start()
    {
        SetData();
        InitalNavMesh();
        EventsManager.Instance.eventEnemyHit += OnHit;
    }

    private void OnDestroy()
    {
       EventsManager.Instance.eventEnemyHit -= OnHit;
    }

    private void Update()
    {
        if (navMeshAgent.enabled)
        {
            if (Vector3.Distance(target.position, transform.position) < aggresionRange && !follow)
            {
                navMeshAgent.isStopped = false;
                follow = true;
                animator.PlayRunAnimation();
            }
            
            if(follow && Vector3.Distance(target.position, transform.position) > navMeshAgent.stoppingDistance)
                navMeshAgent.SetDestination(target.position);
        }
    }

    private void Die()
    {
        collider.tag = "Untagged";
        EnableRigidBody();
        EventsManager.Instance.OnEnemyDeath(transform);
    }

    private void EnableRigidBody()
    {
        navMeshAgent.enabled = false;
        transform.GetComponent<Collider>().enabled = false;
        animator.Animator.enabled = false;
        for (int i = 0; i < enemyRigidbodies.Length; i++)
        {
                enemyRigidbodies[i].isKinematic = false;
        }

        StartCoroutine(GoTroughFloor());

    }

    private void DisableRigidBody()
    {
        navMeshAgent.enabled = true;
        transform.GetComponent<Collider>().enabled = true;
        animator.Animator.enabled = true;
        for (int i = 0; i < enemyRigidbodies.Length; i++)
        {
            enemyRigidbodies[i].isKinematic = true;
        }
        
        for (int i = 0; i < enemyRigidbodies.Length; i++)
        {
            enemyRigidbodies[i].useGravity = true;
            enemyRigidbodies[i].isKinematic = false;
        }

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].isTrigger = false;
        }

    }

    IEnumerator GoTroughFloor()
    {
        yield return new WaitForSeconds(1);
        for (int i = 0; i < enemyRigidbodies.Length; i++)
        {
            enemyRigidbodies[i].useGravity = false;
            enemyRigidbodies[i].isKinematic = true;
        }

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            enemyColliders[i].isTrigger = true;
        }

        enemyPrefab.transform.DOLocalMoveY(-1.75f, 1f).OnComplete( () =>
        {
            DisableRigidBody();
            Destroy(gameObject);
           // ObjectPool.Instance.ReturnObject(gameObject,1f);
        });
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,aggresionRange);    
    }

    private void OnHit(Transform enemyHit)
    {
        if (transform == enemyHit)
        {
            hitPoints--;
            if(hitPoints <= 0)
                Die();
        }
    }
    
    private void SetData()
    {
        aggresionRange = data.aggressionRange;
        hitPoints = data.hitPoints;
    }

    private void InitalNavMesh()
    {

        StartCoroutine(StopInitialNavMesh());
    }

    IEnumerator StopInitialNavMesh()
    {
        yield return new WaitForSeconds(0.2f);
        navMeshAgent.enabled = true;
        yield return new WaitUntil(() => navMeshAgent.isOnNavMesh);
        navMeshAgent.SetDestination(Vector3.zero);
        yield return new WaitForSeconds(0.2f);
        navMeshAgent.isStopped = true;
    }


}
