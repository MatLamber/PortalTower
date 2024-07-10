using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private List<GameObject> scenarios;
    [SerializeField] private Transform enemyStartPoint; // Deberia ser un punto dentro del escenario que sirva como lugar de spawn para enemigos;
    

    [Header("Settigns")] private int currentScenario = 0;
    private float distanceBetweenScnearios = 27.14f;


    private void Start()
    {
        EventsManager.Instance.ActionDoorCrossed += MoveUpScenario;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionDoorCrossed -= MoveUpScenario;
    }

    private void MoveUpScenario()
    {
        StartCoroutine(MoveUpCoroutine());
    }

    IEnumerator MoveUpCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        Vector3 currentPosition = scenarios[currentScenario].transform.position;

        scenarios[currentScenario].transform.position = new Vector3(currentPosition.x,
            currentPosition.y + distanceBetweenScnearios, currentPosition.z);
        currentScenario = currentScenario == 0 ? 1 : 0;
    }
    
}