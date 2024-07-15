using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "WeaponData")]
public class Weapons : ScriptableObject
{
    public float fireRate = 1; // las balas por segundo que dispara el arma;
    public float lastShootTime;
    public float bulletsPerShot = 1;
    public float spreadAmout = 1;
    public GameObject bulletPrefab;
    public float bulletSpeed = 20;


    public Vector3 ApplySpread(Vector3 orginalDirection)
    {
        float randomizedValue = Random.Range(-spreadAmout, spreadAmout);
        Quaternion spreadRotation = Quaternion.Euler(randomizedValue, randomizedValue, randomizedValue);
        return spreadRotation * orginalDirection;
    }

    public bool CanShoot()
    {
   
        if (Time.time > lastShootTime + 1 / fireRate)
        {
            lastShootTime = Time.time;
            return true;
        }

        return false;
    }
}
