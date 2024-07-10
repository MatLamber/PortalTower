using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Joystick : MonoBehaviour
{
    [Header("Elements")] [SerializeField] private RectTransform joystickOutline;
    [Header("Elements")] [SerializeField] private RectTransform joystickNob;

    [Header("Settings")]
    [SerializeField] private float moveFactor;
    private Vector3 move;
    private bool canControl;
    private Vector3 clickedPostion;

    private void Start()
    {
        HideJoystick();
    }

    private void Update()
    {
        if(canControl)
            ControlJoystick();
    }

    public void ClickedOnJoystickZoneCallback()
    {

        clickedPostion = Input.mousePosition;
        joystickOutline.position = clickedPostion; 
        
        ShowJoystick();
    }

    private void ShowJoystick()
    {
        move = Vector3.zero;
        joystickOutline.gameObject.SetActive(true);
        canControl = true;
    }

    public void HideJoystick()
    {
        joystickOutline.gameObject.SetActive(false);
        canControl = false;

        move = Vector3.zero;
        EventsManager.Instance.OnJoystickMove(move);
    }

    private void ControlJoystick()
    {
        Vector3 currentPostion = Input.mousePosition;
        Vector3 direction = currentPostion - clickedPostion;

        float moveMagnitude = direction.magnitude * moveFactor / Screen.width;

        moveMagnitude = Mathf.Min(moveMagnitude, joystickOutline.rect.width/2);

        move = direction.normalized * moveMagnitude;

        Vector3 targetPosition = clickedPostion + move;
        
        joystickNob.position = targetPosition;
        
        EventsManager.Instance.OnJoystickMove(move);
        
    }
}
