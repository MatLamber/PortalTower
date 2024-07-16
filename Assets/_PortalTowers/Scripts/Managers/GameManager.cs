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
    private List<EnemySetup> enemiesSetupList = new List<EnemySetup>();
    private List<GameObject> enemiesOnNextLevel = new List<GameObject>();
    private int currentScenario;
    private int currentLevel;
    private bool firstSpawn;

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
        {
            enemiesOnLevel.Add(enemies);
        }
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
        player.transform.position = playerStartPoints[1].position;
        Joystick.Instance.Teleporting = false;
    }

    private void SpawnEnemies()
    {
        if (currentLevel >= enemiesSetupList.Count)
            currentLevel = Random.Range(0, enemiesSetupList.Count - 1);
        
        if (currentLevel == 0)
        {
            foreach (Enemy enemy in enemiesSetupList[currentLevel].enemies)
            {
                GameObject newEnemy = Instantiate(enemy.prefab, enemyStartPoints[currentLevel].transform.position,
                    quaternion.LookRotation(-player.transform.forward, player.transform.up),
                    floors[currentScenario].transform);
                newEnemy.GetComponent<EnemyController>().Target = player.transform;
                enemiesOnLevel.Add(newEnemy);
            }
        }
        foreach (Enemy enemy in enemiesSetupList[currentLevel + 1].enemies)
        {
            GameObject newEnemy = Instantiate(enemy.prefab, enemyStartPoints[currentLevel + 1].transform.position,
                quaternion.LookRotation(-player.transform.forward, player.transform.up),
                floors[currentScenario].transform);
            newEnemy.GetComponent<EnemyController>().Target = player.transform;
            enemiesOnNextLevel.Add(newEnemy);
        }
    }
}