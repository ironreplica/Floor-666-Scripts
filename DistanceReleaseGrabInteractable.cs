using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class DistanceReleaseGrabInteractable : XRGrabInteractable
{
    public float maxGrabDistance = 0.25f;
    private IXRSelectInteractor cachedInteractor; // keeping track of the interactor
    [SerializeField] private Material grabMaterial;
    private Material defaultMaterial;
    private void Start()
    {
        defaultMaterial = GetComponent<MeshRenderer>().material;
    }
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        cachedInteractor = args.interactorObject;
        GetComponent<MeshRenderer>().material = grabMaterial;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        
        GetComponent<MeshRenderer>().material = defaultMaterial;
        cachedInteractor = null;
    }
    private void Update()
    {
        // This might not work with multiple colliders

        // Check if its being grabbed, if it is measure the distance and see if its within the max grab distance
        if (cachedInteractor != null && Vector3.Distance(cachedInteractor.transform.position, colliders[0].transform.position) > maxGrabDistance)
        {
            /*cachedInteractor.interactablesSelected.Count >*/
            // check if the interactor and the interactable its hitting are within the grab distance
            interactionManager.SelectExit(cachedInteractor, this);
        }
    }
    public void DetachInteractor()
    {
        if (cachedInteractor != null) interactionManager.SelectExit(cachedInteractor, this);
    }
}
