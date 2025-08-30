using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using TMPro;

public class CameraGameplayMechanicController : MonoBehaviour
{
    [SerializeField] private Camera cameraComponent; // Renamed to avoid conflict with 'camera' keyword
    public ActionBasedController leftController;
    public ActionBasedController rightController;
    public float CameraRange = 10f;
    public float CameraIntersectAngle = 90f;
    private GameObject currentEvidence;
    private DynamicGrabPoint cameraObject;

    public GameObject audioPrefab;
    public AudioClip[] flashSoundClips;
    [SerializeField] private GameObject[] evidenceObjects;
    [SerializeField] private AudioSource musicAudioSource;
    private Dictionary<GameObject, bool> evidenceDict = new Dictionary<GameObject, bool>();
    public Animator flashAnimator;
    private bool isPointedAtEvidence;
    public TextMeshProUGUI cameraText;
    [SerializeField] private Color noEvidenceColor;
    [SerializeField] private Color evidenceColor;
    [SerializeField] private string noEvidenceText;
    [SerializeField] private string evidenceText;
    [SerializeField] PlayerScore playerScore;
    private bool isLeftTriggerPressed = false;
    private bool isRightTriggerPressed = false;
    private bool canTakePicture = true;
    public GameObject audioSourcePrefab;
    public AudioClip[] clips;
    public AudioClip chaseSong;
    private void Awake()
    {
        
        isPointedAtEvidence = false;
        cameraObject = GetComponent<DynamicGrabPoint>();
        foreach (GameObject obj in evidenceObjects)
        {
            evidenceDict.Add(obj, false);
        }
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = flashAnimator.GetCurrentAnimatorStateInfo(0);

        if (cameraObject.isSelected && stateInfo.IsName("flashOff") && canTakePicture)
        {
            if (cameraObject.LeftOrRight() == 'L')
            {
                HandleTriggerInput(leftController.activateAction.action, ref isLeftTriggerPressed);
            }
            else if (cameraObject.LeftOrRight() == 'R')
            {
                HandleTriggerInput(rightController.activateAction.action, ref isRightTriggerPressed);
            }
        }

        if (IsAnyEvidenceWithinCamera(cameraComponent, evidenceDict))
        {
            Debug.Log("Evidence within camera");
            cameraText.text = evidenceText;
            cameraText.color = evidenceColor;
        }
        else
        {
            cameraText.text = noEvidenceText;
            cameraText.color = noEvidenceColor;
        }
    }

    private void HandleTriggerInput(InputAction action, ref bool triggerState)
    {
        if (action.WasPressedThisFrame() && !triggerState)
        {
            triggerState = true;
            StartCoroutine(TakePictureSequence());
        }
        else if (action.WasReleasedThisFrame())
        {
            triggerState = false;
        }
    }

    private IEnumerator TakePictureSequence()
    {
        canTakePicture = false;
        TakePicture();

        yield return new WaitForSeconds(flashAnimator.GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitUntil(() => flashAnimator.GetCurrentAnimatorStateInfo(0).IsName("flashOff"));

        canTakePicture = true;
    }

    private void TakePicture()
    {
        flashAnimator.SetTrigger("flash");
        if (isPointedAtEvidence)
        {
            // count this as a photo
            // You might want to add logic here to mark the evidence as photographed
            if (currentEvidence != null && evidenceDict.ContainsKey(currentEvidence) && evidenceDict[currentEvidence] == false)
            {
                evidenceDict[currentEvidence] = true;
                EvidencePhotoEnableObject evidencePhotoEnableObject;
                if(currentEvidence.TryGetComponent(out evidencePhotoEnableObject))
                {
                    evidencePhotoEnableObject.PhotoTaken();
                }
                GameObject source = Instantiate(audioSourcePrefab, null, true);
                PlayRandomSound randomSound = source.GetComponent<PlayRandomSound>();
                source.GetComponent<AudioSource>().volume = 1;
                source.GetComponent<AudioSource>().priority = 30;
                source.GetComponent<AudioSource>().spatialBlend = 0;
                randomSound.clips = clips;
                randomSound.PlaySound();
                // now loop through entire dict to see if its the last piece of evidence
                bool allFound = true;
                foreach (bool isFound in evidenceDict.Values)
                {
                    if (!isFound)
                    {
                        allFound = false;
                        break;
                    }
                }
                if (allFound)
                {
                    Debug.Log("All evidence found");
                    playerScore.CompletedLevel = true;
                    musicAudioSource.clip = chaseSong;
                    musicAudioSource.Play();
                   
                }
            }
        }

        // Uncomment and fix the audio logic if needed
        /*
        GameObject newAudioSource = Instantiate(audioPrefab, transform.position, Quaternion.identity);
        PlayRandomSound playRandSound = newAudioSource.GetComponent<PlayRandomSound>();
        if (playRandSound != null)
        {
            playRandSound.clips = flashSoundClips;
            playRandSound.PlaySound();
        }
        */
    }

    private bool IsAnyEvidenceWithinCamera(Camera cam, Dictionary<GameObject, bool> evidenceDict)
    {
        Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

        foreach (GameObject evidence in evidenceDict.Keys)
        {
            if (evidence != null && evidence.GetComponent<Renderer>() != null &&
                GeometryUtility.TestPlanesAABB(frustumPlanes, evidence.GetComponent<Renderer>().bounds) && evidenceDict[evidence] == false)
            {
                if (Vector3.Distance(transform.position, evidence.transform.position) <= CameraRange)
                {
                    currentEvidence = evidence;
                    isPointedAtEvidence = true;
                    return true;
                }
            }
        }
        isPointedAtEvidence = false;
        currentEvidence = null;
        return false;
    }

    private bool IntersectingObjects()
    {
        if (currentEvidence == null) return true;

        Vector3 direction = currentEvidence.transform.position - transform.position;
        float distance = Vector3.Distance(transform.position, currentEvidence.transform.position);

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > CameraIntersectAngle)
        {
            return true;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction.normalized, out hit, distance))
        {
            if (hit.transform.gameObject != currentEvidence)
            {
                DebugIntersectingObjects(hit);
                return true;
            }
        }

        DebugIntersectingObjects(hit);
        return false;
    }

    private void DebugIntersectingObjects(RaycastHit hit)
    {
        Vector3 start = transform.position;
        Vector3 end = currentEvidence.transform.position;
        Color lineColor = hit.transform.gameObject == currentEvidence ? Color.green : Color.red;

        Debug.DrawLine(start, end, lineColor, 0.1f);
        Debug.DrawLine(end, end + Vector3.up * 0.1f, lineColor, 0.1f);
        Debug.DrawLine(end, end + Vector3.right * 0.1f, lineColor, 0.1f);
        Debug.DrawLine(end, end + Vector3.forward * 0.1f, lineColor, 0.1f);
    }
}
