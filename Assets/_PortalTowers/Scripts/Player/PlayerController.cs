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

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<PlayerAnimator>();
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
}
