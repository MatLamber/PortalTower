using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Rigidbody rigidbody => GetComponent<Rigidbody>();
    private void OnCollisionEnter(Collision other)
    {
        rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }
}
