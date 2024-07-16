using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] private GameObject bulletImpactFX;
    private Rigidbody rigidbody => GetComponent<Rigidbody>();
    private string enemyTag = "Enemy";
    private float power;
    public float Power
    {
        get => power;
        set => power = value;
    }
    private void OnCollisionEnter(Collision other)
    {
        CreateImpactFX(other);
        ObjectPool.Instance.ReturnObject(gameObject,0);
        if (other.collider.tag.Equals(enemyTag))
            EventsManager.Instance.OnEnemyHit(other.transform, power);
        
    }

    private void CreateImpactFX(Collision other)
    {
        if (other.contacts.Length > 0)
        {
            ContactPoint contactPoint = other.contacts[0];
            GameObject newImpactFx = ObjectPool.Instance.GetObjet(bulletImpactFX);
            newImpactFx.transform.position = contactPoint.point;
            newImpactFx.transform.rotation = Quaternion.LookRotation(contactPoint.normal);
            ObjectPool.Instance.ReturnObject(newImpactFx,0.3f);
            
        }
    }
}
