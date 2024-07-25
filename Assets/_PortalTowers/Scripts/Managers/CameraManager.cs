using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
   [SerializeField] private CinemachineVirtualCamera playerCamera;
   [SerializeField] private CinemachineVirtualCamera generalCamera;
   [SerializeField] private CinemachineVirtualCamera finalBossCamera;
   private bool onFinalBoss;
    
    private void Start()
    {
        EventsManager.Instance.eventLevelFinish += EnableGeneralCamera;
        EventsManager.Instance.eventTeleportPlayer += EnablePlayerCamera;
        EventsManager.Instance.eventTeleportPlayer += MoveGeneralCameraUp;
    }



    private void OnDestroy()
    {
        EventsManager.Instance.eventLevelFinish -= EnableGeneralCamera;
        EventsManager.Instance.eventTeleportPlayer -= EnablePlayerCamera;
        EventsManager.Instance.eventTeleportPlayer -= MoveGeneralCameraUp;
    }


    private void EnableGeneralCamera(bool lastFloor)
    {
        onFinalBoss = lastFloor;
        StartCoroutine(GeneralCameraDelay());
    }

    IEnumerator GeneralCameraDelay()
    {
        yield return new WaitForSeconds(0.3f);
        playerCamera.Priority = 0;
        generalCamera.Priority = 1;
    }

    private void EnablePlayerCamera(int id)
    {
        StartCoroutine(onFinalBoss ? FinalBossCameraDelay() : PlayerCameraDelay());
    }
 
    IEnumerator PlayerCameraDelay()
    {
        yield return new WaitForSeconds(1.2f);
        CinemachineTransposer transposer = playerCamera.GetCinemachineComponent<CinemachineTransposer>();
        transposer.m_FollowOffset.z += 5.6f;
        playerCamera.Priority = 1;
        generalCamera.Priority = 0;
    }
    
    IEnumerator FinalBossCameraDelay()
    {
        yield return new WaitForSeconds(1.2f);
        finalBossCamera.Priority = 1;
        playerCamera.Priority = 0;
        generalCamera.Priority = 0;
    }
    
    private void MoveGeneralCameraUp(int obj)
    {
        generalCamera.transform.DOMoveY(generalCamera.transform.position.y + 8, 2f).SetEase(Ease.OutBack).SetDelay(0.3f);
        generalCamera.transform.DOMoveZ(generalCamera.transform.position.z + 5.6f, 2f).SetEase(Ease.OutBack).SetDelay(0.3f);
    }
    
    


    
}
