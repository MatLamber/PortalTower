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
    private List<GameObject> currentEnemies = new List<GameObject>();

    [Header("Settings")] 
    private int currentScenario = 0;
    private float distanceBetweenScnearios = 27.14f;
    private int currentLevel;


    private void Start()
    {
        SpawnEnemies();
        EventsManager.Instance.ActionDoorCrossed += MoveUpScenario;
        EventsManager.Instance.ActionEnemyKilled += OnEnemyKilled;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.ActionDoorCrossed -= MoveUpScenario;
        EventsManager.Instance.ActionEnemyKilled -= OnEnemyKilled;
    }

    private void MoveUpScenario()
    {
        StartCoroutine(MoveUpCoroutine());
    }
    
    private void SpawnEnemies()
    {
        enemiesForCurrentLevel = enemiesPerLevel.enemiesSetup[currentLevel];
        int counter = 0;
        foreach (Enemy enemy in enemiesForCurrentLevel.enemies)
        {
            GameObject instantiatedEnemy =  Instantiate(enemy.prefab, enemyStartPoint.position, Quaternion.Euler(0,180,0));
            instantiatedEnemy.name += counter.ToString();
            currentEnemies.Add(instantiatedEnemy);
            counter++;
        }
        currentLevel++;
    }

    private void OnEnemyKilled(GameObject enemyContainer)
    {
        currentEnemies.Remove(enemyContainer);
        Debug.Log($"Number of enemies left{currentEnemies.Count}");
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