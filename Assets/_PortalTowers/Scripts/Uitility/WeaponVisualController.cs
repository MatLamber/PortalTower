using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform pistol;
    [SerializeField] private Transform rifle;
    [SerializeField] private Transform bazooka;
    [SerializeField] private Transform shotgun;

    [SerializeField] private List<Transform> weaponsTransfrom = new List<Transform>();
    
    [SerializeField] private MultiAimConstraint headAim;
    [SerializeField] private MultiAimConstraint gunAim;
    
    [SerializeField] private TwoBoneIKConstraint leftHandIK;
    [SerializeField] private Transform leftHandTarget;
    [SerializeField] private List<Vector3> leftHandIkPositions;
    [SerializeField] private List<Vector3> leftHandIkRotaions;


    private string pistolName = "Pistol";
    private string rifleName = "Rifle";
    private string rocketLauncherlName = "RocketLauncher";
    private string shotgunName = "Shorty";


    private void Awake()
    {
        pistol.name = pistolName;
        rifle.name = rifleName;
        bazooka.name = rocketLauncherlName;
        shotgun.name = shotgunName;
    }

    private void Start()
    {
        SwitchOnGuns();
        EventsManager.Instance.eventEnemyLockedIn += UpdateTargetLockedInState;
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventEnemyLockedIn -= UpdateTargetLockedInState;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SwitchOnGuns(pistol);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            SwitchOnGuns(rifle);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            SwitchOnGuns(bazooka);
        if(Input.GetKeyDown(KeyCode.Alpha4))
            SwitchOnGuns(shotgun);
        if(Input.GetKeyDown(KeyCode.Alpha0))
            SwitchOnGuns();
        
    }

    private void SwitchOnGuns(Transform gunTransform = null)
    {
        SwitchOffGuns();
        if (gunTransform != null)
            gunTransform.gameObject.SetActive(true);
        OnSwitchedWeapon(gunTransform);
        
    }

    private void OnSwitchedWeapon(Transform gunTransform = null)
    {
        if (gunTransform == null)
        {
            leftHandIK.weight = 0;
            headAim.weight = 0;
            gunAim.weight = 0;
            EventsManager.Instance.OnSwitchedWeapon(0);
        }
        else
        {
            leftHandIK.weight = 1;
            headAim.weight = 1;
            gunAim.weight = 1;
            if (gunTransform.name.Equals(pistolName))
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[0];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[0]);
                EventsManager.Instance.OnSwitchedWeapon(1);
            }
            else if (gunTransform.name.Equals(rifleName))
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[1];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[1]);
                EventsManager.Instance.OnSwitchedWeapon(3);
            }
            else if (gunTransform.name.Equals(rocketLauncherlName))
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[2];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[2]);
                EventsManager.Instance.OnSwitchedWeapon(5);
            }
            else
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[3];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[3]);
                EventsManager.Instance.OnSwitchedWeapon(1);
            }
        }
        

    }

    private void SwitchOffGuns()
    {
        foreach (Transform weaponTransform in weaponsTransfrom)
        {
            weaponTransform.gameObject.SetActive(false);
        }
    }


    private void UpdateTargetLockedInState(bool state)
    {

        if (state)
        {
            headAim.weight = 1;
            gunAim.weight = 1;
            
        }
        else
        {
            headAim.weight = 0;
            gunAim.weight = 0;
            
        }
    }

}
