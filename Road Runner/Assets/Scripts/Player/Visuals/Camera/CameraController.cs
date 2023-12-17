using Mono.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CameraController : NetworkBehaviour
{
    public static CameraController Instance; // This singleton instance is only used by the PausedUI/settings at the moment

    [Header("Camera")]
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private Transform orientation;
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;
    [SerializeField] private float fovMultiplier;
    [SerializeField] private Vector3 cameraLimboPosition;

    public Vector2 Sensitivity
    {
        get { return new Vector2(sensX, sensY); }
        set 
        { 
            sensX = value.x;
            sensY = value.y;
        }
    }

    private float cameraTilt;
    private float _xRotation;
    private float _yRotation;
    private Camera mainCamera;
    private float zoom = 1f;
    private Rigidbody playerRigidbody;

    private bool _inLimbo = false;
    private bool _cameraLocked = false;
    public bool CameraLocked
    {
        get { return _cameraLocked; }
        set 
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
                
            _cameraLocked = value; 
        }
    }

    void Start()
    {
        if (!IsOwner) 
            return;

        if (Instance == null)
            Instance = this;
        
        playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (!IsOwner || _cameraLocked) 
            return;
        
        if (_inLimbo)
        {
            LimboUpdate();
            return;
        }

        GetCameraInputs();
        RotateCamera();
    }

    private void LateUpdate()
    {
        if (!IsOwner)
            return;

        if (_inLimbo)
        {
            LimboUpdate();
            return;
        }

        mainCamera.transform.position = cameraPosition.position;

        Vector3 velocityNoY = new Vector3 (playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
        SetFov(velocityNoY.magnitude);
    }

    public void SetLimbo(bool inLimbo)
    {
        _inLimbo = inLimbo;
    }

    private void LimboUpdate()
    {
        mainCamera.transform.position = cameraLimboPosition;
        mainCamera.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
    }

    private void GetCameraInputs()
    {
        if (PlayerSpawner.localPlayerSpawner.Paused)
            return;
        
        float mouseX = Input.GetAxisRaw("Mouse X") * (sensX / zoom);
        float mouseY = Input.GetAxisRaw("Mouse Y") * (sensY / zoom);

        _yRotation += mouseX;
        _xRotation -= mouseY;

        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
    }

    private void RotateCamera()
    {
        Quaternion rotation = Quaternion.Euler(_xRotation, _yRotation, cameraTilt);
        mainCamera.transform.rotation = rotation;
        cameraPosition.transform.rotation = rotation;

        orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
    }

    #region Camera FX Methods

    public void SetFov(float fovIncrease)
    {
        mainCamera.fieldOfView = (58 + fovIncrease * fovMultiplier) / zoom;
    }

    public void SetTilt(float tilt)
    {
        if(!IsOwner)
            return;
        
        cameraTilt = tilt;
        
        Vector3 cameraEulerAnles = mainCamera.transform.rotation.eulerAngles;
        mainCamera.transform.rotation = Quaternion.Euler(cameraEulerAnles.x, cameraEulerAnles.y, tilt);
        
    }

    public void SetZoom(float multiplier)
    {
        zoom = multiplier;
    }

    #endregion
}
