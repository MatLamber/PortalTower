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
    private GameObject newObject;
    [SerializeField] private List<GameObject> optionPrefab;
    [SerializeField] private List<string> options;
    [SerializeField] private ParticleSystem crossingEffect;
    [SerializeField] private Transform optionContainer;
    
    private string starterPitolName = "StarterPistol";
    private string pistolName = "Pistol";
    private string rifleName = "Rifle";
    private string rocketLauncherlName = "RocketLauncher";
    private string shotgunName = "Shorty";


    private int currentSelection;

    private void Start()
    {
        optionContainer.transform.DORotate(new Vector3(0, 360, 0), 2, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
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
        ObjectPool.Instance.ReturnObject(newObject,0);
        transform.DOScale(Vector3.zero,0.3f).SetDelay(0.3f);

    }

    private void ShowDoor()
    { 
        newObject =  ObjectPool.Instance.GetObjet(optionPrefab[currentSelection]);
        newObject.transform.SetParent(optionContainer.transform);
        newObject.transform.localPosition = Vector3.zero;
        newObject.transform.localScale = new Vector3(1, 1, 1);
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
            EventsManager.Instance.OnSelectedOption(options[currentSelection]);
            currentSelection++;
            EventsManager.Instance.OnTeleportPlayer();

        }
    
    }
}
