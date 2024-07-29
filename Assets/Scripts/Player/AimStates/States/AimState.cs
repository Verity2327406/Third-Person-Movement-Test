using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimState : AimBaseState
{
    public override void EnterState(AimStateManager aim)
    {
        aim._a.SetBool("Aiming", true);
        aim.curFov = aim.adsFov;
    }

    public override void UpdateState(AimStateManager aim)
    {
        if (!aim._ir.ADSValue) aim.SwitchState(aim.Hip);
    }
}
