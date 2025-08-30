using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class DynamicGrabPoint : XRGrabInteractable
{
    public Transform leftHandGrabPoint;
    public Transform rightHandGrabPoint;
    public Vector3 rotationOffset = new Vector3(-45f, 0f, 0f); // Customizable offset
    private string controllerLeftOrRight;

    // This might break every thing, the previous method was onselectentered
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (args.interactor.name.Contains("Left"))
        {
            controllerLeftOrRight = "Left";
            if(ValidateGrabPoints()) attachTransform = leftHandGrabPoint;

        }
        else if (args.interactor.name.Contains("Right"))
        {
            controllerLeftOrRight = "Right";
            if(ValidateGrabPoints()) attachTransform = rightHandGrabPoint;
        }

        // Apply rotation offset
        /*if (attachTransform != null)
        {
            Quaternion offsetRot = Quaternion.Euler(rotationOffset);
            attachTransform.rotation = args.interactorObject.GetAttachTransform(this).rotation * offsetRot;
        }*/

        base.OnSelectEntered(args);
    }
    private bool ValidateGrabPoints()
    {
        return leftHandGrabPoint && rightHandGrabPoint != null ? true : false;
    }
    public char LeftOrRight()
    {
        return controllerLeftOrRight == "Left" ? 'L' : 'R';
    }
    /*protected override void OnSelectExited(SelectExitEventArgs args)
    {
        // Reset rotation when released
        if (attachTransform != null)
        {
            attachTransform.localRotation = Quaternion.identity;
        }

        base.OnSelectExited(args);
    }*/
}
