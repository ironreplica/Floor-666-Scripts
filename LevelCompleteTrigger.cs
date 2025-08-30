using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteTrigger : MonoBehaviour
{
    [SerializeField] private LevelController levelController;
    [SerializeField] private GameObject playerHitBox;
    [SerializeField] private PlayerScore playerScore;
    private void OnTriggerEnter(Collider other)
    {
        if (playerScore.CompletedLevel)
        {
            Debug.Log("via trigger");
            levelController.CompleteLevel();
        }
    }
    
}
