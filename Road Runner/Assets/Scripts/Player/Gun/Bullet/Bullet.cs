using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxTimeActive;
    
    private Rigidbody _rigidbody;

    private float timeActive;
    private float damage = 20;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = false;
    }

    private void Update()
    {
        timeActive += Time.deltaTime;

        if (timeActive >= maxTimeActive)
        {
            gameObject.SetActive(false);
        }
    }

    public void FireBullet(Vector3 velocity, float dmg)
    {
        timeActive = 0;
        
        _rigidbody.velocity = velocity;
        damage = dmg;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Bullet with owner ID " + ownerId + " hit something");
        if (collision.transform.CompareTag("Player"))
        {
            PlayerStats.Instance.PlayHitMarker();
            // Debug.Log("Local Bullet Hit");
            PlayerStats playerStats= collision.transform.GetComponent<PlayerStats>();
            
            BulletNetworkManager.Instance.BulletHitPlayer(playerStats.NetworkObject, damage);
        }
        else
        {
            ContactPoint contactPoint = collision.GetContact(0);
            
            BulletNetworkManager.Instance.BulletHitEnvironment(contactPoint.point, contactPoint.normal);
        }
        
        gameObject.SetActive(false);
    }


}
