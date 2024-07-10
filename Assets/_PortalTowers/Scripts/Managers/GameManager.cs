using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Header("Elements")] 
    [SerializeField] private List<GameObject> scenarios;
    [SerializeField] private Transform enemyStartPoint; // Deberia ser un punto dentro del escenario que sirva como lugar de spawn para enemigos
    [SerializeField] private EnemySetupCollection enemiesPerLevel; // Tiene una lista con todas las combinaciones de enemigos para cada nivel
    private EnemySetup enemiesForCurrentLevel;

    [Header("Settings")] 
    private int currentScenario = 0;
    private float distanceBetweenScnearios = 27.14f;
    private int currentLevel;


    private void Start()
    {
        SpawnEnemies();
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


    private void SpawnEnemies()
    {

        enemiesForCurrentLevel = enemiesPerLevel.enemiesSetup[currentLevel];
        foreach (Enemy enemy in enemiesForCurrentLevel.enemies)
        {
            Instantiate(enemy.prefab, enemyStartPoint.position, Quaternion.Euler(0,180,0));
        }

        currentLevel++;
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