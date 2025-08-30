using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCollider : MonoBehaviour
{
    // Might need to add support for collider arrays 
    [SerializeField] private Collider[] colliders;
    public void Disable()
    {
        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }
    public void Enable()
    {
        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
    }
}
