using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MagazineController : MonoBehaviour
{
    [SerializeField] private int curAmmo;
    [SerializeField] private int maxAmmo;

    [SerializeField] private TextMeshProUGUI text;
    private void Start()
    {
        curAmmo = maxAmmo;
        setText();
        
    }
    /// <summary>
    /// Get the guns current ammo.
    /// </summary>
    /// <returns>Int</returns>
    public int GetCurAmmo()
    {
        return curAmmo;
    }
    /// <summary>
    /// Get the guns max ammo.
    /// </summary>
    /// <returns>Int</returns>
    public int GetMaxAmmo()
    {
        return maxAmmo;
    }
    /// <summary>
    /// Use a bullet from the guns current ammo. Check if you have bullets to take from first.
    /// </summary>
    /// <param name="amount"></param>
    public void UseBullet(int amount)
    {
        curAmmo -= amount;
        setText();
    }
    private void setText()
    {
        text.text = curAmmo + "/" + maxAmmo;
    }
}
