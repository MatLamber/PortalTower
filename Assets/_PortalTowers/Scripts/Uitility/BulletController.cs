using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    [SerializeField] private GameObject bulletImpactFX;
    private Rigidbody rigidbody => GetComponent<Rigidbody>();
    private TrailRenderer trail;
    private string enemyTag = "Enemy";
    private float power;
    public float Power
    {
        get => power;
        set => power = value;
    }

    private void Awake()
    {
        if (GetComponent<TrailRenderer>() != null)
            trail = GetComponent<TrailRenderer>();
    }

    private void OnCollisionEnter(Collision other)
    {
        gameObject.SetActive(false);
        DisableTrail();
        CreateImpactFX(other);
        ObjectPool.Instance.ReturnObject(gameObject,0.3f);
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
            ObjectPool.Instance.ReturnObject(newImpactFx,0.8f);
            
        }
    }

    public void EnableTrail()
    {
        if(trail != null)
            trail.enabled = true;
    }

    public void DisableTrail()
    {
        if(trail != null)
            trail.enabled = false;
    }
    
}
