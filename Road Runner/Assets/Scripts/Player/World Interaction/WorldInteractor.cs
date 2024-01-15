using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Moving party of old inventory script that did world interaction here
public class WorldInteractor : MonoBehaviour, ILocalOnlyBehavior
{
    [Header("World Interaction")]
    [SerializeField] private float maxItemPickupDistance;
    [SerializeField] private LayerMask isItemPickup;
    [SerializeField] private LayerMask isVehicle;

    private Transform mainCamera;

    private VehicleInteractionController vehicle; // LAMO imma have to come back to vehicles big time

    public void Initialize()
    {
        mainCamera = Camera.main.transform;
    }

    // Inputs come in from PlayerInput through here
    public void OnItemPickUpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RaycastForPickups();
        }
    }

    private void RaycastForPickups()
    {
        Ray ray = new Ray(mainCamera.position, mainCamera.forward);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit, maxItemPickupDistance, isItemPickup))
        {
            ItemPickup itemPickup = raycastHit.transform.GetComponent<ItemPickup>();
            Inventory.Instance.TryPickUpItem(itemPickup);
        }
    }
}
