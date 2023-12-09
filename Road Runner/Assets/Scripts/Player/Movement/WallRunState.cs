using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunState : MovementState
{
    [SerializeField] private float wallRunForce;
    [SerializeField] private float stickToWallForce;
    [SerializeField] private float gravityCounterForce;

    [SerializeField] private float wallJumpUpForce;
    [SerializeField] private float wallJumpSideForce;

    private bool exitingWall;
    [SerializeField] private float exitWallTime;
    private float exitWallTimer;

    public override void EnterState(PlayerMovement playerMovement)
    {
        bool wallLeft = playerMovement.GetWallLeft();
        bool wallRight = playerMovement.GetWallRight();

        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        if (wallLeft)
            cameraController.SetTilt(-5f);
        if (wallRight)
            cameraController.SetTilt(5f);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {
        bool wallLeft = playerMovement.GetWallLeft();
        bool wallRight = playerMovement.GetWallRight();

        Vector3 wallNormal = wallRight ? playerMovement.GetWallRightHit().normal : playerMovement.GetWallLeftHit().normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rigidbody.AddForce(playerMovement.GetVerticalInput() * wallRunForce * wallForward, ForceMode.Force);

        if (!(wallLeft && playerMovement.GetHorizontalInput() > 0) && !(wallRight && playerMovement.GetHorizontalInput() < 0))
            rigidbody.AddForce(-wallNormal * stickToWallForce, ForceMode.Force);

       rigidbody.AddForce(Vector3.up * gravityCounterForce, ForceMode.Force);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        exitingWall = true;

        cameraController.SetTilt(0f);
    }

    public override void Jump(PlayerMovement playerMovement)
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        bool wallRight = playerMovement.GetWallRight();
        Vector3 wallNormal = wallRight ? playerMovement.GetWallRightHit().normal : playerMovement.GetWallLeftHit().normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        rigidbody.AddForce(forceToApply, ForceMode.Impulse);
    }
}
