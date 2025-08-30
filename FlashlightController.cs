using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(DynamicGrabPoint))]
[RequireComponent(typeof(AudioSourceInstantiator))]
public class FlashlightController : MonoBehaviour
{
    public ActionBasedController leftController;
    public ActionBasedController rightController;
    private float flashlightAngle;
    private float flashlightRange;
    private DynamicGrabPoint flashlightObj;
    private bool isLeftTriggerPressed = false;
    private AudioSourceInstantiator audioSourceInstantiator;
    [SerializeField] private AudioClip toggleSound;
    private bool isRightTriggerPressed = false;
    private bool flashlightIsOn;
    [SerializeField] private GameObject enemy;
    [SerializeField] private Light spotLight;
    private void Awake()
    {
        flashlightObj = GetComponent<DynamicGrabPoint>();
        audioSourceInstantiator = GetComponent<AudioSourceInstantiator>();
        flashlightAngle = spotLight.spotAngle;
        flashlightRange = spotLight.range;
        flashlightIsOn = false;
    }
    private void HandleTriggerInput(InputAction action, ref bool triggerState)
    {
        // rewrite this to be in its own class, and subscribe to this with a method
        if (action.WasPressedThisFrame() && !triggerState)
        {
            triggerState = true;
            ToggleFlashlight();
        }
        else if (action.WasReleasedThisFrame())
        {
            triggerState = false;
        }
    }
    private void ToggleFlashlight()
    {
        flashlightIsOn = !flashlightIsOn;
        spotLight.enabled = flashlightIsOn;

        audioSourceInstantiator.InstantiateAndActivateAudioSource(toggleSound, transform, true, .4f);
    }
    private void Update()
    {
        if (flashlightObj.LeftOrRight() == 'L')
        {
            HandleTriggerInput(leftController.activateAction.action, ref isLeftTriggerPressed);
        }
        else if(flashlightObj.LeftOrRight() == 'R')
        {
            HandleTriggerInput(rightController.activateAction.action, ref isRightTriggerPressed);
        }
        RaycastHit hit;
        Vector3 direction = enemy.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);
        if(angle <= flashlightAngle)
        {
            /*Debug.Log("Pointing at enemy: " + angle + "/" + flashlightAngle);*/
            if (Physics.Raycast(transform.position, direction.normalized, out hit, flashlightRange))
            {
                // check for intersects
                DebugIntersects(hit);
                if (hit.transform.gameObject == enemy)
                {
                    /*Debug.Log("Hit enemy: " + angle + "/" + flashlightAngle);*/

                    enemy.GetComponent<EnemyController>().PlayerSpotted();
                }
            }
        }

        
    }
    private void DebugIntersects(RaycastHit hit)
    {
        Debug.DrawLine(transform.position, hit.point);
        
    }
}
