using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvidencePhotoEnableObject : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToEnable;
    public void PhotoTaken()
    {
        foreach(GameObject obj in objectsToEnable)
        {
            obj.SetActive(true);
        }
    }
}
