using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchingState : MovementState
{
    [SerializeField] private float crouchYScale;

    private float startYScale;

    protected override void Start()
    {
        base.Start();
        startYScale = transform.localScale.y;
    }

    public override void EnterState(PlayerMovement playerMovement)
    {
        base.EnterState(playerMovement);
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
    }

    public override void UpdateState(PlayerMovement playerMovement)
    {
        base.UpdateState(playerMovement);
    }

    public override void ExitState(PlayerMovement playerMovement)
    {
        base.ExitState(playerMovement);
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
    }

    public override void Jump(PlayerMovement playerMovement)
    {
        base.Jump(playerMovement);
    }
}
