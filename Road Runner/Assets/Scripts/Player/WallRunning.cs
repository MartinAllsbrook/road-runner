/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("wall Running")] 
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float wallRunForce;
    [SerializeField] private float wallJumpUpForce;
    [SerializeField] private float wallJumpSideForce;
    [SerializeField] private float maxWallRunTime;
    [SerializeField] private float stickToWallForce;
    private float wallRunTimer;

    [Header("Input")] 
    [SerializeField] private KeyCode jumpKey;
    private float horizontalInput;
    private float verticalInput;

    [Header("Detection")] 
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exiting")] 
    private bool exitingWall;
    [SerializeField] private float exitWallTime;
    private float exitWallTimer;

    [Header("Gravity")] 
    [SerializeField] private bool useGravity;
    [SerializeField] private float gravityCounterForce;

    [Header("References")] 
    [SerializeField] private Transform orientation;
    private PlayerController playerController;
    private Rigidbody rigidbody;

    private CameraController cameraController;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (playerController.wallRunning)
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        Ray rightRay = new Ray(transform.position, orientation.right);
        wallRight = Physics.Raycast(rightRay, out rightWallHit, wallCheckDistance, whatIsWall);

        Ray leftRay = new Ray(transform.position, -orientation.right);
        wallLeft = Physics.Raycast(leftRay, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool IsAboveGround()
    {
        Ray downRay = new Ray(transform.position, Vector3.down);
        return !Physics.Raycast(downRay, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        if (PlayerSpawner.localPlayerSpawner.Paused)
            return;
     
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if ((wallLeft || wallRight) && IsAboveGround() && !exitingWall)
        {
            if(!playerController.wallRunning)
                StartWallRun();

            if (wallRunTimer > 0)
                wallRunTimer -= Time.deltaTime;

            if (wallRunTimer <= 0 && playerController.wallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }
            
            if (Input.GetKeyDown(jumpKey)) 
                WallJump();
        }
        else if (exitingWall)
        {
            if(playerController.wallRunning)
                StopWallRunning();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }
        else
        {
            if (playerController.wallRunning)
                StopWallRunning();
        }
    }

    private void StartWallRun()
    {
        playerController.wallRunning = true;

        wallRunTimer = maxWallRunTime;
        
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        
        if (wallLeft)
            cameraController.SetTilt(-5f);
        if (wallRight)
            cameraController.SetTilt(5f);
    }

    private void WallRunningMovement()
    {
        rigidbody.useGravity = useGravity;
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        
        rigidbody.AddForce(verticalInput * wallRunForce * wallForward  , ForceMode.Force);
        
        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
            rigidbody.AddForce(-wallNormal * stickToWallForce, ForceMode.Force);
        
        if (useGravity)
            rigidbody.AddForce(Vector3.up * gravityCounterForce, ForceMode.Force);
    }

    private void StopWallRunning()
    {
        exitingWall = true;

        playerController.wallRunning = false;
        
        cameraController.SetTilt(0f);
    }

    private void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);
        rigidbody.AddForce(forceToApply, ForceMode.Impulse);
    }
}
*/