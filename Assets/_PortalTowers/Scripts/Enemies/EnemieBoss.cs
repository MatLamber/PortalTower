using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemieBoss : MonoBehaviour
{
    [SerializeField] private List<Renderer> bodyParts;
    private List<Material> materials = new List<Material>();
    private void Start()
    {
        EventsManager.Instance.eventLevelFinish += MoveForward;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventLevelFinish -= MoveForward;
    }

    private void MoveForward(bool canMove)
    {
        if (canMove)
            transform.DOLocalMoveZ(82, 20).SetEase(Ease.Linear);
    }
}