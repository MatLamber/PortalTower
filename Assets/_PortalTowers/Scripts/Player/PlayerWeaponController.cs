using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private List<Weapons> weaponsData;
    [SerializeField] private List<Transform> gunPoints;
    private Weapons currentWeaponData;
    private PlayerAnimator animator;
    private Transform gunPoint;
    
    private string pistolName = "Pistol";
    private string rifleName = "Rifle";
    private string rocketLauncherlName = "RocketLauncher";
    private string shotgunName = "Shorty";


    private void Awake()
    {
        animator = GetComponent<PlayerAnimator>();
    }

    private void Start()
    {
        EventsManager.Instance.eventSwitchedWepon += SetCurrentWeaponData;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventSwitchedWepon -= SetCurrentWeaponData;
    }

    private void Update()
    {
        if(currentWeaponData is null) return;

        if (currentWeaponData.CanShoot() && animator.isOnTarget)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject newBullet = ObjectPool.Instance.GetBullet();
        newBullet.transform.position = gunPoint.position;
        newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
        newBullet.GetComponent<Rigidbody>().velocity = gunPoint.forward * bulletSpeed;
        EventsManager.Instance.OnPlayerShoot();
    }

    private void SetCurrentWeaponData(int id,Transform gunTransform)
    {

        if (gunTransform.name.Equals(pistolName))
        {
            currentWeaponData = weaponsData[0];
            gunPoint = gunPoints[0];
        }
        else if (gunTransform.name.Equals(rifleName))
        {
            currentWeaponData = weaponsData[1];
            gunPoint = gunPoints[1];
        }
        else if (gunTransform.name.Equals(rocketLauncherlName))
        {
            currentWeaponData = weaponsData[2];
            gunPoint = gunPoints[2];
        }
        else
        {
            currentWeaponData = weaponsData[3];
            gunPoint = gunPoints[3];
        }

        currentWeaponData.lastShootTime = 0;
    }
}
