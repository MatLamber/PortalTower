using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemieBoss : MonoBehaviour
{
    [SerializeField] private List<Renderer> bodyParts;
    private List<Material> materials = new List<Material>();
    private static readonly int EmissiveColor = Shader.PropertyToID("_EmissionColor");
    private Animator animator => GetComponent<Animator>();
    //private string 
    
    private void Start()
    {
        foreach (Renderer bodyPart in bodyParts)
        {
            materials.Add(bodyPart.material);
        }
        EventsManager.Instance.eventLevelFinish += MoveForward;
     //   EventsManager.Instance.eventEnemyHit += OnHit;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventLevelFinish -= MoveForward;
       // EventsManager.Instance.eventEnemyHit -= OnHit;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
            OnHit(transform,0);
    }

    private void MoveForward(bool canMove)
    {
        if (canMove)
            transform.DOLocalMoveZ(82, 20).SetEase(Ease.Linear);
    }
    
    private void OnHit(Transform enemyHit, float power)
    {

        if (transform != enemyHit) return;
            DOVirtual.Color(Color.red, Color.white, 0.05f,ColorChange).SetLoops(2,LoopType.Yoyo).SetEase(Ease.Linear);

    }

    private void ColorChange(Color changedColor)
    {
        foreach (var material in materials)
        {
            material.color = changedColor;
        }
    }
    

}