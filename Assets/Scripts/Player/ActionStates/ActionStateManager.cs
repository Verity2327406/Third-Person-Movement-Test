using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ActionStateManager : MonoBehaviour
{
    [HideInInspector] public ActionBaseState curState;
    public ReloadState Reload = new();
    public DefaultState Default = new();

    public GameObject curWeapon;
    public WeaponAmmo ammo;

    [HideInInspector] public InputReader _ir;
    [HideInInspector] public Animator _a;

    public MultiAimConstraint rHandAim;
    public TwoBoneIKConstraint lHandIk;

    AudioSource audioSource;

    private void Start()
    {
        _ir = FindAnyObjectByType<InputReader>().GetComponent<InputReader>();
        _a = GetComponent<Animator>();
        audioSource = FindAnyObjectByType(typeof(AudioSource)) as AudioSource;

        SwitchState(Default);

        ammo = curWeapon.GetComponent<WeaponAmmo>();
    }

    private void Update()
    {
        curState.UpdateState(this);
    }

    public void SwitchState(ActionBaseState state)
    {
        curState = state;
        curState.EnterState(this);
    }

    public void WeaponReloaded()
    {
        ammo.Reload();
        SwitchState(Default);
    }

    public void MagOut()
    {
        audioSource.PlayOneShot(ammo.magOutSound);
    }

    public void MagIn()
    {
        audioSource.PlayOneShot(ammo.magInSound);
    }

    public void ReleaseSlide()
    {
        audioSource.PlayOneShot(ammo.slideSound);
    }
}
