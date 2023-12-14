using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using QFSW.QC;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private float maxSlopeAngle;

    [Header("Wall Check")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float minJumpHeight;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl; // TODO: Merge crouching and sliding
    [SerializeField] private KeyCode walkKey = KeyCode.LeftShift;

    [SerializeField] private Transform orientation;

    [Header("NoClip")]
    [SerializeField] private float noClipSpeed = 10f;
    private bool _inNoClipMode = false;
    private Rigidbody _rigidbody;

    private float slideTimer;

    // Inputs
    private float horizontalInput;
    private float verticalInput;

    private bool jumpInput;
    private bool crouchInput;
    private bool walkInput;

    private bool grounded;
    private bool onSlope;
    private bool wallLeft;
    private bool wallRight;

    private RaycastHit slopeHit;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;

    [SerializeField] private MovementState[] movementStates;

    private StateEnum state;

    public enum StateEnum
    {
        Running,
        Walking,
        InAir,
        Sliding,
        Crouching,
        WallRunning,
    }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!IsOwner || _inNoClipMode)
            return;

        GetInputs();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;
        
        if (_inNoClipMode)
        {
            NoClipFixedUpdated();
            return;
        }

        UpdateState(state);
    }

    #region Input

    private void GetInputs()
    {

        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        onSlope = OnSlope(out slopeHit, maxSlopeAngle);

        Ray rightRay = new Ray(transform.position, orientation.right);
        wallRight = Physics.Raycast(rightRay, out rightWallHit, wallCheckDistance, whatIsWall);

        Ray leftRay = new Ray(transform.position, -orientation.right);
        wallLeft = Physics.Raycast(leftRay, out leftWallHit, wallCheckDistance, whatIsWall);

        ChangeStates();

    }

    public void SetMoveInput(InputAction.CallbackContext context)
    {
        Vector2 direction = context.ReadValue<Vector2>();
        horizontalInput = direction.x;
        verticalInput = direction.y;
    }

    public void SetJumpInput(InputAction.CallbackContext context)
    {
        jumpInput = context.action.IsPressed();
    }

    public void SetCrouchInput(InputAction.CallbackContext context)
    {
        crouchInput = context.action.IsPressed();
    }

    public void SetWalkInput(InputAction.CallbackContext context)
    {
        walkInput = context.action.IsPressed();
    }

    #endregion

    #region State Controll

    private bool OnSlope(out RaycastHit slopeHit, float maxSlopeAngle)
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private void ChangeStates()
    {
        if (grounded)
        {
            if (jumpInput)
            {
                JumpState(state);
                SetState(StateEnum.InAir);
                return;
            }

            if (crouchInput)
            {
                if (slideTimer >= 0 && (horizontalInput != 0 || verticalInput != 0))
                {
                    slideTimer -= Time.deltaTime;
                    SetState(StateEnum.Sliding);
                    return;
                }

                SetState(StateEnum.Crouching);
                return;
            }

            if (walkInput)
            {
                SetState(StateEnum.Walking);
                return;
            }

            SetState(StateEnum.Running);
            return;
        }

        if (wallLeft || wallRight)
        {
            if (jumpInput)
            {
                JumpState(state);
                SetState(StateEnum.InAir);
                return;
            }

            SetState(StateEnum.WallRunning);
            return;
        }

        SetState(StateEnum.InAir);
    }

    private void SetState(StateEnum newState)
    {
        StateEnum previousState = state;
        state = newState;

        if (previousState != state)
        {
            ExitState(previousState);
            EnterState(state);
        }
    }

    private void ExitState(StateEnum state)
    {
        foreach (var movementState in movementStates)
        {
            if (movementState.StateEnum == state)
            {
                movementState.ExitState(this);
                return;
            }
        }
    }

    private void UpdateState(StateEnum state)
    {
        foreach (var movementState in movementStates)
        {
            if (movementState.StateEnum == state)
            {
                movementState.UpdateState(this);
                return;
            }
        }
    }

    private void EnterState(StateEnum state)
    {
        foreach (var movementState in movementStates)
        {
            if(movementState.StateEnum == state)
            {
                movementState.EnterState(this);
                return;
            }
        }
    }

    private void JumpState(StateEnum state)
    {
        foreach (var movementState in movementStates)
        {
            if (movementState.StateEnum == state)
            {
                movementState.Jump(this);
                return;
            }
        }
    }

    public void ResetSlideTimer(float time)
    {
        slideTimer = time;
    }
    
    #endregion

    #region Getters

    public float GetHorizontalInput()
    {
        return horizontalInput;
    }

    public float GetVerticalInput()
    {
        return verticalInput;
    }

    public bool GetWallRight()
    {
        return wallRight;
    }

    public bool GetWallLeft()
    {
        return wallLeft;
    }

    public RaycastHit GetWallRightHit()
    {
        return rightWallHit;
    }

    public RaycastHit GetWallLeftHit()
    {
        return leftWallHit;
    }

    public bool GetOnSlope()
    {
        return onSlope;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }

    #endregion

    #region NoClip

    [Command]
    private void ToggleNoClip()
    {
        if (_inNoClipMode)
        {
            _inNoClipMode = false;
            _rigidbody.useGravity = true;
        }
        else
        {
            _inNoClipMode = true;
            _rigidbody.useGravity = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleNoClipServerRpc()
    {
        ToggleNoClipClientRpc();
    }

    [ClientRpc]
    private void ToggleNoClipClientRpc()
    {
        // Do stuff if this is needed
        // Github Copilot said do nothing lol
    }

    private void NoClipFixedUpdated()
    {
        float upDownInput = 0;
        if (jumpInput)
            upDownInput += 1;
        if (crouchInput)
            upDownInput -= 1;

        Vector3 moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput + orientation.up * upDownInput;
        Vector3 newPosition = transform.position + moveDirection * noClipSpeed * Time.fixedDeltaTime;

        _rigidbody.MovePosition(newPosition);
    }

    #endregion
}
