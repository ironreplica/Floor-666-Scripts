using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableObjects : MonoBehaviour
{
    public AudioSource[] audioSourcesToEnable;
    public Light[] lightsToEnable;
    public void EnableAllObjects()
    {
        // oppurtunity to play sounds or light flicker animations here
        if(audioSourcesToEnable.Length > 0)
        {
            foreach(AudioSource audioSource in audioSourcesToEnable)
            {
                audioSource.enabled = true;
            }
        }
        if(lightsToEnable.Length > 0)
        {
            foreach(Light light in lightsToEnable)
            {
                light.enabled = true;
            }
        }
    }
}
    
