using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Elements")] 
    private GameObject prefab;
    private Transform prefabTransform;
    private Animator animator;
    [SerializeField] private Transform target;
    private Transform targetFolow;

    [Header("Settings")] 
    private string xVelocityParamter = "xVelocity";
    private string zVelocityParamter = "zVelocity";
    private string moveSpeedParamter = "moveSpeed";
    private string isRunningParamter = "isRunning";
    private string hasHandgunParameter = "hasHandGun";
    private string hasRifleParameter = "hasRifle";
    private string hasRocketLauncherParamter = "hasRocketLauncher";
    private string shootParamter = "Shoot";
    [SerializeField] private bool onTarget;
    Vector3 targetLookAt;
    private float runnigThreshold = 1f;
    private bool isArmed;
    private int currentWeaponLayer;

    public bool isOnTarget => onTarget;
    private void Awake()
    {
        prefab = transform.GetChild(0).gameObject;
        animator = prefab.GetComponent<Animator>();
        prefabTransform = prefab.transform;
    }

    private void Start()
    {
        EventsManager.Instance.eventSwitchedWepon += SwitchAnimationLayer;
        EventsManager.Instance.eventEnemyDeath += FreeTargetLocking;
        EventsManager.Instance.eventPlayerShoot += ShootAnimation;
        FreeTargetLocking();
    }


    private void OnDestroy()
    {
        EventsManager.Instance.eventSwitchedWepon -= SwitchAnimationLayer;
        EventsManager.Instance.eventEnemyDeath -= FreeTargetLocking;
        EventsManager.Instance.eventPlayerShoot -= ShootAnimation;
    }

    private void Update()
    {
        if (onTarget)
        {
            if (targetFolow is not null)
            {
                target.transform.position = targetFolow.transform.position;
            }
            
        }

    }   


    public void ManageAnimations(Vector3 movementVector)
    {

        float moveSpeed = movementVector.magnitude * 100;
        if (onTarget)
        {
            ResetVerticalAxis();
        }
        else
        {
            if(movementVector.magnitude > 0)
                targetLookAt = movementVector.normalized;
        }
        prefabTransform.forward = targetLookAt;
        if (onTarget)
        {
            if (isArmed)
            {
                AimWhileMoving(movementVector, moveSpeed);
            }
            else
            {
                animator.SetFloat(moveSpeedParamter,moveSpeed);
            }

        }
        else
        {
            animator.SetFloat(moveSpeedParamter,moveSpeed);
        }


    }

    private void ResetVerticalAxis()
    {
        targetLookAt  = (target.position - transform.position).normalized;
        targetLookAt.z = targetLookAt.z;
        targetLookAt.y = 0;
    }

    private void AimWhileMoving(Vector3 movementVector, float moveSpeed)
    {
        if (moveSpeed > 0)
        {
            animator.SetFloat(isRunningParamter,moveSpeed,.2f,Time.deltaTime);
        }
        else
        {
            animator.SetFloat(isRunningParamter,moveSpeed);
        }
            
        float xVelocity = Vector3.Dot(movementVector.normalized,prefabTransform.right);
        float zVelocity = Vector3.Dot(movementVector.normalized,prefabTransform.forward);
        animator.SetFloat(xVelocityParamter,xVelocity);
        animator.SetFloat(zVelocityParamter,zVelocity);
    }


    private void ShootAnimation()
    {
        animator.SetTrigger(shootParamter);
    }


    private void SwitchAnimationLayer(int weaponIdOnAnimationLayer, Transform weaponTransfrom = null)
    {
        TurnOffAllAnimationLayers();
        if (weaponIdOnAnimationLayer != 0)
        {
            isArmed = true;
            currentWeaponLayer = weaponIdOnAnimationLayer;
            if (onTarget)
            {
                SetLayerWeight(weaponIdOnAnimationLayer);
            }

            switch (weaponIdOnAnimationLayer)
            {
                case 1:
                    animator.SetBool(hasHandgunParameter,true);
                    animator.SetBool(hasRifleParameter,false);
                    animator.SetBool(hasRocketLauncherParamter,false);
                    break;
                case 3:
                    animator.SetBool(hasHandgunParameter,false);
                    animator.SetBool(hasRifleParameter,true);
                    animator.SetBool(hasRocketLauncherParamter,false);
                    break;
                case 5:
                    animator.SetBool(hasHandgunParameter,false);
                    animator.SetBool(hasRifleParameter,false);
                    animator.SetBool(hasRocketLauncherParamter,true);
                    break;
            }
        }
        else
        {
            onTarget = false;
        }

    }

    private void SetLayerWeight(int weaponIdOnAnimationLayer)
    {
        animator.SetLayerWeight(weaponIdOnAnimationLayer,1);
        animator.SetLayerWeight(weaponIdOnAnimationLayer+1,1);
    }

    private void TurnOffAllAnimationLayers()
    {
        isArmed = false;
        for (int i = 1; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i,0);
        }
    }

    public void LockToTarget(Transform newTarget)
    {
        if (!onTarget)
        {
            if (newTarget != null)
            {
                onTarget = true;
                targetFolow = newTarget;
                SetLayerWeight(currentWeaponLayer);
            }
        }
        else
        {
            if (Vector3.Distance(transform.position, newTarget.position) <
                Vector3.Distance(transform.position, target.position))
            {
                onTarget = true;
                targetFolow = newTarget;
            }
        }
        EventsManager.Instance.OnEnemyLockedIn(onTarget);
    }

    private void FreeTargetLocking(Transform enemyTransfrom = null)
    {
        if (enemyTransfrom is not null)
        {
            if (targetFolow.Equals(enemyTransfrom))
            {
                animator.SetFloat(moveSpeedParamter,0);
                onTarget = false;
                target = null;
                TurnOffAllAnimationLayers();
            }
        }
        EventsManager.Instance.OnEnemyLockedIn(onTarget);
    }
}
