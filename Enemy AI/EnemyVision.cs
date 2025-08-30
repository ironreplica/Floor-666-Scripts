using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    [SerializeField] private EnemyController enemyController;
    [SerializeField] private LayerMask layerMask;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ActivateTrigger();
    }
    public void ActivateTrigger()
    {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            Debug.LogError("MeshCollider not found on this GameObject");
            return;
        }

        meshCollider.convex = true;  // MeshCollider must be convex to be used as a trigger
        meshCollider.isTrigger = true;


        // Calculate the bounds of the scaled mesh collider
        Bounds bounds = meshCollider.bounds;
        Vector3 center = bounds.center;
        float radius = bounds.extents.magnitude;  // Use the magnitude of extents as an approximation of radius

        // Check for overlapping colliders
        Collider[] overlappingColliders = Physics.OverlapSphere(center, radius, layerMask);

        foreach (Collider col in overlappingColliders)
        {
            if (col.gameObject != gameObject)
            {
                // This collider is inside the trigger
                if (col.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Player detected");
                    enemyController.SetTarget(col.gameObject.transform);
                }
                
            }
        }
    }

}
