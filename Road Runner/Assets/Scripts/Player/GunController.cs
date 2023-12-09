/*using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GunController : NetworkBehaviour
{
    [Header("Gun")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private Transform cameraPosition;
    [SerializeField] private AudioSource gunFireAudio;
    [SerializeField] private AudioSource reloadAudio;

    [Header("Inputs")]
    [SerializeField] private KeyCode fireKey = KeyCode.Mouse0;
    [SerializeField] private KeyCode reloadKey = KeyCode.R;

    [SerializeField] private GunSO gunSo;
    private Magazine magazine;

    [SerializeField] private Transform gunModel;

    private bool reloading = false;
    private HUDController hudController;

    private float timeSinceLastShot;
    private bool triggerLifted;
    
    *//*public struct NewBulletData : INetworkSerializable
    {
        public int OwnerId;
        public Vector3 ExitPoint;
        public Vector3 ExitVelocity;
    
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref OwnerId);
            serializer.SerializeValue(ref ExitPoint);
            serializer.SerializeValue(ref ExitVelocity);
        }
    }*//*

    private void Start()
    {
        if (!IsOwner) 
            return;

        hudController = GameObject.Find("HUD").GetComponent<HUDController>();
        magazine = new Magazine(gunSo.magSize);
    }

    private void Update()
    {
        if (!IsOwner)
            return;
        
        timeSinceLastShot += Time.deltaTime;

        GetInputs();
    }

    private void GetInputs()
    {
        if (HUDController.FreezeInputs)
             return;
        
        if (Input.GetKey(fireKey))
            TryShootLoop();

        if (Input.GetKeyUp(fireKey))
            triggerLifted = true;

        if (Input.GetKeyDown(reloadKey))
            StartCoroutine(Reload());
    }

    public void SetGun(GunSO newGun)
    {
        gunSo = newGun;
    }

    private void TryShootLoop()
    {
        if (reloading)
            return;

        if (gunSo.singleShot && !triggerLifted)
            return;
        
        triggerLifted = false;
        
        if (timeSinceLastShot < gunSo.fireRate)
            return;
        
        int ammoCount = magazine.ConsumeRound();
        hudController.SetAmmoCountDisplay(ammoCount, gunSo.magSize);

        if (ammoCount <= 0)
            return;

        timeSinceLastShot = 0;
        CreateBullet();
    }
    
    private void Shoot()
    {

    }

    private void CreateBullet()
    {
        Vector3 velocity = cameraPosition.forward * gunSo.bulletSpeed;
        Vector3 position = cameraPosition.position;
        
        BulletPool.Instance.FireBullet(velocity, position, gunSo.damage);
        // Instantiate(bulletTransform, position, rotation);
        SpawnBulletServerRpc();
    }

    private IEnumerator Reload()
    {
        reloading = true;
        hudController.PlayReloadUIAnimation(gunSo.reloadTime);
        
        yield return new WaitForSeconds(gunSo.reloadTime);
        
        reloadAudio.Play();
        hudController.StopReloadUIAnimation();
        magazine = new Magazine(gunSo.magSize);
        hudController.SetAmmoCountDisplay(gunSo.magSize, gunSo.magSize);
        reloading = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnBulletServerRpc()
    {
        NetworkObject playerNetworkObject = NetworkManager.Singleton.ConnectedClients[OwnerClientId].PlayerObject;
        PlayFireClientRpc(playerNetworkObject);
    }

    [ClientRpc]
    private void PlayFireClientRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
        
        playerNetworkObject.GetComponent<GunController>().PlayFire();
    }
    
    private void PlayFire()
    {
        gunFireAudio.Play();
    }
}
*/