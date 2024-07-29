using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using TMPro;

public class WeaponAmmo : MonoBehaviour
{
    public int clipSize;
    public int extraAmmo;
    public int maxExtraAmmo;
    [HideInInspector] public int curAmmo;

    private TMP_Text ammoText;
    public AudioClip magInSound, magOutSound, slideSound;

    private void Start()
    {
        curAmmo = clipSize;
        ammoText =  GameObject.Find("ReloadText").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        ammoText.text = string.Format("{0}/{1}", curAmmo, extraAmmo);
    }

    public void Reload()
    {
        if(extraAmmo >= clipSize)
        {
            int ammoToReload = clipSize - curAmmo;
            extraAmmo -= ammoToReload;
            curAmmo += ammoToReload;
        } else if(extraAmmo > 0)
        {
            if(extraAmmo + curAmmo > clipSize)
            {
                int leftOverAmmot = extraAmmo + curAmmo - clipSize;
                extraAmmo -= leftOverAmmot;
                curAmmo = clipSize;
            } else
            {
                curAmmo += extraAmmo;
                extraAmmo = 0;
            }
        }
    }
}
