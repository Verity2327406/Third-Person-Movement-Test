using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.cover = true;
        movement._a.SetBool("Cover", true);
        movement.transform.GetChild(1).localRotation = new Quaternion(0, 180, 0, 0);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if(movement.ir.MovementValue.y != 0)
        {
            ExitState(movement, movement.Walk);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.cover = false;
        movement._a.SetBool("Cover", false);
        movement.transform.GetChild(1).localRotation = new Quaternion(0, 0, 0, 0);
        movement.SwitchState(state);
    }
}
