using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollisionSoundSource : MonoBehaviour
{
    [SerializeField] public GameObject soundTargetandTriggerPrefab;
    private NoiseManager nm;
    private AudioSource audioSource;
    private float[] samples = new float[1024];
    private float peakDBValue = -100f;
    private float dbValue = -100f;
    private string uniqueID;

    void GenerateUniqueID()
    {
        uniqueID = System.Guid.NewGuid().ToString();
    }
    private void Awake()
    {
        
        nm = GameObject.Find("NoiseManager").GetComponent<NoiseManager>();
        if (string.IsNullOrEmpty(uniqueID))
        {
            GenerateUniqueID();
        }
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        
    }
    public void Activate()
    {
        
        audioSource.Play();
        StartCoroutine(selfDestruct());
    }
    private void OnAudioFilterRead(float[] data, int channels)
    {
        samples = data;
    }
    // this needs to be in every single thing that emits sound
    private void FixedUpdate()
    {
        if (audioSource.isPlaying)
        {
            calculateDB();
            nm.UpdateNoiseLevel(uniqueID, dbValue);
            if (dbValue > peakDBValue)
            {
                peakDBValue = dbValue;
            }
           /* Debug.Log($"Source ID: {uniqueID}, DB Value: {dbValue}");*/
        }
        else
        {
            nm.UpdateNoiseLevel(uniqueID, -100f); // Set to minimum when not playing
        }
    }


    private void calculateDB()
    {
        float sum = 0f;
        for (int i = 0; i < samples.Length; i++)
        {
            sum += samples[i] * samples[i];
        }
        float rmsValue = Mathf.Sqrt(sum / samples.Length);
        dbValue = 20 * Mathf.Log10(rmsValue / 0.1f);
    }
    private IEnumerator selfDestruct()
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }
        nm.UpdateNoiseLevel(uniqueID, -100f);
        GameObject targetSRC = Instantiate(soundTargetandTriggerPrefab, null);


        targetSRC.transform.position = transform.position;
        targetSRC.GetComponent<AudioTargetAndTrigger>().ActivateTrigger((peakDBValue) * -1);
        Destroy(gameObject);
    }
}

