using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent), typeof(EnemyAnimationController)), RequireComponent(typeof(Outline))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Enemy data;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private bool useExtraForce;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private Image healthFill;
    [SerializeField] private GameObject damageText;
    [SerializeField] private List<Renderer> bodyParts;
    [SerializeField] private Color flashColor;
    private Collider collider => GetComponent<Collider>();
    private NavMeshAgent navMeshAgent => GetComponent<NavMeshAgent>();
    private EnemyAnimationController animator => GetComponent<EnemyAnimationController>();
    private Rigidbody[] enemyRigidbodies => transform.GetComponentsInChildren<Rigidbody>();
    private Collider[] enemyColliders => transform.GetComponentsInChildren<Collider>();

    private Transform target;

    private float aggresionRange;
    private bool follow;
    private float hitPoints;
    private float originalSpeed;
    private float flyAwayForce = 450;
    private float extraForce = 500;

    private List<Material> materials = new List<Material>();
    private List<Material> originalMaterials = new List<Material>();
        
    private bool isDead;
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");

    private Outline outline => GetComponent<Outline>();
    
    private float attackRange;
    private bool isAttacking;
    
    public Transform Target
    {
        get => target;
        set => target = value;
    }


    private void Awake()
    {
        foreach (Renderer bodyPart in bodyParts)
        {
            materials.Add(bodyPart.material);
            originalMaterials.Add(bodyPart.material);
        }
        
        outline.OutlineColor = Color.red;
        DisableOutLine(false,transform);
    }

    private void Start()
    {
        healthUI.SetActive(false);
        SetData();
        InitalNavMesh();
        EventsManager.Instance.eventEnemyHit += OnHit;
        EventsManager.Instance.eventEnemyLockedIn += EnableOutline;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventEnemyLockedIn -= EnableOutline;
         EventsManager.Instance.eventEnemyHit -= OnHit;
    }

    private void Update()
    {
        if (navMeshAgent.enabled)
        {
            CheckIfPlayerIsOnAggressionRange();
            if (follow)
            {
                FollowPlayer();
                Attack();
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
      
        }
    }

    private void Attack()
    {
        if (Vector3.Distance(target.position, transform.position) < attackRange && !isAttacking)
        {
            isAttacking = true;
            animator.PlayAttackAnimation();
            StartCoroutine(CheckIfPlayerWasHist());
        }
    }

    IEnumerator CheckIfPlayerWasHist()
    {
        yield return new WaitForSeconds(1.3f);
        EventsManager.Instance.OnPlayerHit(Vector3.Distance(target.position, transform.position) < attackRange && !animator.IsStuned);
        isAttacking = false;
    }

    private void FollowPlayer()
    {
        if(Vector3.Distance(target.position, transform.position) > navMeshAgent.stoppingDistance)
            navMeshAgent.SetDestination(target.position);
    }

    private void CheckIfPlayerIsOnAggressionRange()
    {
        if (Vector3.Distance(target.position, transform.position) < aggresionRange && !follow)
        {
            navMeshAgent.isStopped = false;
            follow = true;
            animator.PlayRunAnimation();
        }
    }

    private void Die()
    {
        isDead = true;
        collider.tag = "Untagged";
        DisableOutLine(false,transform);
        ChangeToDeadMaterial();
        healthUI.SetActive(false);
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
                
                enemyRigidbodies[i].AddForce( -enemyPrefab.transform.forward * flyAwayForce);
                enemyRigidbodies[i].AddForce(enemyPrefab.transform.up * flyAwayForce);
        }

        StartCoroutine(GoTroughFloor());

    }

    private void DisableRigidBody()
    {
        navMeshAgent.enabled = true;
        transform.GetComponent<Collider>().enabled = true;
        animator.Animator.enabled = true;
        foreach (var t in enemyRigidbodies)
        {
            t.useGravity = true;
            t.isKinematic = false;
        }
        foreach (var t in enemyColliders)
        {
            t.isTrigger = false;
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

    private void OnHit(Transform enemyHit, float power)
    {
        if (transform != enemyHit) return;
        healthUI.SetActive(true);
        hitPoints -= power;
        ShowDamageText(power);
        SetHealthFillBar();
        LeanTween.value(0, 0.5f, 0.1f).setOnUpdate((f => FlashMaterialsOnHit(f))).setLoopPingPong(1);
        if(hitPoints <= 0)
            Die();
        if (!navMeshAgent.enabled) return;
        StartCoroutine(StoppigPowerEffect());
    }

    IEnumerator StoppigPowerEffect()
    {

        navMeshAgent.speed = 1;
        animator.PlayReactionAnimation();
        yield return new WaitForSeconds(0.35f);
        navMeshAgent.speed = originalSpeed;
    }
    
    private void SetData()
    {
        aggresionRange = data.aggressionRange;
        hitPoints = data.hitPoints;
        attackRange = data.attackRange;
        if (useExtraForce)
            flyAwayForce += extraForce;

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
        yield return new WaitForSeconds(0.1f);
        navMeshAgent.isStopped = true;
        navMeshAgent.speed *= data.speed;
        originalSpeed = navMeshAgent.speed;
    }

    private void SetHealthFillBar()
    {
        healthFill.fillAmount = hitPoints / data.hitPoints;
    }

    private void ShowDamageText(float damage)
    {
        GameObject newDamageText =  ObjectPool.Instance.GetObjet(damageText);
        newDamageText.transform.parent = healthUI.transform;
        newDamageText.GetComponent<TextMeshProUGUI>().text = (-damage).ToString();
        newDamageText.transform.localPosition = new Vector3(0,500,0);
        newDamageText.transform.localScale = new Vector3(7.5f, 7.5f, 7.5f);
        newDamageText.transform.localRotation = Quaternion.Euler(Vector3.zero);
        newDamageText.transform.DOLocalMoveY(1200, 0.3f).SetEase(Ease.OutSine);
        ObjectPool.Instance.ReturnObject(newDamageText,0.4f);
    }

    private void FlashMaterialsOnHit(float emissiveIntensity)
    {
        
        foreach (Material material in materials)
        {
            material.SetColor(EmissiveColor, Color.white * emissiveIntensity  );
        }
    }

    private void ChangeToDeadMaterial()
    {
        foreach (Material material in materials)
        {
            material.color = Color.gray;
            
        }
    }

    public void EnableOutline(bool value,Transform enemyTransform)
    {
        
        if(enemyTransform is null || isDead) return;        
        outline.OutlineWidth = transform == enemyTransform ? 2 : 0;
    }

    public void DisableOutLine(bool value,Transform enemyTransform)
    {
        if(enemyTransform != null && transform == enemyTransform)
            outline.OutlineWidth = 0;
    }
    
    
    


}
