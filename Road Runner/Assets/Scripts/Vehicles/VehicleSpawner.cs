using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VehicleSpawner : NetworkBehaviour
{
    [SerializeField] private Transform[] vehicles;
    [SerializeField] private float spawnTime;

    [SerializeField] private int worldSize;
    [SerializeField] private int worldPadding;

    private int startArea;
    private int endArea;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            Debug.LogWarning("Disableing Vehicle Spawner on Client");
            enabled = false;
            return;
        }

        TerrainManager.onTerrainGenerated.AddListener(() =>
        {
            startArea = worldPadding;
            endArea = worldSize - worldPadding;

            StartCoroutine(SpawnVehiclesRoutine());
        });
    }

    private IEnumerator SpawnVehiclesRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnTime);


            float x = Random.Range(startArea, endArea);
            float z = Random.Range(startArea, endArea);

            Vector3 spawnPos = new Vector3(x, 100, z);

            SpawnVehicle(spawnPos);
        }
    }

    // if we are the server, we can skip the server rpc and just spawn the thing
    public void SpawnVehicle(Vector3 position)
    {
        Transform vehicle = vehicles[Random.Range(0, vehicles.Length)];

        Transform itemGameObject = Instantiate(vehicle, position, new Quaternion(0, 0, 0, 0));

        NetworkObject itemNetworkObject = itemGameObject.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);

        //SpawnVehicleServerRpc(position);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnVehicleServerRpc(Vector3 position)
    {
        Transform vehicle = vehicles[Random.Range(0, vehicles.Length)];

        Transform itemGameObject = Instantiate(vehicle, position, new Quaternion(0, 0, 0, 0));

        NetworkObject itemNetworkObject = itemGameObject.GetComponent<NetworkObject>();
        itemNetworkObject.Spawn(true);
    }
}
