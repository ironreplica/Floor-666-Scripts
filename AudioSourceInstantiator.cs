using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSourceInstantiator : MonoBehaviour
{
    public GameObject audioSourcePrefab;
    public AudioClip[] clips;
    public bool threeDimensional;
    public float volume;
    /// <summary>
    /// Create a new audiosource. Auto decays for optimization.
    /// </summary>
    /// <param name="clip">Audioclip to play</param>
    /// <param name="pos">Position to create the source at</param>
    /// <param name="threeDimensional"></param>
    /// <param name="volume">Floating point between 0-1</param>
    /// <returns>a new audio source gamewAdwAobject.</returns>
    public GameObject InstantiateAndActivateAudioSource(AudioClip clip, Transform pos,  bool threeDimensional, float volume)
    {
        GameObject newAudioSource = Instantiate(audioSourcePrefab, null);
        newAudioSource.transform.position = pos.position;
        AudioSource audioSource = newAudioSource.GetComponent<AudioSource>();
        audioSource.GetComponent<PlayRandomSound>().AddClip(clip);
        if (threeDimensional)
        {
            audioSource.spatialBlend = 1;
        }
        else
        {
            audioSource.spatialBlend = 0;
        }
        audioSource.volume = volume;
        audioSource.GetComponent<PlayRandomSound>().PlaySound();
        return newAudioSource;
    }
    /// <summary>
    /// This will auto instantiate the source in the position of the parent object
    /// </summary>
    public void InstantiateSourceFromAnimation()
    {
        GameObject newAudioSource = Instantiate(audioSourcePrefab, null, true);
        AudioSource audioSource = newAudioSource.GetComponent<AudioSource>();
        audioSource.GetComponent<PlayRandomSound>().clips = clips;
        if (threeDimensional)
        {
            audioSource.spatialBlend = 1;
        }
        else
        {
            audioSource.spatialBlend = 0;
        }
        audioSource.volume = volume;
        audioSource.GetComponent<PlayRandomSound>().PlaySound();
       
    }
}
