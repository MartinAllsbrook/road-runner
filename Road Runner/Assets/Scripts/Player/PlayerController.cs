using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController Instance;

    [Header("Movement")]
    [SerializeField] private float moveForce;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float wallRunningSpeed;
    [SerializeField] private float groundDrag;

    [SerializeField] private float speedIncreaseMultiplier;
    [SerializeField] private float slopeIncreaseMultiplier;

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private float moveSpeed;
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _moveDirection;

    [Header("Air")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier; // TODO: Lets make this variable more specific
    private bool _readyToJump = true;

    [Header("Crouching")]
    [SerializeField] private float crouchYScale;
    private float startYScale;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.C; // TODO: Merge crouching and sliding

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    public bool onSlope;
    private bool exitingSlope;

    [Header("References")]
    [SerializeField] private Transform orientation;
    private Rigidbody _rigidbody;
    private CameraController cameraController;
    private ParticleController particleController;

    public bool grounded;
    public bool sliding;
    public bool wallRunning;

    private MovementState state;
    private enum MovementState
    {
        walking,
        wallRunning,
        crouching,
        sliding,
        air
    }

    void Start()
    {
        if (!IsOwner)
            return;

        if (Instance == null)
            Instance = this;

        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.freezeRotation = true;

        cameraController = GetComponent<CameraController>();
        particleController = GetComponent<ParticleController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        startYScale = transform.localScale.y;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        MovementHUD.Instance.SetGroundedDisplay(grounded);

        onSlope = OnSlope();
        MovementHUD.Instance.SetOnSlopeDisplay(onSlope);

        HandleMoveInput();
        MoveStateHandler();

        MovementHUD.Instance.SetSpeedDisplay(_rigidbody.velocity.magnitude);
        MovementHUD.Instance.SetMaxMoveSpeedDisplay(moveSpeed);

        if (grounded)
            _rigidbody.drag = groundDrag;
        else
            _rigidbody.drag = 0;

        Vector3 velocityNoY = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        cameraController.SetFov(velocityNoY.magnitude);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        MovePlayer();
    }

    private void HandleMoveInput()
    {
        if (PlayerSpawner.localPlayerSpawner.Paused)
        {
            _horizontalInput = 0;
            _verticalInput = 0;
            return;
        }

        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");

        if (Input.GetKey(jumpKey) && _readyToJump && grounded)
        {
            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            _rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void MoveStateHandler()
    {
        if (wallRunning)
        {
            state = MovementState.wallRunning;
            desiredMoveSpeed = wallRunningSpeed;
        }
        else if (sliding)
        {
            state = MovementState.sliding;

            if (onSlope && _rigidbody.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;
            else
                desiredMoveSpeed = slideSpeed;
        }
        else if (grounded && Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }
        else
        {
            state = MovementState.air;
            desiredMoveSpeed = walkSpeed;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4 && moveSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(LerpMoveSpeed());
        }
        else
        {
            moveSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator LerpMoveSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            if (onSlope)
            {
                float slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
                float slopeAngleIncrease = 1 + (slopeAngle / 90f);

                time += Time.deltaTime * speedIncreaseMultiplier * slopeIncreaseMultiplier * slopeAngleIncrease;
            }
            else
            {
                time += Time.deltaTime * speedIncreaseMultiplier;
            }

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
    }

    private void MovePlayer()
    {
        // Calculate move direction [[ Move to get inputs? ]]
        _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        Vector3 newMoveDirection = _moveDirection;
        Vector3 velocityNoY = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);

        // remove component of new moveForce that would add velocity to the player
        if (_rigidbody.velocity.magnitude > moveSpeed)
        {
            Vector3 tangentMoveVector = Vector3.Project(_moveDirection, velocityNoY);
            newMoveDirection = _moveDirection - tangentMoveVector;
        }


        if (onSlope && !exitingSlope)
        {
            Vector3 targetVelocity = _moveDirection.normalized * moveSpeed;
            newMoveDirection = targetVelocity - velocityNoY;
            newMoveDirection = GetSlopeMoveDirection(newMoveDirection);

            if (newMoveDirection.magnitude < 1)
            {
                _rigidbody.AddForce(moveForce * newMoveDirection, ForceMode.Force);
                particleController.StopSlowingDownParticles();
            }
            else
            {
                _rigidbody.AddForce(moveForce * newMoveDirection.normalized, ForceMode.Force);
                particleController.SetSlowingDownParticles(-newMoveDirection.normalized, _rigidbody.velocity.magnitude);
            }

            if (_rigidbody.velocity.y > 0)
            {
                _rigidbody.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

        }
        else if (sliding)
        {
            // _rigidbody.AddForce(moveForce * airMultiplier * newMoveDirection.normalized, ForceMode.Force);
        }
        else if (grounded)
        {
            Vector3 targetVelocity = _moveDirection.normalized * moveSpeed;
            newMoveDirection = targetVelocity - velocityNoY;

            if (newMoveDirection.magnitude < 1)
            {
                _rigidbody.AddForce(moveForce * newMoveDirection, ForceMode.Force);
                particleController.StopSlowingDownParticles();
            }
            else
            {
                _rigidbody.AddForce(moveForce * newMoveDirection.normalized, ForceMode.Force);
                particleController.SetSlowingDownParticles(-newMoveDirection.normalized, _rigidbody.velocity.magnitude);
            }
        }
        else if (!grounded)
        {
            _rigidbody.AddForce(moveForce * airMultiplier * newMoveDirection.normalized, ForceMode.Force);
            particleController.StopSlowingDownParticles();
        }

        if (!wallRunning)
            _rigidbody.useGravity = !onSlope;


    }

    private void Jump()
    {
        _readyToJump = false;
        exitingSlope = true;

        _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);

        _rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        _readyToJump = true;
        exitingSlope = false;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal);
    }
}
