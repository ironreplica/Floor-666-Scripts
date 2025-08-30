using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    public bool CompletedLevel;
    private void Awake()
    {
        CompletedLevel = false;
    }
}
