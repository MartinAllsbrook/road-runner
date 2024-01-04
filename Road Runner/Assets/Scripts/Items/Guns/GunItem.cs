using JetBrains.Annotations;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using static Inventory;

public class GunItem : UseableItem
{
    #region Variables

    [Header("References")]
    [SerializeField] private GameObject bullet;

    [Header("Gun Stats")]
    [SerializeField] private int magSize;
    [SerializeField] private bool singleShot;
    [SerializeField] private float damage;
    [SerializeField] private float fireRate;
    [SerializeField] private float reloadTime;
    [SerializeField] private int bulletSpeed;
    [SerializeField] private float zoom = 1.4f;

    [Header("Gun Accuracy")]
    [SerializeField] private float minInaccuracy = 0.05f;
    [SerializeField] private float maxInaccuracy = 1f;
    [SerializeField] private float inaccuracyIncreasePercentPerShot = 0.1f;
    [SerializeField] private float inaccuracyDecreaseRate = 0.1f;
    private float _inaccuracyPercent;
    private float _inaccuracy;

    [Header("Gun Settings")]
    [SerializeField] private Transform bulletExitPoint;
    [SerializeField] private Vector3 aimOffset;
    
    [Header("Gunshots & Audio")]
    [SerializeField] private EffectPool gunshotSoundPool;
    [SerializeField] private GameObject muzzleFlash;
    [SerializeField] protected AudioSource seccondaryUseAudio;
    [SerializeField] protected AudioSource reloadAudio;

    [Header("Attachment Slots")]
    [SerializeField] private Transform[] attachmentPoints;

    private Magazine magazine;
    private bool reloading = false;
    private float timeSinceLastShot;
    private bool triggerLifted = true;
    private int ammoCount;

    #endregion

    private void Start()
    {
        magazine = new Magazine(magSize);
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        DecreaseInaccuracy(); // always decrease inaccuracy because a real person would always be trying to aim better

        if (!triggerLifted && Input.GetKeyUp(KeyCode.Mouse0))
        {
            triggerLifted = true;
        }
    }

    private void LateUpdate()
    {
        if (isOwner)
            HUDController.Instance.SetCrosshaireInaccuracy(_inaccuracy);
    }

    public override void OnSeccondaryUseItemInput(InputAction.CallbackContext context)
    {
        if (!isOwner) return;

        if (context.started)
            StartAim();

        if (context.canceled)
            StopAim();
    }

    private void StartAim()
    {
        //Debug.Log(zoom);
        //transform.position += (aimOffset.x * transform.right + aimOffset.y * transform.up + aimOffset.z * transform.forward);
        parentItemController.SetHandPosition(UseableItemController.HandPosition.Aim);
        parentItemController.GetComponent<CameraController>().SetZoom(zoom);
    }

    private void StopAim()
    {
        parentItemController.SetHandPosition(UseableItemController.HandPosition.Resting);
        parentItemController.GetComponent<CameraController>().SetZoom(1);
    }

    public override void OnUseItemInput()
    {
        TryShootLoop();
    }

    public override void OnReloadItemInput()
    {
/*        if (!reloading)
            StartCoroutine(Reload());*/
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

        parentItemController.HudController.SetAmmoCountDisplay(ammoCount - 1, magSize);
        timeSinceLastShot = 0;
        Fire(_inaccuracy);

    }

    protected virtual void Fire(float accuracy) 
    {
        CreateBullet(accuracy);
        IncreaseInaccuracy();
        parentItemController.UseServerRpc();
    }

    #region Inaccuracy

    private void IncreaseInaccuracy()
    {
        _inaccuracyPercent = Mathf.Lerp(_inaccuracyPercent, 1, inaccuracyIncreasePercentPerShot);
        _inaccuracy = CalculateInaccuracy(_inaccuracyPercent);
    }

    private void DecreaseInaccuracy()
    {
        _inaccuracyPercent -= inaccuracyDecreaseRate * Time.deltaTime;
        _inaccuracyPercent = Mathf.Clamp(_inaccuracyPercent, 0, 1);
        _inaccuracy = CalculateInaccuracy(_inaccuracyPercent);
    }

    private float CalculateInaccuracy(float inaccuracyPercent)
    { 
        return Mathf.Lerp(minInaccuracy, maxInaccuracy, inaccuracyPercent);
    }

    #endregion

    protected void CreateBullet(float accuracy)
    {
        Vector3 velocity = parentItemController.CameraPosition.forward * bulletSpeed;

        Vector3 randomVector3 = UnityEngine.Random.insideUnitSphere;
        Vector3 axis = Vector3.ProjectOnPlane(randomVector3, velocity);

        float inaccuracy = UnityEngine.Random.Range(0f, accuracy);

        velocity = Quaternion.AngleAxis(inaccuracy, axis) * velocity;
        
        Vector3 position = parentItemController.CameraPosition.position;

        BulletPool.Instance.FireBullet(velocity, position, damage);
        //SpawnBulletServerRpc();
    }

    public void SetMag(StoredItemID magazineSIID, int modSlotIndex)
    {
        UniqueItemID magUIID = magazineSIID.UniqueItemID;

        Debug.Log("Equiping a mag in slot " + modSlotIndex);

        uniqueItemID.TryModifyItem(magUIID, modSlotIndex, out UniqueItemID oldMagUIID); // Edit copy in hands

        // TODO: Instantiate the magazine at the respective attachment point and remove the old magazine

        Debug.Log("Equiping a mag of size " + magazineSIID.UniqueItemID.CounterCount + " Containing " + magazineSIID.UniqueItemID.CounterItem);
        magazine = new Magazine(magazineSIID.UniqueItemID.CounterCount);

        // TODO: Check if the magazine is compatible with this gun -- This will probaby be done by the item modification point setting this
        // TODO: Possibly check if the bullets are compatible with this gun -- Again, this will probably be done by the item modification point setting this

    }

    private IEnumerator Reload()
    {
        parentItemController.SetHandPosition(UseableItemController.HandPosition.Reloading);
        reloading = true;
        parentItemController.HudController.PlayReloadUIAnimation(reloadTime);

        yield return new WaitForSeconds(reloadTime);

        reloadAudio.Play();
        parentItemController.SetHandPosition(UseableItemController.HandPosition.Resting);
        parentItemController.HudController.StopReloadUIAnimation();
        magazine.Reload();
        parentItemController.HudController.SetAmmoCountDisplay(magSize, magSize);
        reloading = false;
    }

    public override void UseServerAction()
    {
        base.UseServerAction();
        gunshotSoundPool.PlayEffect();
        StartCoroutine(FlashCoroutine());
    }

    private IEnumerator FlashCoroutine()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.04f);
        muzzleFlash.SetActive(false);
    }
}
