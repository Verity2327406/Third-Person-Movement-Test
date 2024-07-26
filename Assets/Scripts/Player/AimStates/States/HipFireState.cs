using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipFireState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        aim._a.SetBool("Aiming", false);
        aim.curFov = aim.hipFov;
    }

    public override void UpdateState(AimStateManager aim)
    {
        if(aim._ir.ADSValue && aim.movement.weaponManager.canShoot) aim.SwitchState(aim.Aim);
    }
}
