using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class ToggleRays : MonoBehaviour
{
    [Tooltip("Right controllers ActionBasedController component.")]
    public ActionBasedController rightHand;

    [Tooltip("Left controllers ActionBasedController component.")]
    public ActionBasedController leftHand;

    [Tooltip("Left controller desired button InputAction reference.")]
    public InputActionReference leftButton;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (leftButton.action.IsPressed())
        {
            Debug.Log("Nice");
        }
    }
}
