/*using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingController : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerTransform;
    private PlayerController playerController;
    private Rigidbody rigidbody;

    [Header("Sliding")] 
    [SerializeField] private float maxSlideTime;
    [SerializeField] private float maxSpeedUpTime;
    [SerializeField] private float maxSlideSpeed;
    [SerializeField] private float slopeSlideForce;
    [SerializeField] private float groundSlideForce;
    private Vector3 slideStartedInputDirection;
    private float slideTime;

    [SerializeField] private float slideYScale;
    private float startYScale;

    [Header("Input")] [SerializeField] private KeyCode slideKey = KeyCode.LeftControl;
    private float horizontalInput;
    private float verticalInput;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
        
        startYScale = playerTransform.localScale.y;
    }

    private void Update()
    {
        if (PlayerSpawner.localPlayerSpawner.Paused)
            return;
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0))
            StartSlide();

        if (Input.GetKeyUp(slideKey) && playerController.sliding)
            StopSlide();
    }

    private void FixedUpdate()
    {
        if (playerController.sliding)
            SlidingMovement();
    }

    private void StartSlide()
    {
        playerController.sliding = true;
        playerTransform.localScale =
            new Vector3(playerTransform.localScale.x, slideYScale, playerTransform.localScale.z);
        
        rigidbody.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideStartedInputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        slideTime = maxSlideTime;
    }

    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Going down slope VS up or off slope
        if (!playerController.onSlope || rigidbody.velocity.y > -0.1f)
        {
            SlopelessMovement();
        }
        else
        {
            rigidbody.AddForce(playerController.GetSlopeMoveDirection(inputDirection).normalized * slopeSlideForce, ForceMode.Force);
        }

        
        if (slideTime <= 0)
            StopSlide();
    }

    private void SlopelessMovement()
    {
        if (slideTime > maxSlideTime - maxSpeedUpTime && playerController.grounded)
        {
            Vector3 velocityNoY = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            Vector3 targetVelocity = maxSlideSpeed * slideStartedInputDirection.normalized;
            Vector3 newMoveDirection = targetVelocity - velocityNoY;

            if (newMoveDirection.magnitude < 1)
            {
                rigidbody.AddForce(groundSlideForce * newMoveDirection, ForceMode.Force);
            }
            else
            {
                rigidbody.AddForce(groundSlideForce * newMoveDirection.normalized, ForceMode.Force);
            }
        }

        slideTime -= Time.deltaTime;
    }

    private void StopSlide()
    {
        playerController.sliding = false;
        
        playerTransform.localScale =
            new Vector3(playerTransform.localScale.x, startYScale, playerTransform.localScale.z);
    }
}
*/