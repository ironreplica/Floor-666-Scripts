using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public class CollisionSound : MonoBehaviour
{
    public GameObject audioSourcePrefab;
    public AudioClip[] audioClips;
    public int priority = 128;
    public float volume = .8f;

    private void OnCollisionEnter(Collision collision)
    {
        // Get the first contact point of the collision
        ContactPoint contact = collision.contacts[0];
        Vector3 collisionPoint = contact.point;
        /*Debug.Log(collision.collider.gameObject.name);*/
        GameObject source = Instantiate(audioSourcePrefab, collisionPoint, Quaternion.identity);
        source.transform.parent = null;

        source.GetComponent<AudioSource>().clip = audioClips[Random.Range(0, audioClips.Length)];
        source.GetComponent<AudioSource>().priority = priority;
        source.GetComponent<AudioSource>().volume = volume;
        source.GetComponent<CollisionSoundSource>().Activate();
    }
}
