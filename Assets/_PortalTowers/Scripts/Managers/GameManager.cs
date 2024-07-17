using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesOnLevel;
    [SerializeField] private List<Transform> playerStartPoints;
    [SerializeField] private List<Transform> enemyStartPoints;
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> floors;
    [SerializeField] private EnemySetupCollection enemyProgression;
    [SerializeField] private GameObject portalsContainer;
    
    private List<EnemySetup> enemiesSetupList = new List<EnemySetup>();
    private List<GameObject> enemiesOnNextLevel = new List<GameObject>();
    private int currentScenario;
    private int currentLevel;
    private bool firstSpawn;

    private Vector3 localPortalsPosition = new Vector3(-2.5f, -0.5f, -3.7f);

    private void Start()
    {
        foreach (EnemySetup enemiesSetup in enemyProgression.enemiesSetup)
            enemiesSetupList.Add(enemiesSetup);
        SpawnEnemies();
        EventsManager.Instance.eventEnemyDeath += RemoveEnemyFromLevel;
        EventsManager.Instance.eventTeleportPlayer += TeleportPlayer;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventEnemyDeath -= RemoveEnemyFromLevel;
        EventsManager.Instance.eventTeleportPlayer -= TeleportPlayer;

    }
    
    private void RemoveEnemyFromLevel(Transform enemy)
    {
        if (enemiesOnLevel.Contains(enemy.gameObject))
            enemiesOnLevel.Remove(enemy.gameObject);
        CheckLevelStatus();
    }

    private void CheckLevelStatus()
    {
        if (enemiesOnLevel.Count > 0) return;
        foreach (GameObject enemies in enemiesOnNextLevel)
            enemiesOnLevel.Add(enemies);
        currentLevel++;
        StartCoroutine(LevelFinishDelay());
    }

    IEnumerator LevelFinishDelay()
    {
        yield return new WaitForSeconds(0.2f);
        EventsManager.Instance.OnLevelFinish();
        SpawnEnemies();
    }
    private void TeleportPlayer()
    {
        Joystick.Instance.Teleporting = true;
        Joystick.Instance.HideJoystick();
        player.transform.position = playerStartPoints[currentLevel].position;
        Joystick.Instance.Teleporting = false;
        currentScenario++;
        SetPortalsAsChildOfCurrentFloor(currentScenario);
    }
    private void SpawnEnemies()
    {
        if (currentLevel >= enemiesSetupList.Count)
            currentLevel = Random.Range(0, enemiesSetupList.Count - 1);
        
        if (currentLevel == 0)
            enemiesOnLevel =  SpawnEnemiesFromList(enemiesSetupList[currentLevel].enemies,currentLevel,currentScenario);
        enemiesOnNextLevel = SpawnEnemiesFromList(enemiesSetupList[currentLevel + 1].enemies, currentLevel + 1,currentScenario+1); 

    }

    private List<GameObject> SpawnEnemiesFromList(List<Enemy> listOfEnemies, int levelIndex, int scenarioIndex)
    {
        List<GameObject> resultingEnemyList = new List<GameObject>();
        foreach (Enemy enemy in listOfEnemies)
        {
            GameObject newEnemy = Instantiate(enemy.prefab, enemyStartPoints[levelIndex].transform.position,
                quaternion.LookRotation(-player.transform.forward, player.transform.up),
                floors[scenarioIndex].transform);
            newEnemy.GetComponent<EnemyController>().Target = player.transform;
            resultingEnemyList.Add(newEnemy);
        }

        return resultingEnemyList;
    }

    private void SetPortalsAsChildOfCurrentFloor(int currentFloor)
    {
        StartCoroutine(SetPortalsAsChildOfCurrentFloorDelay(0.8f));
    }

    IEnumerator SetPortalsAsChildOfCurrentFloorDelay(float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        portalsContainer.transform.parent = floors[currentScenario].transform;
        portalsContainer.transform.localPosition = localPortalsPosition;
    }
}