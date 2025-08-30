using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyFootStep : MonoBehaviour
{
    [SerializeField] private float yAxisThreshold = .1f;
    [SerializeField] private PlayRandomSound randomSoundAudioSource;
    private bool didStep;
    private void Awake()
    {
        didStep = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y > yAxisThreshold)
        {
            didStep = false;
        }
        else if(!didStep)
        {
            didStep = true;
            randomSoundAudioSource.PlaySound();

        }
    }
}
