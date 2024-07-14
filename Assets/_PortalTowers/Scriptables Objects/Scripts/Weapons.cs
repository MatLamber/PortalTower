using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Weapon", menuName = "WeaponData")]
public class Weapons : ScriptableObject
{
    public float fireRate = 1; // las balas por segundo que dispara el arma;
    public float lastShootTime;

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
