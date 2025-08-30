using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class StepEffect : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;
    private AudioSource _audioSource;
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = clips[Random.Range(0, clips.Length)];
        _audioSource.Play();
    }
}
