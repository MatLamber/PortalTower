using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController),typeof(PlayerAnimator),typeof(PlayerWeaponController))]
public class PlayerController : MonoBehaviour
{
    [Header("Elements")]
    private CharacterController characterController;
    private PlayerAnimator animator;


    [Header("Settings")] 
    [SerializeField] private float moveSpeed;

    private string enemyTagName = "Enemy";

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<PlayerAnimator>();
        characterController.slopeLimit = 0;
    }

    private void Start()
    {
        EventsManager.Instance.eventJoyStrickMove += Move;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventJoyStrickMove -= Move;
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
}
