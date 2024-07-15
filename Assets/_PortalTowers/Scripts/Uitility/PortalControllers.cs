using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PortalControllers : MonoBehaviour
{
    private GameObject portalVfx => transform.GetChild(0).gameObject;
    private string playerTagName = "Player";
    private bool doorCrossed;

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
        doorCrossed = false;
        portalVfx.GetComponent<ParticleSystem>().Stop();
        transform.DOScale(Vector3.zero,0.3f);

    }

    private void ShowDoor()
    {
        transform.DOScale(new Vector3(1,1,1),0.3f).SetEase(Ease.OutBack).SetDelay(0.3f).OnComplete(() =>
        {
            portalVfx.GetComponent<ParticleSystem>().Play();
        });

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals(playerTagName) && !doorCrossed)
        {
            doorCrossed = true;
            EventsManager.Instance.OnTeleportPlayer();
        }
    
    }
}
