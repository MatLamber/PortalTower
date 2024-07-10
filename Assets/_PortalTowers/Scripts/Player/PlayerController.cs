using System;
using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(PlayerAnimator))]
public class PlayerController : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform nextStartPosition;
    private PlayerAnimator playerAnimator;
    
    
    [Header("Settings")] 
    [SerializeField] private float moveSpeed;
    private string enemyTag = "Enemy";
    private string doorTag = "Door";
    private bool enemyLocked;
    private Transform enemyTransform;


    private void Awake()
    {
        playerAnimator = GetComponent<PlayerAnimator>();
    }

    private void Start()
    {
        EventsManager.Instance.ActionJoystickMove += Move;
        EventsManager.Instance.ActionEnemyKilled += OnEnemyOutOfRange;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionJoystickMove -= Move;
        EventsManager.Instance.ActionEnemyKilled -= OnEnemyOutOfRange;
    }


    private void Update()
    {
        if (!enemyLocked) return;
        enemyLocked = true;
        EventsManager.Instance.OnEnemyOnRange(enemyTransform);
    }

    private void Move(Vector3 move)
    {
        Vector3 moveVector = move * (moveSpeed * Time.deltaTime) / Screen.width;
        moveVector.z = moveVector.y;
        moveVector.y = 0;
        characterController.Move(moveVector);
        playerAnimator.ManageAnimations(moveVector);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(enemyTag) && !enemyLocked)
        {
            enemyLocked = true;
            enemyTransform = other.transform;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Collider colliderHit = hit.collider;
        if (colliderHit.tag.Equals(doorTag))
        {
            transform.position = nextStartPosition.position;
            EventsManager.Instance.OnDoorCrossed();
        }
    }

    private void OnEnemyOutOfRange(GameObject enemyContainer = null)
    {
        enemyLocked = false;
        EventsManager.Instance.OnEnemyOutOfRange();
    }
}