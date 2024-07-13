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
    public Action <int> eventSwitchedWepon;
    public void OnJoystickMove(Vector3 moveVector) => eventJoyStrickMove?.Invoke(moveVector);
    public void OnPlayerShoot() => eventPlayerShoot?.Invoke();
    public void OnSwitchedWeapon(int weaponIDonAnimationLayer) => eventSwitchedWepon?.Invoke(weaponIDonAnimationLayer);

}
