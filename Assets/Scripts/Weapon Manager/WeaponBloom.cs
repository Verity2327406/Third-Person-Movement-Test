using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBloom : MonoBehaviour
{
    [SerializeField] float defaultBloomAngle = 3;
    [SerializeField] float walkBloomMultiplier = 1.5f;
    [SerializeField] float sprintBloomMultiplier = 2f;
    [SerializeField] float adsBloomMultiplier = 0.5f;

    MovementStateManager movement;
    AimStateManager aim;

    float curBloom;

    private void Start()
    {
        movement = GetComponentInParent<MovementStateManager>();
        aim = GetComponentInParent<AimStateManager>();
    }

    public Vector3 BloomAngle(Transform barrelPos)
    {
        if (movement.curState == movement.Idle) curBloom = defaultBloomAngle;
        else if (movement.curState == movement.Walk) curBloom = defaultBloomAngle * walkBloomMultiplier;
        else if (movement.curState == movement.Run) curBloom = defaultBloomAngle * sprintBloomMultiplier;

        if(aim.curState == aim.Aim) curBloom *= adsBloomMultiplier;

        float randX= Random.Range(-curBloom, curBloom);
        float randY= Random.Range(-curBloom, curBloom);
        float randZ= Random.Range(-curBloom, curBloom);

        Vector3 randomRoation = new Vector3(randX, randY, randZ);
        return barrelPos.localEulerAngles + randomRoation;
    }
}
