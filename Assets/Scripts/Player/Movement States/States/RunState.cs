using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement._a.SetBool("Running", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (!movement.isSprint) ExitState(movement, movement.Walk);
        else if(movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);

        if (movement.yInput < 0) movement.curMoveSpeed = movement.runBackSpeed;
        else movement.curMoveSpeed = movement.runSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement._a.SetBool("Running", false);
        movement.SwitchState(state);
    }
}
