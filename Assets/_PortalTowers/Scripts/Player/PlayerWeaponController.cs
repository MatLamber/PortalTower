using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private List<Weapons> weaponsData;
    [SerializeField] private List<Transform> gunPoints;
    [SerializeField] private List<ParticleSystem> muzzleFlashes;
    private GameObject muzzleFlash;
    private Weapons currentWeaponData;
    private PlayerAnimator animator;
    private Transform gunPoint;
    private float bulletSpeed;
    private string starterPitolName = "StarterPistol";
    private string pistolName = "Pistol";
    private string rifleName = "Rifle";
    private string rocketLauncherlName = "RocketLauncher";
    private string shotgunName = "Shorty";
    private ParticleSystem currentGunMuzzle;


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

    private void LateUpdate()
    {
        if(currentWeaponData is null) return;
        if (currentWeaponData.CanShoot() && animator.isOnTarget)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        for (int i = 0; i < currentWeaponData.bulletsPerShot; i++)
        {
          //  GameObject newMuzzleFlash = ObjectPool.Instance.GetObjet(currentWeaponData.muzzlePrefab);
           // newMuzzleFlash.transform.position = gunPoint.transform.position;
           // newMuzzleFlash.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
           // ObjectPool.Instance.ReturnObject(newMuzzleFlash,0.8f);
            GameObject newBullet = ObjectPool.Instance.GetObjet(currentWeaponData.bulletPrefab);
            newBullet.transform.position = gunPoint.position;
            newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
            BulletController newBulletController = newBullet.GetComponent<BulletController>();
            newBulletController.Power = currentWeaponData.power;
            Vector3 bulletDirection = currentWeaponData.ApplySpread(gunPoint.forward);
            currentGunMuzzle.Play();
            newBullet.GetComponent<Rigidbody>().velocity = bulletDirection * (currentWeaponData.bulletSpeed);
            newBulletController.EnableTrail();

        }
        EventsManager.Instance.OnPlayerShoot();
    }

    private void SetCurrentWeaponData(int id,Transform gunTransform)
    {

        if (gunTransform.name.Equals(starterPitolName))
        {
            currentWeaponData = weaponsData[0];
            gunPoint = gunPoints[0];
            currentGunMuzzle = muzzleFlashes[0];
        }
        else if (gunTransform.name.Equals(pistolName))
        {
            currentWeaponData = weaponsData[1];
            gunPoint = gunPoints[1];
            currentGunMuzzle = muzzleFlashes[1];
        }
        else if (gunTransform.name.Equals(rifleName))
        {
            currentWeaponData = weaponsData[2];
            gunPoint = gunPoints[2];
            currentGunMuzzle = muzzleFlashes[2];
        }
        else if (gunTransform.name.Equals(rocketLauncherlName))
        {
            currentWeaponData = weaponsData[3];
            gunPoint = gunPoints[3];
            currentGunMuzzle = muzzleFlashes[3];
        }
        else
        {
            currentWeaponData = weaponsData[4];
            gunPoint = gunPoints[4];
            currentGunMuzzle = muzzleFlashes[4];
        }

        currentWeaponData.lastShootTime = 0;
    }
    
}
