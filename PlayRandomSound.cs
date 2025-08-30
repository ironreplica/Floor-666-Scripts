using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSound : MonoBehaviour
{
    [SerializeField] public bool decays = true;
    [SerializeField] public AudioClip[] clips;
    [SerializeField] public float decayTime = 1f; // Time to wait after sound finishes before destroying

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySound()
    {
        if (clips.Length == 0) return;

        audioSource.clip = clips[Random.Range(0, clips.Length)];
        audioSource.Play();

        if (decays)
        {
            StartCoroutine(DestroyAfterPlay());
        }
    }
    public void AddClip(AudioClip newClip)
    {
        AudioClip[] newArray = new AudioClip[clips.Length + 1];
        clips.CopyTo(newArray, 0);
        newArray[newArray.Length - 1] = newClip;
        clips = newArray;
    }
    private IEnumerator DestroyAfterPlay()
    {
        // Wait for the audio to finish playing
        yield return new WaitForSeconds(audioSource.clip.length);

        // Wait for the decay time
        yield return new WaitForSeconds(decayTime);

        // Destroy the GameObject
        Destroy(gameObject);
    }
}
