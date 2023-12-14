using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ParticleController : NetworkBehaviour
{
    [SerializeField] private ParticleSystem slowingDownParticles;
    [SerializeField] private float lowerSpeedMultiplier;
    [SerializeField] private float upperSpeedMultiplier;
    [SerializeField] private float emissionMultiplier;
    [SerializeField] private float climbRate;
    
    public void SetSlowingDownParticles(Vector3 direction, float speed)
    {
        SetSlowingDownParticlesServerRpc(direction, speed);
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void SetSlowingDownParticlesServerRpc(Vector3 direction, float speed)
    {
        SetSlowingDownParticlesClientRpc(direction, speed);
    }
    
    [ClientRpc]
    private void SetSlowingDownParticlesClientRpc(Vector3 direction, float speed)
    {
        /*// Getting player that got hit
        playerStatsNetworkObjectReference.TryGet(out NetworkObject playerStatsNetworkObject);
        PlayerStats playerStats = playerStatsNetworkObject.GetComponent<PlayerStats>();*/

        if(!slowingDownParticles.isEmitting)
            slowingDownParticles.Play();
        
        slowingDownParticles.transform.rotation = Quaternion.LookRotation(direction);
        var emission = slowingDownParticles.emission;
        emission.rateOverTime =  emissionMultiplier * (1 - 1 / (climbRate * speed + 1));

        var main = slowingDownParticles.main;
        main.startSpeed = new ParticleSystem.MinMaxCurve(speed * lowerSpeedMultiplier, speed * upperSpeedMultiplier);
    }
    
    public void StopSlowingDownParticles()
    {
        StopSlowingDownParticlesServerRpc();
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void StopSlowingDownParticlesServerRpc()
    {
        StopSlowingDownParticlesClientRpc();
    }
    
    [ClientRpc]
    private void StopSlowingDownParticlesClientRpc()
    {
        if(slowingDownParticles.isEmitting)
            slowingDownParticles.Stop();
    }
}
