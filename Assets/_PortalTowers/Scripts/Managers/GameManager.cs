using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesOnLevel;
    [SerializeField] private List<Transform>  playerStartPoints;
    [SerializeField] private List<Transform>  enemyStartPoints;
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> floors;
    [SerializeField] private EnemySetupCollection enemyProgression;
    private List<EnemySetup> enemiesSetupList = new List<EnemySetup>();
    private int currentScenario;
    private int currentLevel;
    private bool firstSpawn;
    private void Start()
    {
        foreach (EnemySetup enemiesSetup in enemyProgression.enemiesSetup)
            enemiesSetupList.Add(enemiesSetup);
        SetPlayerAndEnemiesIntheCurrentFloor();
        SpawnEnemies();
        EventsManager.Instance.eventEnemyDeath += RemoveEnemyFromLevel;
        EventsManager.Instance.eventTeleportPlayer += TeleportPlayer;
        EventsManager.Instance.eventLevelFinish += SpawnEnemies;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventEnemyDeath -= RemoveEnemyFromLevel;
        EventsManager.Instance.eventTeleportPlayer -= TeleportPlayer;
        EventsManager.Instance.eventLevelFinish -= SpawnEnemies;
    }
    
    private void SetPlayerAndEnemiesIntheCurrentFloor()
    {
        player.transform.parent = floors[currentScenario].transform;
        foreach (GameObject enemy in enemiesOnLevel)
            enemy.transform.parent = floors[currentScenario].transform;
    }

    private void RemoveEnemyFromLevel(Transform enemy)
    {
        if (enemiesOnLevel.Contains(enemy.gameObject))
        {
            enemiesOnLevel.Remove(enemy.gameObject);
        }
        CheckLevelStatus();
    }

    private void CheckLevelStatus()
    {
        if (enemiesOnLevel.Count == 0)
        {
            if (currentScenario == 0)
            {
                currentScenario = 1;
            }
            else if (currentScenario == 1)
            {
                currentScenario = 0;
            }
            
            currentLevel++;
            EventsManager.Instance.OnLevelFinish();
        }

    }


    private void TeleportPlayer()
    {
        Joystick.Instance.Teleporting = true;
        Joystick.Instance.HideJoystick();
        Debug.Log($"{playerStartPoints[currentScenario]}");
        player.transform.position = playerStartPoints[1].position;
        Joystick.Instance.Teleporting = false;
        SetPlayerAndEnemiesIntheCurrentFloor();
        Invoke(nameof(ChangeFloorPositions),0.7f);
        
    }


    private void ChangeFloorPositions()
    {
        foreach (GameObject enemy in enemiesOnLevel)
            enemy.GetComponent<NavMeshAgent>().enabled = false;
        if (currentScenario == 0)
        {
            Vector3 auxPosition = floors[0].transform.position;
            floors[0].transform.position = floors[1].transform.position;
            floors[1].transform.position = auxPosition;
        }
        else
        {
            Vector3 auxPosition = floors[1].transform.position;
            floors[1].transform.position = floors[0].transform.position;
            floors[0].transform.position = auxPosition;
        }

        foreach (GameObject enemy in enemiesOnLevel)
            enemy.GetComponent<NavMeshAgent>().enabled = true;
        


    }


    private void SpawnEnemies()
    {
        foreach (Enemy enemy in enemiesSetupList[currentLevel].enemies)
        {
            int index = currentScenario;
            if (index == 0 && firstSpawn)
            {
                index = 1;
            }
 
           GameObject newEnemy = Instantiate(enemy.prefab, enemyStartPoints[index].transform.position,
               quaternion.LookRotation(-player.transform.forward, player.transform.up));
           newEnemy.GetComponent<EnemyController>().Target = player.transform;
            /* GameObject newEnemy = ObjectPool.Instance.GetObjet(enemy.prefab);
            newEnemy.transform.position = enemyStartPoints[index].transform.position;
            newEnemy.transform.rotation = quaternion.LookRotation(-player.transform.forward,newEnemy.transform.up);
            newEnemy.GetComponent<EnemyController>().Target = player.transform;*/
            enemiesOnLevel.Add(newEnemy);
            if (!firstSpawn)
                firstSpawn = true;

        }
    }
}
