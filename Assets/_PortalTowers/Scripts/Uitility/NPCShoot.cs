using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCShoot : MonoBehaviour
{
    [SerializeField] private Weapons currentWeaponData;
    [SerializeField] private Transform gunPoint;
    [SerializeField] private ParticleSystem currentGunMuzzle;
    private bool canShoot = true;

    private void Start()
    {
        currentWeaponData.lastShootTime = 0;
        StartCoroutine(DisableShooting());
    }

    private void LateUpdate()
    {
        if(currentWeaponData is null || !canShoot) return;
        if (currentWeaponData.CanShoot())
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        for (int i = 0; i < 3; i++)
        {
              GameObject newMuzzleFlash = ObjectPool.Instance.GetObjet(currentWeaponData.muzzlePrefab);
             newMuzzleFlash.transform.position = gunPoint.transform.position;
             newMuzzleFlash.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
             ObjectPool.Instance.ReturnObject(newMuzzleFlash,0.8f);
            GameObject newBullet = ObjectPool.Instance.GetObjet(currentWeaponData.bulletPrefab);
            newBullet.transform.position = gunPoint.position;
            newBullet.transform.rotation = Quaternion.LookRotation(gunPoint.forward);
            BulletController newBulletController = newBullet.GetComponent<BulletController>();
            newBulletController.Power = currentWeaponData.power;
            Vector3 bulletDirection = currentWeaponData.ApplySpread(gunPoint.forward);
           // currentGunMuzzle.Play();
            newBullet.GetComponent<Rigidbody>().velocity = bulletDirection * (currentWeaponData.bulletSpeed);
            newBulletController.EnableTrail();

        }
        EventsManager.Instance.OnPlayerShoot();
    }

    IEnumerator DisableShooting()
    {
        yield return new WaitForSeconds(5);
        canShoot = false;
    }
}
