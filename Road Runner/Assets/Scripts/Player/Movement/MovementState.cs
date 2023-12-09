using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class MovementState : MonoBehaviour
{
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float moveForce;


    [SerializeField] protected PlayerMovement.StateEnum stateEnum;

    [SerializeField] private float jumpForce;


    private bool exitingSlope;

    public PlayerMovement.StateEnum StateEnum
    {
        get { return stateEnum; }
        private set { }
    }

    protected CameraController cameraController;
    protected Rigidbody rigidbody;
    protected Transform orientation;
    protected ParticleController particleController;

    protected virtual void Start()
    {

        orientation = transform.GetChild(0);
        
        cameraController = GetComponent<CameraController>();
        rigidbody = GetComponent<Rigidbody>();
        particleController = GetComponent<ParticleController>();
    }

    public virtual void EnterState(PlayerMovement playerMovement)
    {

    }

    public virtual void UpdateState(PlayerMovement playerMovement)
    {
        // Calculate move direction
        Vector3 moveDirection = orientation.forward * playerMovement.GetVerticalInput() + orientation.right * playerMovement.GetHorizontalInput();
        Vector3 targetVelocityNoY = moveDirection.normalized * maxSpeed;
        Vector3 velocityNoY = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);

        Vector3 newMoveDirection = targetVelocityNoY - velocityNoY; ;

        

        if (playerMovement.GetOnSlope() && !exitingSlope)
        {
            rigidbody.useGravity = false;

            newMoveDirection = playerMovement.GetSlopeMoveDirection(newMoveDirection);

            if (newMoveDirection.magnitude < 1)
            {
                rigidbody.AddForce(moveForce * newMoveDirection, ForceMode.Force);
                particleController.StopSlowingDownParticles();
            }
            else
            {
                rigidbody.AddForce(moveForce * newMoveDirection.normalized, ForceMode.Force);
                particleController.SetSlowingDownParticles(-newMoveDirection.normalized, rigidbody.velocity.magnitude);
            }

            if (rigidbody.velocity.y > 0)
            {
                rigidbody.AddForce(Vector3.down * 1000f, ForceMode.Force);
            }
        }
        else
        {
            rigidbody.useGravity = true;

            if (newMoveDirection.magnitude < 1)
            {
                rigidbody.AddForce(moveForce * newMoveDirection, ForceMode.Force);
                particleController.StopSlowingDownParticles();
            }
            else
            {
                rigidbody.AddForce(moveForce * newMoveDirection.normalized, ForceMode.Force);
                particleController.SetSlowingDownParticles(-newMoveDirection.normalized, rigidbody.velocity.magnitude);
            }
        }
    }


    public virtual void ExitState(PlayerMovement playerMovement)
    {
        rigidbody.useGravity = true;
    }

    public virtual void Jump(PlayerMovement playerMovement)
    {
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0f, rigidbody.velocity.z);

        rigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
}
