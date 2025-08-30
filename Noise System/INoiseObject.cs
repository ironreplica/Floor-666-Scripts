using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoiseObject
{
    int noiseLevel { get; set; }
    /*CollisionSound collisionSound;*/
    void EmitNoise();
}
public abstract class NoiseObjectBase : MonoBehaviour, INoiseObject
{
    public abstract void EmitNoise();
    public int noiseLevel { get; set; }
    private void OnCollisionEnter(Collision collision)
    {
        EmitNoise();
        IncreaseNoiseLevel(noiseLevel);
    }
    protected virtual void IncreaseNoiseLevel(float value)
    {

    }
    
}