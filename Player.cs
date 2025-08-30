using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Player : MonoBehaviour
{
    [Tooltip("Continuous move provider script, to be able to disable players ability to move while being jumpscared.")]
    [SerializeField] private ActionBasedContinuousMoveProvider _moveProvider;

    [Tooltip("Snap turn provider script, to be able to disable players ability to snap look while being jumpscared.")]
    [SerializeField] private ActionBasedSnapTurnProvider _snapProvider;

    [SerializeField] private float maxLives = 3;
    private float lives;

    private void Awake()
    {
        lives = maxLives;
    }

    #region Toggle move and snap-turn functionality
    public void EnablePlayerMove()
    {
        _moveProvider.enabled = true;
    }
    public void DisablePlayerMove()
    {
        _moveProvider.enabled = false;
    }
    public void EnablePlayerSnapLook()
    {
        _snapProvider.enabled = true;
    }
    public void DisablePlayerSnapLook()
    {
        _snapProvider.enabled = false;
    }
    #endregion
    #region
    public void UseLife()
    {
        lives -= 1;
    }
    public float GetLives()
    {
        return lives;
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
