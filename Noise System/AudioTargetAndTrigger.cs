using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTargetAndTrigger : MonoBehaviour
{
    [SerializeField] private float scaleMultipler = 2f;
    [SerializeField] private float decayTime = 3f;
    private bool hasHitEnemy;
    public void ActivateTrigger(float scale)
    {
        // if the enemy has a different target, destroy this for performance enhancement
        SphereCollider triggerCollider = GetComponent<SphereCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.radius = scale * scaleMultipler;
        
        if (triggerCollider != null)
        {
            Collider[] overlappingColliders = Physics.OverlapSphere(triggerCollider.bounds.center, triggerCollider.radius);
            hasHitEnemy = false;
            foreach (Collider col in overlappingColliders)
            {
                if (col.gameObject != gameObject)
                {
                    
                    
                    if (col.gameObject.GetComponent<EnemyController>())
                    {
                       
                        col.gameObject.GetComponent<EnemyController>().SetTarget(gameObject.transform);
                        hasHitEnemy = true;
                    }
                    
                }
            }
            if (!hasHitEnemy)
            {
                StartCoroutine(DecayWithNoEnemy());
            }
        }
    }
    private IEnumerator DecayWithNoEnemy()
    {
        // Wait for the audio to finish playing
        yield return new WaitForSeconds(decayTime);

        // Wait for the decay time
        

        // Destroy the GameObject
        Destroy(gameObject);
    }

}
