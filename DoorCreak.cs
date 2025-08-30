using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class DoorCreak : MonoBehaviour
{
    [Tooltip("Angular velocity threshold to trigger creak sound")]
    [SerializeField] private float graceValue = 0.1f;
    [SerializeField] private AudioClip creakSound;

    private AudioSource doorAudioSource;
    private Rigidbody doorRigidbody;
    private Vector3 angularVelocity;

    void Start()
    {
        doorAudioSource = GetComponent<AudioSource>();
        doorAudioSource.clip = creakSound;
        doorRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        angularVelocity = doorRigidbody.angularVelocity;

        // Parentheses fix for proper condition evaluation
        bool isMoving = (angularVelocity.x > graceValue ||
                        angularVelocity.y > graceValue ||
                        angularVelocity.z > graceValue);

        if (isMoving)
        {
            if (!doorAudioSource.isPlaying)
            {
                doorAudioSource.Play();
                Debug.Log(doorAudioSource.isPlaying);
            }
        }
        else
        {
            if (doorAudioSource.isPlaying)
            {
                doorAudioSource.Stop();
                Debug.Log(doorAudioSource.isPlaying);

            }
        }
    }
}
