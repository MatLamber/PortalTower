using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> enemiesOnLevel;
    [SerializeField] private List<GameObject> playerStartPoints;
    [SerializeField] private List<Transform> enemyStartPoints;
    [SerializeField] private GameObject player;
    [SerializeField] private List<GameObject> floors;
    [SerializeField] private EnemySetupCollection enemyProgression;
    [SerializeField] private GameObject portalsContainer;
    [SerializeField] private List<GameObject> portalFx;
    [SerializeField] private GameObject finalBoss;

    private List<EnemySetup> enemiesSetupList = new List<EnemySetup>();
    private List<GameObject> enemiesOnNextLevel = new List<GameObject>();
    private int currentScenario;
    private int currentLevel;
    private bool firstSpawn;

    private Vector3 localPortalsPosition = new Vector3(-2.5f, -0.5f, -3.7f);

    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        foreach (EnemySetup enemiesSetup in enemyProgression.enemiesSetup)
            enemiesSetupList.Add(enemiesSetup);
        //SpawnEnemies();
        for (int i = 0; i < enemiesSetupList.Count; i++)
        {
            if (i == 0)
            {
                enemiesOnLevel = SpawnEnemiesFromList(enemiesSetupList[i].enemies, i, i);
            }
            else if (i == 1)
            {
                enemiesOnNextLevel = SpawnEnemiesFromList(enemiesSetupList[i].enemies, i, i);
            }
            else
            {
                SpawnEnemiesFromList(enemiesSetupList[i].enemies, i, i);
            }
               
 
        }
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
        playerStartPoints[currentLevel - 1].transform.DOScale(new Vector3(1, 1, 1), 0.4f).SetDelay(0.4f);
        StartCoroutine(LevelFinishDelay());
    }

    IEnumerator LevelFinishDelay()
    {
        yield return new WaitForSeconds(0.2f);
        EventsManager.Instance.OnLevelFinish(currentLevel >= enemiesSetupList.Count);
       // SpawnEnemies();
    }

    private void TeleportPlayer(int id)
    {
        Joystick.Instance.Teleporting = true;
        Joystick.Instance.HideJoystick();
        LeanTween.scale(playerPrefab, new Vector3(0.0001f, 0.0001f), 0.3f).setOnComplete((() =>
        {
            player.transform.position =
                playerStartPoints[currentLevel - 1].transform.GetChild(id).GetChild(0).position + new Vector3(0, -1, 2);
            //  GameObject newPortalFx = ObjectPool.Instance.GetObjet(portalFx[0]);
            // newPortalFx.transform.position = player.transform.position + new Vector3(0, 1, 0);
            // newPortalFx.transform.DOScale(new Vector3(1, 1, 1), 0.3f).SetEase(Ease.OutBack);
            // newPortalFx.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.OutBack).SetDelay(1.5f).OnComplete((() => ObjectPool.Instance.ReturnObject(newPortalFx, 0)));
        }));
        LeanTween.scale(playerPrefab, new Vector3(1, 1, 1), 0.3f).setDelay(0.5f)
            .setOnComplete((() =>
            {
                Joystick.Instance.Teleporting = false;
                playerStartPoints[currentLevel - 1].transform.GetChild(0).DOLocalMoveY(-1.75f, 0.4f).OnComplete((() =>
                    playerStartPoints[currentLevel - 1].transform.GetChild(0).gameObject.SetActive(false)));
                playerStartPoints[currentLevel - 1].transform.GetChild(1).DOLocalMoveY(-1.75f, 0.4f).OnComplete((() =>
                    playerStartPoints[currentLevel - 1].transform.GetChild(1).gameObject.SetActive(false)));
            }));
        playerPrefab.transform.DORotate(new Vector3(360, 360, 360), 0.5f, RotateMode.FastBeyond360);
        playerPrefab.transform.DOLocalMoveY(3, 0.5f)
            .OnComplete((() => player.transform.GetChild(0).DOLocalMoveY(0, 0.3f)));
        currentScenario++;
        if (currentLevel >= enemiesSetupList.Count)
        {
            player.GetComponent<PlayerController>().LockOnFinalsBoss(finalBoss.transform);
        }

        SetPortalsAsChildOfCurrentFloor(currentScenario);
    }

    private void SpawnEnemies()
    {
        if (currentLevel >= enemiesSetupList.Count) return;
        if (currentLevel == 0)
            enemiesOnLevel =
                SpawnEnemiesFromList(enemiesSetupList[currentLevel].enemies, currentLevel, currentScenario);
        if (currentLevel + 1 < enemiesSetupList.Count)
            enemiesOnNextLevel = SpawnEnemiesFromList(enemiesSetupList[currentLevel + 1].enemies, currentLevel + 1,
                currentScenario + 1);
    }

    private List<GameObject> SpawnEnemiesFromList(List<Enemy> listOfEnemies, int levelIndex, int scenarioIndex)
    {
        List<GameObject> resultingEnemyList = new List<GameObject>();
        foreach (Enemy enemy in listOfEnemies)
        {
            GameObject newEnemy = Instantiate(enemy.prefab, enemyStartPoints[levelIndex].transform.position,
                quaternion.LookRotation(-player.transform.forward, player.transform.up));
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