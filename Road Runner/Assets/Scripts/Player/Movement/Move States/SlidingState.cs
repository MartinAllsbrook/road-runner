using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingState : MovementState
{
    [SerializeField] private float maxSlideSpeed;
    [SerializeField] private float startSlideForce;
    [SerializeField] private float slopeSlideForce;
    [SerializeField] private float groundSlideForce;
    [SerializeField] private float maxSlideTime = 0.5f;
    [SerializeField] private float slideCooldownTime = 1.5f;
    [SerializeField] private float slideYScale;
    private float startYScale;
    private bool slideReady = true;

    protected override void Start()
    {
        base.Start();
        startYScale = transform.localScale.y;
    }

    public override void EnterState(PlayerMovement playerMovement)
    {
        transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);

        rigidbody.AddForce(Vector3.down * 50f, ForceMode.Impulse);

        if (slideReady)
        {
            Vector3 inputDirection = orientation.forward * playerMovement.GetVerticalInput() + orientation.right * playerMovement.GetHorizontalInput();
            rigidbody.AddForce(playerMovement.GetSlopeMoveDirection(inputDirection).normalized * startSlideForce, ForceMode.Impulse);
        }
        StartSlideCooldown(slideCooldownTime);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {
        Vector3 inputDirection = orientation.forward * playerMovement.GetVerticalInput() + orientation.right * playerMovement.GetHorizontalInput();

        if (playerMovement.GetOnSlope() && rigidbody.velocity.y < -0.1f)
        {
            playerMovement.ResetSlideTimer(maxSlideTime);

            rigidbody.AddForce(playerMovement.GetSlopeMoveDirection(inputDirection).normalized * slopeSlideForce, ForceMode.Force);
        }
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        playerMovement.ResetSlideTimer(maxSlideTime);

        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    public override void Jump(PlayerMovement playerMovement)
    {
        base.Jump(playerMovement);
    }

    public void StartSlideCooldown(float time)
    {
        StartCoroutine(SlideCooldown(time));
    }

    private IEnumerator SlideCooldown(float time)
    {
        slideReady = false;
        yield return new WaitForSeconds(time);
        slideReady = true;
    }


}
