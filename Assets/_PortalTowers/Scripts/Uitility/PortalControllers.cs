using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PortalControllers : MonoBehaviour
{
    private string playerTagName = "Player";
    private bool doorCrossed;
    private bool canBeCrossed;

    [SerializeField] private ParticleSystem crossingEffect;

    private void Start()
    {
        EventsManager.Instance.eventLevelFinish += ShowDoor;
        EventsManager.Instance.eventTeleportPlayer += HideDoor;
    }


    private void OnDestroy()
    {
        EventsManager.Instance.eventLevelFinish -= ShowDoor;
        EventsManager.Instance.eventTeleportPlayer -= HideDoor;
    }


    private void HideDoor()
    {
        canBeCrossed = false;
        doorCrossed = false;
        transform.DOScale(Vector3.zero,0.3f).SetDelay(0.3f);

    }

    private void ShowDoor()
    {
        transform.DOScale(new Vector3(1,1,1),0.3f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(() =>
        {
            canBeCrossed = true;
        });

    }

    private void OnTriggerEnter(Collider other)
    {
        if(!canBeCrossed) return;
        if (other.tag.Equals(playerTagName) && !doorCrossed )
        {
            crossingEffect.Play();
            doorCrossed = true;
            EventsManager.Instance.OnTeleportPlayer();
        }
    
    }
}
