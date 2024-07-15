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
        EventsManager.Instance.eventLevelFinish += EnableGeneralCamera;
        EventsManager.Instance.eventTeleportPlayer += EnablePlayerCamera;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventLevelFinish -= EnableGeneralCamera;
        EventsManager.Instance.eventTeleportPlayer -= EnablePlayerCamera;
    }


    private void EnableGeneralCamera()
    {
        playerCamera.Priority = 0;
        generalCamera.Priority = 1;
    }

    private void EnablePlayerCamera()
    {
        playerCamera.Priority = 1;
        generalCamera.Priority = 0;
    }
    
}
