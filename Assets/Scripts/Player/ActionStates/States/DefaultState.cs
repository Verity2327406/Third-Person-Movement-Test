using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultState : ActionBaseState
{
    public override void EnterState(ActionStateManager action)
    {

    }

    public override void UpdateState(ActionStateManager action)
    {
        action.rHandAim.weight = Mathf.Lerp(action.rHandAim.weight, 1, 10 * Time.deltaTime);
        action.lHandIk.weight = Mathf.Lerp(action.rHandAim.weight, 1, 10 * Time.deltaTime);

        if (action._ir.isReload && CanReload(action))
            action.SwitchState(action.Reload);
    }

    bool CanReload(ActionStateManager action)
    {
        if (action.ammo.curAmmo == action.ammo.clipSize) return false;
        else if(action.ammo.extraAmmo==0) return false;
        else return true;
    }
}
