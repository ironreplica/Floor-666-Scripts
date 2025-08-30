using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [SerializeField] private GameObject DoorBarrier;
    private Animator doorAnimator;
    private void Awake()
    {
        doorAnimator = GetComponent<Animator>();
        DoorBarrier.SetActive(false);
    }
    public void CompleteLevel()
    {
        DoorBarrier.SetActive(true);
        doorAnimator.SetTrigger("CloseDoor");
    }
}
