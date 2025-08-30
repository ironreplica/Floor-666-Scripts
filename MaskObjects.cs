using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskObjects : MonoBehaviour
{
    public GameObject[] maskObjs;
    void Start()
    {
        for(int i = 0; i < maskObjs.Length; i++)
        {
            maskObjs[i].GetComponent<MeshRenderer>().material.renderQueue = 3002;
        }   
    }

}
