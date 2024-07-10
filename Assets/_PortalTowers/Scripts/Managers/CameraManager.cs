using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCamera;
    [SerializeField] private CinemachineVirtualCamera generalCamera;


    private void Start()
    {
        EventsManager.Instance.ActionEnemyKilled += EnableGeneralCamera;
        EventsManager.Instance.ActionDoorCrossed += OnDoorCrossed;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionEnemyKilled -= EnableGeneralCamera;
        EventsManager.Instance.ActionDoorCrossed -= OnDoorCrossed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) 
            EnablePlayerCamera();
        if(Input.GetKeyDown(KeyCode.S))
            EnableGeneralCamera();
    }

    private void EnablePlayerCamera()
    {
        playerCamera.Priority = 1;
        generalCamera.Priority = 0;
    }


    private void OnDoorCrossed()
    {
        Invoke(nameof(EnablePlayerCamera),0.2f);
    }

    private void EnableGeneralCamera(GameObject enemyContainer = null)
    {
        playerCamera.Priority = 0;
        generalCamera.Priority = 1;
    }
}
