using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController),typeof(PlayerAnimator),typeof(PlayerWeaponController))]
public class PlayerController : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private GameObject healthUI;
    [SerializeField] private Image healthFill;
    [SerializeField] private GameObject damageText;
    private CharacterController characterController;
    private PlayerAnimator animator;


    [Header("Settings")] 
    [SerializeField] private float moveSpeed;

    private string enemyTagName = "Enemy";

    private float hitPoints = 10f;
    private float totalHitPoints = 10f;
    

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<PlayerAnimator>();
        characterController.slopeLimit = 0;
    }

    private void Start()
    {
        EventsManager.Instance.eventJoyStrickMove += Move;
        EventsManager.Instance.eventPlayerHit += OnHit;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventJoyStrickMove -= Move;
        EventsManager.Instance.eventPlayerHit -= OnHit;
    }

    private void OnHit(bool wasHit)
    {
        if (wasHit)
        {
            hitPoints--;
            healthUI.SetActive(true);
            healthFill.fillAmount = hitPoints / totalHitPoints;
            ShowDamageText(1);
            Debug.Log(hitPoints.ToString());
        }


    }

    private void Move(Vector3 moveVector)
    {
        Vector3 movementVector = moveVector * (moveSpeed * Time.deltaTime) / Screen.width;
        movementVector.z = movementVector.y;
        movementVector.y = 0;
        characterController.Move(movementVector);
        animator.ManageAnimations(movementVector);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag.Equals(enemyTagName))
            animator.LockToTarget(other.transform);
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
}
