using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] float fireRate;
    float fireRateTimer;
    [SerializeField] bool semiAuto;
    [SerializeField] Transform barrelPos;

    [SerializeField] AudioClip audioClip;

    AudioSource audioSource;
    InputReader _ir;
    DecalPainter _painter;

    WeaponAmmo _weaponAmmo;
    WeaponBloom _weaponBloom;
    ActionStateManager action;
    WeaponRecoil recoil;

    void Start()
    {
        fireRateTimer = fireRate;

        _ir = FindAnyObjectByType<InputReader>().GetComponent<InputReader>();
        _painter = FindAnyObjectByType<DecalPainter>().GetComponent<DecalPainter>();
        _weaponAmmo = GetComponent<WeaponAmmo>();
        _weaponBloom = GetComponent<WeaponBloom>();
        audioSource = FindAnyObjectByType<AudioSource>().GetComponent<AudioSource>();
        action = GetComponentInParent<ActionStateManager>();
        recoil = GetComponent<WeaponRecoil>();
    }

    private void Update()
    {
        if (ShouldFire()) Fire();
    }

    bool ShouldFire()
    {
        fireRateTimer += Time.deltaTime;
        if (fireRateTimer < fireRate) return false;
        if (_weaponAmmo.curAmmo == 0) return false;
        if (action.curState == action.Reload) return false;
        if (semiAuto && _ir.isShootingSemi) return true;
        if (!semiAuto && _ir.isShooting) return true;
        return false;
    }

    void Fire()
    {
        fireRateTimer = 0;
        barrelPos.localEulerAngles = _weaponBloom.BloomAngle(barrelPos);
        audioSource.PlayOneShot(audioClip);
        recoil.TriggerRecoil();
        _weaponAmmo.curAmmo--;

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("Hit: " + hit.collider.name);
            _painter.PaintDecal(hit.point, hit.normal, hit.collider);
        }
    }
}
