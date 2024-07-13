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

    [Header("Settings")] 
    [SerializeField] private int handGunLayer;
    [SerializeField] private int rifleLayer;
    [SerializeField] private int rocketLauncherLayer;
    private string xVelocityParamter = "xVelocity";
    private string zVelocityParamter = "zVelocity";
    private string moveSpeedParamter = "moveSpeed";
    private string isRunningParamter = "isRunning";
    private string shootParamter = "Shoot";
    private bool onTarget;
    Vector3 targetLookAt;
    private float runnigThreshold = 1f;


    private void Awake()
    {
        prefab = transform.GetChild(0).gameObject;
        animator = prefab.GetComponent<Animator>();
        prefabTransform = prefab.transform;
    }

    private void Start()
    {
        EventsManager.Instance.eventPlayerShoot += ShootAnimation;
        EventsManager.Instance.eventSwitchedWepon += SwitchAnimationLayer;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventPlayerShoot -= ShootAnimation;
        EventsManager.Instance.eventSwitchedWepon += SwitchAnimationLayer;
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
            AimWhileMoving(movementVector, moveSpeed);
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


    private void SwitchAnimationLayer(int weaponIdOnAnimationLayer)
    {
        TurnOffAllAnimationLayers();
        if (weaponIdOnAnimationLayer != 0)
        {
            onTarget = true;
            animator.SetLayerWeight(weaponIdOnAnimationLayer,1);
            animator.SetLayerWeight(weaponIdOnAnimationLayer+1,1);
        }
        else
        {
            onTarget = false;
        }

    }

    private void TurnOffAllAnimationLayers()
    {
        for (int i = 1; i < animator.layerCount; i++)
        {
            animator.SetLayerWeight(i,0);
        }
    }
}
