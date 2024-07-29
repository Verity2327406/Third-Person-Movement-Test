using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterMover
{
    void BeginMoveToCover(Vector3 targetPos);
    Vector3 inCoverMoveDirection { get; set; }
    Vector3 inCoverProhibitedDirection { get; set; }
}
