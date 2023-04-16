using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    // IS PLAYER ON THE GROUND
    void CheckGround();

    // HANDLE PLAYER MOVEMENTS
    void HandleMovement();
}
