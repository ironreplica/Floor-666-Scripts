using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSocketInteractor))]
public class InventorySlot : MonoBehaviour
{
    private XRSocketInteractor socket;
    private GameObject currentItem;
    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }
    
    public void OnSocketHoverEnter()
    {
        Debug.Log(currentItem.name);
        currentItem.GetComponent<MeshCollider>().enabled = true;
    }
    public void OnSocketHoverExit()
    {
        currentItem.GetComponent<MeshCollider>().enabled = false;
    }
    public void OnSocketEnter()
    {
        currentItem = socket.interactablesSelected[0].transform.gameObject;
        currentItem.GetComponent<MeshCollider>().enabled = false;
    }
    public void OnSocketExit()
    {
        currentItem.GetComponent<MeshCollider>().enabled = true;
    }
}
