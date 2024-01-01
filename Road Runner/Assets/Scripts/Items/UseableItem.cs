using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class UseableItem : MonoBehaviour
{
    #region Variables

    [SerializeField] protected UniqueItemID uniqueItemID;

    [SerializeField] protected InspectPoint[] inspectPoints;
 
    protected UseableItemController parentItemController;
 
    protected bool isOwner; // Used to determine if the local player owns this item
    protected int containedItemKey; // Key != ID (this refers to the key of the item in the inventory)

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

    public int ContainedItemKey
    {
        get { return containedItemKey; }
        set { containedItemKey = value; }
    }

    #endregion

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
