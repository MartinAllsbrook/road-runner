using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxTimeActive;

    private float _minDistanceForCrack = 250f;
    
    private Rigidbody _rigidbody;

    private float _timeActive = 0f;
    private float _estimatedDistanceTraveled = 0f;

    private float _damage = 20;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = false;
    }

    private void Update()
    {
        _timeActive += Time.deltaTime;
        _estimatedDistanceTraveled += speed * Time.deltaTime;

        if (_timeActive >= maxTimeActive)
        {
            gameObject.SetActive(false);
        }
    }

    public void FireBullet(Vector3 velocity, float damage)
    {
        _timeActive = 0;
        _estimatedDistanceTraveled = 0;
        
        _rigidbody.velocity = velocity;
        _damage = damage;
    }

    /// <summary>
    /// When bullet hits something check if it was a player or environment and react accordingly
    /// </summary>
    /// <param name="collision">Default Collision parameter</param>
    private void OnCollisionEnter(Collision collision)
    {
        // Debug.Log("Bullet with owner ID " + ownerId + " hit something");
        if (collision.transform.CompareTag("Player"))
        {
            PlayerStats.Instance.PlayHitMarker();
            // Debug.Log("Local Bullet Hit");
            PlayerStats playerStats= collision.transform.GetComponent<PlayerStats>();
            
            BulletNetworkManager.Instance.BulletHitPlayer(playerStats.NetworkObject, _damage);
        }
        else
        {
            ContactPoint contactPoint = collision.GetContact(0);
            
            BulletNetworkManager.Instance.BulletHitEnvironment(contactPoint.point, contactPoint.normal);
        }
        
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Distance Traveled: " + _estimatedDistanceTraveled + " VS Min: " + _minDistanceForCrack);

        if (_estimatedDistanceTraveled < _minDistanceForCrack)
            return;

        if (other.CompareTag("Bullet Crack")) // TODO: Make this a layer?
        {
            Debug.Log("Crack Success");
            BulletNetworkManager.Instance.SpawnBulletCrack(transform.position);
        }
    }
}
