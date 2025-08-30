using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    [Tooltip("Prefab for the footstep effects, play on awake and decay.")]
    [SerializeField] private GameObject stepEffect;

    [Tooltip("Left foot position")]
    [SerializeField] private Transform leftFoot;

    [Tooltip("Right foot position")]
    [SerializeField] private Transform rightFoot;
    public void LeftStep()
    {
        GameObject effect = Instantiate(stepEffect, null);
        effect.transform.position = leftFoot.position;
    }
    public void RightStep()
    {
        GameObject effect = Instantiate(stepEffect, null);
        effect.transform.position = rightFoot.position;
    }
}
