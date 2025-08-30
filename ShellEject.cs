using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this to the shell ejecting game object on a weapon
public class ShellEject : MonoBehaviour
{
    [Tooltip("Shell eject point")]
    [SerializeField] private Transform shellEjectPoint;

    [Tooltip("Shell prefab")]
    [SerializeField] private GameObject shellPrefab;

    [Tooltip("Vector3 direction for shell to be ejected towards relative to the weapon.")]
    [SerializeField] private Vector3 direction;

    [Tooltip("Speed to eject the casing at.")]
    [SerializeField] private float ejectSpeed;

    public void Eject()
    {
        GameObject newCasing = Instantiate(shellPrefab, null);
        newCasing.transform.position = shellEjectPoint.position;
        newCasing.GetComponent<Rigidbody>().AddForce(direction * ejectSpeed);
    }
}
