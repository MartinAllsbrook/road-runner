using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class GunItem : UseableItem
{
    [Header("Gun")]
    [SerializeField] private GameObject bullet;
    //[SerializeField] private Transform cameraPosition;

    [SerializeField] private int magSize;
    [SerializeField] private bool singleShot;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float reloadTime;
    [SerializeField] private int bulletSpeed;
    [SerializeField] private Transform bulletExitPoint;
    [SerializeField] private float accuracy;
    [SerializeField] private Vector3 aimOffset;
    [SerializeField] private float zoom = 1.4f;

    [SerializeField] private GameObject muzzleFlash;

    private Magazine magazine;
    private bool reloading = false;
    private float timeSinceLastShot;
    private bool triggerLifted = true;
    private int ammoCount;

    private void Start()
    {
        magazine = new Magazine(magSize);
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        if (!triggerLifted && Input.GetKeyUp(KeyCode.Mouse0))
        {
            triggerLifted = true;
        }

        GetInputs();
    }

    private void GetInputs()
    {
        if (!isOwner) return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
            StartAim();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            StopAim();
    }

    private void StartAim()
    {
        Debug.Log(zoom);
        transform.position += (aimOffset.x * transform.right + aimOffset.y * transform.up + aimOffset.z * transform.forward);
        UseableItemController.Instance.GetComponent<CameraController>().SetZoom(zoom);
    }

    private void StopAim()
    {
        transform.position -= (aimOffset.x * transform.right + aimOffset.y * transform.up + aimOffset.z * transform.forward);
        UseableItemController.Instance.GetComponent<CameraController>().SetZoom(1);
    }

    public override void UseItem()
    {
        base.UseItem();
        TryShootLoop();
    }

    public override void SeccondaryUseItem()
    {
        base.SeccondaryUseItem();
    }

    public override void ReloadItem()
    {
        base.ReloadItem();

        if (!reloading)
            StartCoroutine(Reload());
    }

    private void TryShootLoop()
    {
        if (reloading)
            return;

        if (singleShot && !triggerLifted)
            return;

        triggerLifted = false;

        if (timeSinceLastShot < fireRate)
            return;


        ammoCount = magazine.ConsumeRound();

        if (ammoCount <= 0)
            return;

        UseableItemController.Instance.HudController.SetAmmoCountDisplay(ammoCount - 1, magSize);
        timeSinceLastShot = 0;
        Fire(accuracy);

        UseableItemController.Instance.UseServerRpc();
    }

    protected virtual void Fire(float accuracy) 
    { 
        CreateBullet(accuracy);
    }

    protected void CreateBullet(float accuracy)
    {
        Vector3 velocity = UseableItemController.Instance.CameraPosition.forward * bulletSpeed;

        Vector3 randomVector3 = UnityEngine.Random.insideUnitSphere;
        Vector3 axis = Vector3.ProjectOnPlane(randomVector3, velocity);

        float inaccuracy = UnityEngine.Random.Range(0f, accuracy);

        velocity = Quaternion.AngleAxis(inaccuracy, axis) * velocity;
        
        Vector3 position = UseableItemController.Instance.CameraPosition.position;

        BulletPool.Instance.FireBullet(velocity, position, damage);
        //SpawnBulletServerRpc();
    }

    private IEnumerator Reload()
    {
        reloading = true;
        UseableItemController.Instance.HudController.PlayReloadUIAnimation(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        reloadAudio.Play();
        UseableItemController.Instance.HudController.StopReloadUIAnimation();
        magazine.Reload();
        UseableItemController.Instance.HudController.SetAmmoCountDisplay(magSize, magSize);
        reloading = false;
    }

    public void PlayReloadAudio()
    {

    }

    public void PlayFireAudio()
    {

    }

    public override void UseServerAction()
    {
        base.UseServerAction();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.04f);
        muzzleFlash.SetActive(false);
    }
}
