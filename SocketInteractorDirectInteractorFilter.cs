using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Filtering;


[RequireComponent(typeof(XRSocketInteractor))]
public class SocketInteractorDirectInteractorFilter : MonoBehaviour, IXRSelectFilter
{
    public bool canProcess => true;

    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        Debug.Log(interactable + ": " + interactable.transform.gameObject);
        return interactor is XRDirectInteractor;
    }
}
