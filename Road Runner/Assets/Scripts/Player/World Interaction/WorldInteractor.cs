using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Moving party of old inventory script that did world interaction here
public class WorldInteractor : MonoBehaviour, ILocalOnlyBehavior
{
    [Header("World Interaction")]
    [SerializeField] private float maxItemPickupDistance;
    [SerializeField] private LayerMask isInteractable;

    private Transform mainCamera;

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

        if (Physics.Raycast(ray, out raycastHit, maxItemPickupDistance, isInteractable))
        {
            Debug.Log("Raycast Hit: " + raycastHit.transform.name);
            if(raycastHit.transform.CompareTag("Item Pickup"))
            {
                ItemPickup itemPickup = raycastHit.transform.GetComponent<ItemPickup>();
                Inventory.Instance.TryPickUpItem(itemPickup);
            }
            
            if (raycastHit.transform.CompareTag("Interactive Scatter"))
            {
                InteractiveScatter interactiveScatter = raycastHit.transform.GetComponent<InteractiveScatter>();
                interactiveScatter.Interact();
            }
        }
    }
}
