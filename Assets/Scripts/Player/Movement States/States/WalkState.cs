using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement._a.SetBool("Walking", true);
    }

    public override void UpdateState(MovementStateManager movement)
    {
        if (movement.isSprint) ExitState(movement, movement.Run);
        else if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);

        if (movement.yInput < 0) movement.curMoveSpeed = movement.walkBackSpeed;
        else movement.curMoveSpeed = movement.walkSpeed;
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement._a.SetBool("Walking", false);
        movement.SwitchState(state);
    }
}
