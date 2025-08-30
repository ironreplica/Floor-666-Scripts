using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Player player;
    [SerializeField] private GameObject menuCanvas;
    [SerializeField] private GameObject loadingCanvas;

    [SerializeField] private bool isMainMenu;
    [SerializeField] private Animator elevatorAnimator;

    [SerializeField] private Slider loadingSlider;
    public void Play()
    {
        menuCanvas.SetActive(false);
        // Start the animation for elevator
        // once th doors shut, load
        // once the level loads, start the player in an elevator in the same position and open the doors.
        if (isMainMenu)
        {
            elevatorAnimator.SetTrigger("CloseDoors");
            
        }
        else
        {
            StartCoroutine(loadLevelAsync());
            loadingCanvas.SetActive(true);
        }
    }
    public void Calibrate()
    {

    }
    private void CalibrateHeight()
    {
        /* - Grab the height of the headset relative to the floor
         * 
         * 
         */
    }
    private void CalibrateArmLength()
    {
        /* Make the player TPose
         * - Calculate the distance between arms and HMD (Head mounted display)
         * - Whichever value is bigger use that and store it in a variable
         * - Get the length of the default arms
         * - Divide the default length by the distance between arms and HMD
         * - Use this reciprical to multiply the the current length of arms
         * - Apply changes
         */
    }
    public void Exit()
    {
        // load to main menu?
        Application.Quit();
    }
    void Start()
    {
        RenderSettings.fogEndDistance = 7;
        player.DisablePlayerMove();

    }
    IEnumerator loadLevelAsync()
    {
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(1);
        while (!loadOperation.isDone)
        {
            float progress = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progress;
            yield return null;
        }
    }
    public void AfterElevatorDoorClose()
    {
        StartCoroutine(loadLevelAsync());
        loadingCanvas.SetActive(true);
    }
}
