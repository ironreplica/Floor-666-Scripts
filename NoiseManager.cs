using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class NoiseManager : MonoBehaviour
{
    private Dictionary<string, float> noiseSourceLevels = new Dictionary<string, float>();
    [SerializeField] private Slider noiseSlider;
    private float currentMaxDB = -100f; // base lowest noise

    public void UpdateNoiseLevel(string sourceId, float dbLevel)
    {
        noiseSourceLevels[sourceId] = dbLevel;
        UpdateMaxDbLevel();
    }

    private void UpdateMaxDbLevel()
    {
        float newMaxDB = noiseSourceLevels.Values.Max();
        if (newMaxDB != currentMaxDB)
        {
            currentMaxDB = newMaxDB;
            UpdateSlider(currentMaxDB);
        }
    }

    private void UpdateSlider(float targetValue)
    {
        noiseSlider.value = targetValue;
       /* Debug.Log($"Slider updated. New value: {noiseSlider.value}");*/
    }
}
