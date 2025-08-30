using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreChildColliders : MonoBehaviour
{
    private Collider[] _colliders;
    private void Awake()
    {
        _colliders = GetComponentsInChildren<Collider>();
        ignoreColliders();
    }

    private void ignoreColliders()
    {
        foreach(Collider collider in _colliders)
        {
            foreach(Collider collider2 in _colliders)
            {
                Physics.IgnoreCollision(collider, collider2);
            }
        }
        Debug.Log("Ignored " + _colliders.Length + " colliders.");
    }
}
