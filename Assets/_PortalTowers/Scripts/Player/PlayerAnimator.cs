using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Elements")] [SerializeField] private Animator playerAnimator;
    [SerializeField] private Transform rendererTransform;

    [Header("Settings")] private string walkParameter = "Walk";
    private string runParameter = "Run";
    private string idleParameter = "Idle";
    private string idleSpeedParameter = "IdleSpeed";
    private float idleValue = 0f;
    private float walkValue = 0.005f;

    private Vector3 forwardLookAt;
    
    private bool enemyOnRange;
    private bool combatTween;


    private void Start()
    {
        EventsManager.Instance.ActionEnemyOnRange += OnEnemyOnRange;
        EventsManager.Instance.ActionEnemyOutOfRange += OnEnemyOutRange;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionEnemyOnRange -= OnEnemyOnRange;
        EventsManager.Instance.ActionEnemyOutOfRange -= OnEnemyOutRange;
    }

    public void ManageAnimations(Vector3 move)
    {
        if(!enemyOnRange)
            forwardLookAt = move.normalized;
        if (move.magnitude > idleValue && move.magnitude < walkValue)
        {
            PlayWalkAnimation();
            if(!enemyOnRange)
                SetForwardPosition(forwardLookAt);
        }
        else if (move.magnitude > idleValue && move.magnitude > walkValue)
        {
            PlayRunAnimation();
            if(!enemyOnRange)
                SetForwardPosition(forwardLookAt);
        }
        else if (move.magnitude <= idleValue)
        {
            PlayIdleAnimation();
        }
    }

    private void PlayWalkAnimation()
    {
        playerAnimator.SetBool(idleParameter, false);
        playerAnimator.SetBool(runParameter, false);
        playerAnimator.SetBool(walkParameter, true);
    }

    private void PlayRunAnimation()
    {
        playerAnimator.SetBool(idleParameter, false);
        playerAnimator.SetBool(walkParameter, false);
        playerAnimator.SetBool(runParameter, true);
    }

    private void PlayIdleAnimation()
    {
        playerAnimator.SetBool(walkParameter, false);
        playerAnimator.SetBool(runParameter, false);
        playerAnimator.SetBool(idleParameter, true);
    }

    private void SetUpperLayerBody()
    {
        if (combatTween) return;
        combatTween = true;
        DOVirtual.Float(0, 1, 0.3f, value => playerAnimator.SetLayerWeight(1, value));
        playerAnimator.SetFloat(idleSpeedParameter,0.00001f);

    }

    private void SetDefaultLayerBody()
    {
        if (!combatTween) return;
        combatTween = false;
        DOVirtual.Float(1, 0, 0.3f, value => playerAnimator.SetLayerWeight(1, value));
        playerAnimator.SetFloat(idleSpeedParameter,1);
    }

    private void SetForwardPosition(Vector3 forwardVector)
    {
        rendererTransform.forward = forwardVector;
    }

    private void OnEnemyOnRange(Transform enemyTransform)
    {
        Debug.Log($"SET NEW FOWARD POSITION");
        SetUpperLayerBody();
        enemyOnRange = true;
        Vector3 direction = (enemyTransform.position - rendererTransform.position);
        direction.y = 0 ;
        forwardLookAt = direction.normalized;
        SetForwardPosition(forwardLookAt);
    }

    private void OnEnemyOutRange()
    {
        SetDefaultLayerBody();
        enemyOnRange = false;
    }
    
}