using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabbableWithPhysics : MonoBehaviour
{
    private XRGrabInteractable thisItem;
    private Rigidbody rb;
    private Collider[] colliders;
    private int defaultLayer;

    void Start()
    {
        thisItem = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        defaultLayer = gameObject.layer;

        // Subscribe to grab events
        thisItem.selectEntered.AddListener(OnGrabbed);
        thisItem.selectExited.AddListener(OnReleased);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log(args.interactorObject);
        DisableCollider disableCollider;
        if(args.interactorObject.transform.gameObject.TryGetComponent(out disableCollider)){
            disableCollider.Disable();
        }
        SetLayerRecursively(gameObject, LayerMask.NameToLayer("GrabInteractable"));
        RefreshCollisionState();
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        DisableCollider disableCollider;
        if (args.interactorObject.transform.gameObject.TryGetComponent(out disableCollider))
        {
            disableCollider.Disable();
        }
        SetLayerRecursively(gameObject, defaultLayer);
        RefreshCollisionState();
    }

    private void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    private void RefreshCollisionState()
    {
        // Temporarily disable physics simulation to force collision refresh
        if (rb != null)
        {
            rb.detectCollisions = false;
            rb.detectCollisions = true;
        }

        // Alternative method for colliders
        foreach (Collider col in colliders)
        {
            col.enabled = false;
            col.enabled = true;
        }
    }

    private void OnDestroy()
    {
        // Clean up event listeners
        thisItem.selectEntered.RemoveListener(OnGrabbed);
        thisItem.selectExited.RemoveListener(OnReleased);
    }
}
