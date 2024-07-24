using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsManager : MonoBehaviour
{
    public static EventsManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }


    public Action<Vector3> eventJoyStrickMove;
    public Action eventPlayerShoot;
    public Action <int,Transform> eventSwitchedWepon;
    public Action<bool, Transform> eventEnemyLockedIn;
    public Action<Transform, float> eventEnemyHit;
    public Action <Transform> eventEnemyDeath;
    public Action eventLevelFinish;
    public Action <int> eventTeleportPlayer;
    public Action <bool> eventPlayerHit;
    public Action<string> eventSelectedOption;
    public void OnJoystickMove(Vector3 moveVector) => eventJoyStrickMove?.Invoke(moveVector);
    public void OnPlayerShoot() => eventPlayerShoot?.Invoke();
    public void OnSwitchedWeapon(int weaponIDonAnimationLayer, Transform weaponTransform = null) => eventSwitchedWepon?.Invoke(weaponIDonAnimationLayer, weaponTransform);
    public void OnEnemyLockedIn(bool state, Transform enemyLocked) => eventEnemyLockedIn?.Invoke(state,enemyLocked);
    public void OnEnemyHit(Transform enemyTransfrom, float power) => eventEnemyHit?.Invoke(enemyTransfrom, power);
    public void OnEnemyDeath(Transform enemyTransfrom) => eventEnemyDeath?.Invoke(enemyTransfrom);
    public void OnLevelFinish() => eventLevelFinish?.Invoke();
    public void OnTeleportPlayer(int id) => eventTeleportPlayer?.Invoke(id);
    public void OnPlayerHit(bool wasHit) => eventPlayerHit?.Invoke(wasHit);
    public void OnSelectedOption(string optionName) => eventSelectedOption?.Invoke(optionName);

}
