using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class WeaponVisualController : MonoBehaviour
{
    [SerializeField] private Transform starterPistol;
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


    private string starterPitolName = "StarterPistol";
    private string pistolName = "Pistol";
    private string rifleName = "Rifle";
    private string rocketLauncherlName = "RocketLauncher";
    private string shotgunName = "Shorty";

    private bool onTarget;

    private void Awake()
    {
        starterPistol.name = starterPitolName;
        pistol.name = pistolName;
        rifle.name = rifleName;
        bazooka.name = rocketLauncherlName;
        shotgun.name = shotgunName;
    }

    private void Start()
    {
        EventsManager.Instance.eventEnemyLockedIn += UpdateTargetLockedInState;
        EventsManager.Instance.eventSelectedOption += OnSelectedOption;
        StartCoroutine(FirstWeaponDelay());
    }

    private void OnDestroy()
    {
        EventsManager.Instance.eventSelectedOption -= OnSelectedOption;
        EventsManager.Instance.eventEnemyLockedIn -= UpdateTargetLockedInState;
    }

    IEnumerator FirstWeaponDelay()
    {
        yield return new WaitForSeconds(0f);
        SwitchOnGuns(starterPistol);
    }

    private void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SwitchOnGuns(starterPistol);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            SwitchOnGuns(pistol);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            SwitchOnGuns(rifle);
        if(Input.GetKeyDown(KeyCode.Alpha4))
            SwitchOnGuns(bazooka);
        if(Input.GetKeyDown(KeyCode.Alpha5))
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

    private void OnSelectedOption(string optionName)
    {
        if (optionName.Equals(pistolName))
        {
            SwitchOnGuns(pistol);
        }
        else if (optionName.Equals(rifleName))
        {
            SwitchOnGuns(rifle);
        }
        else if (optionName.Equals(rocketLauncherlName))
        {
            SwitchOnGuns(bazooka);
        }
        else if (optionName.Equals(starterPitolName))
        {
            SwitchOnGuns(starterPistol);
        }
        else if (optionName.Equals(shotgunName))
        {
            SwitchOnGuns(shotgun);
        }
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

            if (onTarget)
            {
                leftHandIK.weight = 1;
                headAim.weight = 1;
                gunAim.weight = 1;
            }

            if (gunTransform.name.Equals(pistolName))
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[0];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[0]);
                EventsManager.Instance.OnSwitchedWeapon(1,gunTransform);
            }
            else if (gunTransform.name.Equals(rifleName))
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[1];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[1]);
                EventsManager.Instance.OnSwitchedWeapon(3,gunTransform);
            }
            else if (gunTransform.name.Equals(rocketLauncherlName))
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[2];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[2]);
                EventsManager.Instance.OnSwitchedWeapon(5,gunTransform);
            }
            else if (gunTransform.name.Equals(starterPitolName))
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[0];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[0]);
                EventsManager.Instance.OnSwitchedWeapon(1,gunTransform);
            }
            else
            {
                leftHandTarget.transform.localPosition = leftHandIkPositions[3];
                leftHandTarget.transform.localRotation = Quaternion.Euler(leftHandIkRotaions[3]);
                EventsManager.Instance.OnSwitchedWeapon(1,gunTransform);
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


    private void UpdateTargetLockedInState(bool state, Transform enemylocked)
    {
        
        onTarget = state;
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
