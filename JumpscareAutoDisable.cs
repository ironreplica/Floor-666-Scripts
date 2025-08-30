using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSourceInstantiator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(EnableObjects))]
public class JumpscareAutoDisable : MonoBehaviour
{
    private AudioSource jumpscareBuildupSound;
    private AudioSourceInstantiator instantiator;
    private EnableObjects enableObjects;
    [SerializeField] private AudioClip dissapearSound;
    [SerializeField] private Transform goal;
    
    private void OnEnable()
    {
        enableObjects = GetComponent<EnableObjects>();
        jumpscareBuildupSound = GetComponent<AudioSource>();
        jumpscareBuildupSound.Play();
        instantiator = GetComponent<AudioSourceInstantiator>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("CutJumpscare"))
        {
            instantiator.InstantiateAndActivateAudioSource(dissapearSound, transform, false, 1);
            enableObjects.EnableAllObjects();
            Destroy(gameObject);
        }
    }
    private void Update()
    {
       /* if (goal != null) gameObject.transform.LookAt(goal);*/

    }
}
