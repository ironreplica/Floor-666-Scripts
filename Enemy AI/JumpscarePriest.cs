using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSourceInstantiator))]
public class JumpscarePriest : MonoBehaviour
{
    private Transform _playerNeck;
    private Player _player;
    private bool hasJumpscared;
    private float darknessIntensity = .25f;
    private float clipTrim = -2.5f;
    private AudioSourceInstantiator audioSourceInstantiator;
    public float bellVolume = 10;
    public float gripVolume = 30;
    private bool isAdjustingFog;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip bellSound;
    [SerializeField] private AudioClip gripSound;
    [SerializeField] private float xRotationOffset;
    [SerializeField] private Transform leftHandIK; // Assign the left hand IK target in the Inspector
    [SerializeField] private Transform rightHandIK; // Assign the right hand IK target in the Inspector
    [SerializeField] private float movementDuration = 3f; // Duration for movement (in seconds)

    private bool isJumpscareActive = false;

    private void Awake()
    {
        hasJumpscared = false;
        isAdjustingFog = false;
        audioSourceInstantiator = GetComponent<AudioSourceInstantiator>();
    }
    public void ActivateJumpscare(Transform PlayerNeck, Player player)
    {
        audioSourceInstantiator.InstantiateAndActivateAudioSource(bellSound, transform, false, bellVolume);
        // Second audio trigger - bell sound
        player.DisablePlayerMove();
        player.DisablePlayerSnapLook();
        _playerNeck = PlayerNeck;
        isJumpscareActive = true;
        _player = player;
        // Start the coroutine to move hands
        StartCoroutine(MoveHandsToTarget());
    }

    private IEnumerator MoveHandsToTarget()
    {
        // Store initial positions and rotations of hands
        Vector3 leftHandStartPosition = leftHandIK.position;
        Vector3 rightHandStartPosition = rightHandIK.position;

        Quaternion leftHandStartRotation = leftHandIK.rotation;
        Quaternion rightHandStartRotation = rightHandIK.rotation;

        // Calculate target rotations
        Quaternion leftHandTargetRotation = Quaternion.Euler(
            _playerNeck.eulerAngles.x + xRotationOffset,
            _playerNeck.eulerAngles.y,
            _playerNeck.eulerAngles.z
        );

        Quaternion rightHandTargetRotation = Quaternion.Euler(
            _playerNeck.eulerAngles.x + xRotationOffset,
            _playerNeck.eulerAngles.y,
            _playerNeck.eulerAngles.z
        );

        float elapsedTime = 0f;

        while (elapsedTime < movementDuration)
        {
            // Increment elapsed time
            elapsedTime += Time.deltaTime;

            // Calculate interpolation factor
            float t = elapsedTime / movementDuration;

            // Smoothly move hands to their target positions
            leftHandIK.position = Vector3.Lerp(leftHandStartPosition, _playerNeck.position, t);
            rightHandIK.position = Vector3.Lerp(rightHandStartPosition, _playerNeck.position, t);

            // Smoothly rotate hands to their target rotations
            leftHandIK.rotation = Quaternion.Slerp(leftHandStartRotation, leftHandTargetRotation, t);
            rightHandIK.rotation = Quaternion.Slerp(rightHandStartRotation, rightHandTargetRotation, t);

            yield return null; // Wait for the next frame
        }

        // Ensure hands reach their exact target positions and rotations at the end
        leftHandIK.position = _playerNeck.position;
        rightHandIK.position = _playerNeck.position;

        leftHandIK.rotation = leftHandTargetRotation;
        rightHandIK.rotation = rightHandTargetRotation;

        // Third sound - grip
        audioSource = audioSourceInstantiator.InstantiateAndActivateAudioSource(gripSound, transform, false, gripVolume).GetComponent<AudioSource>();
        StartCoroutine(WaitForAudioClip(gripSound));
        hasJumpscared = true;
        isJumpscareActive = false; // Jumpscare is complete
    }
    
    private void EndJumpscare()
    {
        _player.EnablePlayerMove();
        _player.EnablePlayerSnapLook();
    }
    private IEnumerator WaitForAudioClip(AudioClip clip)
    {
        yield return new WaitForSeconds(clip.length + clipTrim);
        StartCoroutine(AdjustFogEndDistance(.8f));
    }
    private IEnumerator AdjustFogEndDistance(float duration)
    {
        
        isAdjustingFog= true;

        float startFogEndDistance = RenderSettings.fogEndDistance;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            RenderSettings.fogEndDistance = Mathf.Lerp(startFogEndDistance, darknessIntensity, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        RenderSettings.fogEndDistance = darknessIntensity;
        isAdjustingFog = false;
        SceneManager.LoadScene(2);
    }
}
