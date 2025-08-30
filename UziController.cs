using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(ShellEject))]
public class UziController : MonoBehaviour
{
    // TODO: Add a better cooldown, muzzleflash, bullet shell drop, reloading, damage, etc
    
    [Tooltip("Transform at which to instantiate the muzzleflash and audio source components.")]
    [SerializeField] private Transform muzzleEndpoint;

    [Tooltip("XRSocketInteractable object for attaching the magazine to.")]
    [SerializeField] private XRSocketInteractor magazineSocketInteractor;

    [Tooltip("Prefab for the audio source for the gun shot sound.")]
    [SerializeField] private GameObject audioSourcePrefab;

    [Tooltip("Prefab for concrete bullet impact.")]
    [SerializeField] private GameObject bulletImpactPrefab;

    [Tooltip("Prefab for muzzle flash. Make sure prefab plays on wake and destroys itself.")]
    [SerializeField] private GameObject muzzleFlashPrefab;

    [Tooltip("Audio clips for gunshots. At least one clip is required.")]
    [SerializeField] private AudioClip[] audioClips;
    
    [Tooltip("Right controllers ActionBasedController component.")]
    public ActionBasedController rightHand;

    [Tooltip("Shoot cooldown/firerate")]
    public float cooldownTime;

    [Tooltip("Added Force to objects bullets hit that contain RigidBodies")]
    public float forceModifier = 1;

    [Tooltip("Bullet range.")]
    public int bulletRange;

    [Tooltip("Layermask for bullets to hit.")]
    public LayerMask layerMask;

    [Tooltip("Flip raycast if the bullets are being shot backwards.")]
    public bool flipRay;

    [SerializeField] private XRGrabInteractable gun;
    private bool isShooting;

    private ShellEject shellEject;
    private void Start()
    {
        gun = GetComponent<XRGrabInteractable>();
        shellEject = GetComponent<ShellEject>();
        isShooting = false;
    }
    void Update()
    {
        if(rightHand.activateAction.action.IsPressed() && gun.isSelected)
        {
            if (!isShooting && magazineSocketInteractor.interactablesSelected.Count > 0)
            {
                StartCoroutine(Shoot());
            }
            
        }
    }
    private IEnumerator Shoot()
    {
        Debug.Log(magazineSocketInteractor.interactablesSelected.Count);
        if (magazineSocketInteractor.interactablesSelected.Count > 0 )
        {
            MagazineController magazineController = magazineSocketInteractor.interactablesSelected[0].transform.gameObject.GetComponent<MagazineController>();
            Debug.Log(magazineController.GetCurAmmo());
            if(magazineController.GetCurAmmo() - 1 >= 0)
            {
                magazineController.UseBullet(1);
                isShooting = true;
                GameObject gunShotSoundObj = Instantiate(audioSourcePrefab);
                GameObject muzzleFlashObj = Instantiate(muzzleFlashPrefab, muzzleEndpoint);
                shellEject.Eject();
                gunShotSoundObj.transform.parent = null;
                gunShotSoundObj.transform.position = muzzleEndpoint.position;

                gunShotSoundObj.GetComponent<AudioSource>().clip = audioClips[Random.Range(0, audioClips.Length)];

                gunShotSoundObj.GetComponent<CollisionSoundSource>().Activate();
                CastRay();
                yield return new WaitForSeconds(cooldownTime);
                isShooting = false;
                StopCoroutine(Shoot());
            }
            else
            {
                yield return null;
            }
        }
        else
        {
            yield return null;
            StopCoroutine(Shoot());

        }

    }
    private RaycastHit CastRay()
    {
        Ray ray;
        if (flipRay)
        {
            ray = new Ray(muzzleEndpoint.transform.position, -muzzleEndpoint.transform.forward);
        }
        else
        {
            ray = new Ray(muzzleEndpoint.transform.position, muzzleEndpoint.transform.forward);
        }
        
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, bulletRange, ~layerMask))
        {
            // Ray hit something
            /*GameObject hitObject = hit.collider.gameObject;*/
            GameObject hitEffect = Instantiate(bulletImpactPrefab);
            hitEffect.transform.parent = null;
            hitEffect.transform.position = hit.point;

            if (hit.transform.gameObject.GetComponent<Rigidbody>())
            {
                
                hit.transform.gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * forceModifier, ForceMode.Impulse);
            }
            // Do something with the hit object if needed
        }

        return hit;
    }

}
