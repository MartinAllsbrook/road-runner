using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InAirState : MovementState
{
    public override void EnterState(PlayerMovement playerMovement)
    {
        base.EnterState(playerMovement);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {
        Vector3 moveDirection = orientation.forward * playerMovement.GetVerticalInput() + orientation.right * playerMovement.GetHorizontalInput();
        Vector3 velocityNoY = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        if (velocityNoY.magnitude > maxSpeed)
        {
            Vector3 tangentMoveVector = Vector3.Project(moveDirection, velocityNoY);
            moveDirection -= tangentMoveVector;
        }

        rigidbody.AddForce(moveForce * moveDirection.normalized, ForceMode.Force);
        particleController.StopSlowingDownParticles();
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
    }

    public override void Jump(PlayerMovement playerMovement)
    {

    }
}
