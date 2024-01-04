using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class UseableItem : MonoBehaviour
{
    #region Variables

    [SerializeField] private Inventory.ItemID baseItemID;

    // Info from inventory
    protected UniqueItemID uniqueItemID;
    protected int inventoryKey;
    protected int itemKey;

    [SerializeField] protected InspectPoint[] inspectPoints;
 
    protected UseableItemController parentItemController;
 
    protected bool isOwner; // Used to determine if the local player owns this item

    #endregion

    #region Properties

    public UniqueItemID UniqueItemID
    {
        get { return uniqueItemID; }
    }

    public InspectPoint[] InspectPoints
    {
        get { return inspectPoints; }
    }

    public UseableItemController ParentItemController
    {
        set { parentItemController = value; }
    }

    public bool IsOwner // Property used by UseableItemController to set isOwner
    {
        set { isOwner = value; }
    }

    #endregion

    public void SetUniqueItemID(StoredItemID storedItemID)
    {
        uniqueItemID = storedItemID.UniqueItemID;
        inventoryKey = storedItemID.InventoryKey;
        itemKey = storedItemID.ItemKey;
    }

    #region Virtual On Input Methods
    public virtual void OnUseItemInput()
    {

    }

    public virtual void OnReloadItemInput()
    {

    }

    public virtual void OnSeccondaryUseItemInput(InputAction.CallbackContext context)
    {

    }

    #endregion

    #region Virtual Server Action Methods

    public virtual void UseServerAction()
    {

    }

    public virtual void SeccondaryUseServerAction()
    {

    }

    public virtual void ReloadServerAction()
    {

    }

    #endregion
}
