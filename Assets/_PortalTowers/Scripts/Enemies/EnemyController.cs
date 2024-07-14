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
    private Collider collider => GetComponent<Collider>();
    private NavMeshAgent navMeshAgent => GetComponent<NavMeshAgent>();

    [SerializeField] private Transform target;

    [SerializeField] private float aggresionRange;

    private bool follow;

    private GameObject enemyPrefab => transform.GetChild(0).gameObject;
    private Rigidbody[] enemyRigidbodies => transform.GetComponentsInChildren<Rigidbody>();
    private Collider[] enemyColliders => transform.GetComponentsInChildren<Collider>();


    private int hitPoints = 3;


    private void Start()
    {
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
            if (Vector3.Distance(target.position, transform.position) < aggresionRange)
                follow = true;

            if(follow && Vector3.Distance(target.position, transform.position) > navMeshAgent.stoppingDistance)
                navMeshAgent.SetDestination(target.position);

        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Die();
        }
    }

    private void Die()
    {
        collider.tag = "Untagged";
        EnableRigidBody();
        EventsManager.Instance.OnEnemyDeath(transform);
     //  Destroy(gameObject);
    }

    private void EnableRigidBody()
    {
        navMeshAgent.enabled = false;
        transform.GetComponent<Collider>().enabled = false;
        enemyPrefab.GetComponent<Animator>().enabled = false;
        for (int i = 0; i < enemyRigidbodies.Length; i++)
        {
                enemyRigidbodies[i].isKinematic = false;
        }

        StartCoroutine(GoTroughFloor());

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

        enemyPrefab.transform.DOLocalMoveY(-1.75f, 2.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position,aggresionRange);    
    }

    private void OnHit(Transform enemyHit)
    {
        Debug.Log($"HIT");
        if (transform == enemyHit)
        {
            hitPoints--;
            if(hitPoints <= 0)
                Die();
        }
    }
}
