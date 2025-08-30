using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decay : MonoBehaviour
{
    [Tooltip("Begin decay timer on collision or on start.")]
    [SerializeField] private bool OnCollision = false;

    [Tooltip("Time until decay after the timer begins.")]
    [SerializeField] private float time = 10;
    void Start()
    {
        if (OnCollision) return;
        StartCoroutine(decay());
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!OnCollision) return;
        StartCoroutine(decay());
    }
    private IEnumerator decay()
    {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
