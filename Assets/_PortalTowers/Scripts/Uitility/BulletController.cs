using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] private GameObject bulletImpactFX;
    private Rigidbody rigidbody => GetComponent<Rigidbody>();
    private void OnCollisionEnter(Collision other)
    {
        CreateImpactFX(other);
        ObjectPool.Instance.ReturnBullet(gameObject);
    }

    private void CreateImpactFX(Collision other)
    {
        if (other.contacts.Length >0)
        {
            ContactPoint contactPoint = other.contacts[0];
            GameObject newImpactFx = Instantiate(bulletImpactFX, contactPoint.point,
                Quaternion.LookRotation(contactPoint.normal));
        }
    }
}
