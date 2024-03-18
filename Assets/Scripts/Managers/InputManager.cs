using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] DynamicJoystick joystick;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnDown();
        }
        if (Input.GetMouseButton(0))
        {
            OnDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnUp();
        }
    }

    private void OnDown()
    {

    }
    private void OnDrag()
    {
        Vector3 movement = new Vector3(joystick.Horizontal, joystick.Vertical) * Time.deltaTime;
        EventManager.onJoystick?.Invoke(movement);
    }
    private void OnUp()
    {
        Vector3 movement = new Vector3(joystick.Horizontal, joystick.Vertical) * Time.deltaTime;
        EventManager.onJoystick?.Invoke(movement);
    }
}
