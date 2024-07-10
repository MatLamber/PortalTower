using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    private static EventsManager instance;
    
    public static EventsManager Instance
    {
        get
        {
            if (instance != null) return instance;
            GameObject go = new GameObject("EventsManager");
            instance = go.AddComponent<EventsManager>();
            DontDestroyOnLoad(go);
            return instance;
        }
    }
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public Action<Vector3> ActionJoystickMove;
    public Action<Transform> ActionEnemyOnRange;
    public Action ActionEnemyOutOfRange;
    public Action <GameObject> ActionEnemyKilled;
    public Action ActionDoorCrossed;
    public void OnJoystickMove(Vector3 move) => ActionJoystickMove?.Invoke(move);
    public void OnEnemyOnRange(Transform enemyRange) => ActionEnemyOnRange?.Invoke(enemyRange);
    public void OnEnemyOutOfRange() => ActionEnemyOutOfRange?.Invoke();
    public void OnEnemyKilled(GameObject enemy) => ActionEnemyKilled.Invoke(enemy);
    public void OnDoorCrossed() => ActionDoorCrossed?.Invoke();
}
