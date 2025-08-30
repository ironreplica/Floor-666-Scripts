using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSourceInstantiator))]
public class JumpscareTrigger : MonoBehaviour
{
    [Tooltip("Enables the object you want to become enabled after entering the trigger")]
    [SerializeField] private GameObject jumpscareToEnable;
    [SerializeField] private AudioSource[] audioSourcesToDisable;
    [SerializeField] private Light[] lightsToDisable;
    [Tooltip("Create a delay before the jumpscare object is enabled, for enabling x seconds after lights shut off")]
    [SerializeField] private float delayAfterDisable = 0;
    [Tooltip("If there is an enemy assigned to this, it will respawn him when the jumpscare is activated")]
    [SerializeField] private EnemyController enemyController;
    private AudioSourceInstantiator audioSourceInstantiator;
    [Tooltip("Audio clip will play when the jumpscare begins, then after the delay the chosen object will be enabled")]
    [SerializeField] private AudioClip disableSound;
    [Tooltip("Optional object to create the disableSound at")]
    [SerializeField] private Transform disableSoundLocation;
    [Tooltip("True will disable objects when the jumpscare trigger is active, false will disable objects on trigger enter.")]
    [SerializeField] private bool disableObjectsOnEnable;
    private void OnEnable()
    {
        audioSourceInstantiator = GetComponent<AudioSourceInstantiator>();
        if (disableObjectsOnEnable)
        {
            DisableObjects();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemyController.Respawn();
            Jumpscare();
        }
    }
    [ContextMenu("Test jumpscare")]
    public void Jumpscare()
    {
        StartCoroutine(delay());
        if (!disableObjectsOnEnable)
        {
            DisableObjects();
        }
    }
    private void DisableObjects()
    {
        audioSourceInstantiator.InstantiateAndActivateAudioSource(disableSound, disableSoundLocation == null ? transform : disableSoundLocation, false, 1);
        if (audioSourcesToDisable.Length > 0)
        {
            foreach (AudioSource source in audioSourcesToDisable)
            {
                source.enabled = false;
            }
            EnableObjects enableObjects;
            if (jumpscareToEnable.TryGetComponent(out enableObjects))
            {
                enableObjects.audioSourcesToEnable = audioSourcesToDisable;

            }
        }
        if (lightsToDisable.Length > 0)
        {
            foreach (Light light in lightsToDisable)
            {
                light.enabled = false;
            }
            EnableObjects enableObjects;

            if (jumpscareToEnable.TryGetComponent(out enableObjects))
            {
                enableObjects.lightsToEnable = lightsToDisable;

            }

        }
    }
    private IEnumerator delay()
    {
        yield return new WaitForSeconds(delayAfterDisable);
        jumpscareToEnable.SetActive(true);
        Destroy(gameObject);
    }
}
