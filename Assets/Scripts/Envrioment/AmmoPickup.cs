using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    int amount;
    WeaponAmmo ammo;

    private void Start()
    {
        ammo = GameObject.FindGameObjectWithTag("Player").GetComponent<ActionStateManager>().curWeapon.GetComponent<WeaponAmmo>();
        amount = ammo.clipSize / 2;
    }

    private void TryPickup()
    {
        if (ammo.extraAmmo == ammo.maxExtraAmmo) return;
        int newAmmo = ammo.extraAmmo += amount;
        ammo.extraAmmo = Mathf.Min(ammo.maxExtraAmmo, ammo.extraAmmo);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickup();
        }
    }
}
