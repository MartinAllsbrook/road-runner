using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class VehicleInteractionController : NetworkBehaviour
{
    private VehicleController vehicleController;

    private bool rideVehicle;

    private Transform[] riderTransforms;
    [SerializeField] private Transform[] riderPositions;

    [SerializeField] private LayerMask vehicleLayerMask;

    private Inventory vehicleInventory;


    private void Awake()
    {
        vehicleController = GetComponent<VehicleController>();
        riderTransforms = new Transform[riderPositions.Length];
        vehicleInventory = GetComponent<Inventory>();
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    

    }

    private void Update()
    {
        UpdateRiderPositions();
    }


    private void UpdateRiderPositions()
    {
        for (int i = 0; i < riderTransforms.Length; i++)
        {
            if (riderTransforms[i] != null)
            {
                riderTransforms[i].position = riderPositions[i].position;
                riderTransforms[i].rotation = riderPositions[i].rotation;
            }
        }
    }

    public void EnterVehicle(NetworkObject riderNetworkObject)
    {
        rideVehicle = true;
        vehicleController.SetBeingDriven(true);

        Debug.Log(riderNetworkObject);
        AddRiderServerRpc(riderNetworkObject);
    }

    [ServerRpc (RequireOwnership = false)]
    private void AddRiderServerRpc(NetworkObjectReference riderNetworkObjectReference)
    {
        AddRiderClientRpc(riderNetworkObjectReference);
    }

    [ClientRpc]
    private void AddRiderClientRpc(NetworkObjectReference riderNetworkObjectReference)
    {
        riderNetworkObjectReference.TryGet(out NetworkObject riderNetworkObject);
        Transform riderTransform = riderNetworkObject.transform;

        if (riderTransform == null)
            return;

        for (int i = 0; i < riderTransforms.Length; i++)
        {
            if (riderTransforms[i] == null)
            {
                AddRiderToPosition(i, riderTransform, riderNetworkObject);
                return;
            }
        }
    }

    private void AddRiderToPosition(int position, Transform riderTransform, NetworkObject riderNetworkObject)
    {
        riderNetworkObject.GetComponent<Rigidbody>().useGravity = false;
        riderNetworkObject.GetComponent<CapsuleCollider>().excludeLayers += vehicleLayerMask;
        riderNetworkObject.GetComponent<PlayerMovement>().enabled = false;

        //Debug.Log("Position " + i + " is empty");
        riderTransforms[position] = riderTransform;

        if (position == 0)
        {
            ulong clientId = riderNetworkObject.OwnerClientId;
            if (clientId != OwnerClientId)
                GetComponent<NetworkObject>().ChangeOwnership(clientId);
        }
    }
    



    public void ExitVehicle(NetworkObject riderNetworkObject)
    {
        rideVehicle = false;
        
        vehicleController.ResetWheels();
        vehicleController.SetBeingDriven(false);

        // player passes in their NO
        Debug.Log(riderNetworkObject);
        RemoveRiderServerRpc(riderNetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveRiderServerRpc(NetworkObjectReference riderNetworkObjectReference)
    {
        RemoveRiderClientRpc(riderNetworkObjectReference);
    }


    [ClientRpc]
    private void RemoveRiderClientRpc(NetworkObjectReference riderNetworkObjectReference)
    {
        riderNetworkObjectReference.TryGet(out NetworkObject riderNetworkObject);
        Transform riderTransform = riderNetworkObject.transform;

        riderTransform.position = transform.position + transform.right * -3;

        riderNetworkObject.GetComponent<Rigidbody>().useGravity = true;
        riderNetworkObject.GetComponent<CapsuleCollider>().excludeLayers -= vehicleLayerMask;
        riderNetworkObject.GetComponent<PlayerMovement>().enabled = true;

        if (riderTransform != null)
        {
            for (int i = 0; i < riderTransforms.Length; i++)
            {
                if (riderTransforms[i] == riderTransform)
                    riderTransforms[i] = null;
            }
        }  
    }

    public Inventory GetInvetory()
    {
        return vehicleInventory;
    }
}
