using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static GlobalItemDictionary;

public class UseableItem : MonoBehaviour
{
    #region Variables

    [SerializeField] private ItemID baseItemID;
    [SerializeField] private UniqueItemModel uniqueItemModel;

    // Info from inventory
    private UniqueItemID uniqueItemID;
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

    #region Unique Item Methods

    public void SetUniqueItemID(StoredItemID storedItemID)
    {
        uniqueItemID = storedItemID.UniqueItemID;
        inventoryKey = storedItemID.InventoryKey;
        itemKey = storedItemID.ItemKey;
    }

    protected void ModifyUniqueItemID(StoredItemID modificationSIID, int modificationSlot)
    {
        UniqueItemID modificationUIID = modificationSIID.UniqueItemID;

        if(!uniqueItemID.TryModifyItem(modificationUIID, modificationSlot, out UniqueItemID oldModID)) // Edit Copy in hands
            return;

        BuildModel();
        UpdateUniqueItemID();

        Inventory.Instance.RemoveItem(modificationSIID.InventoryKey, modificationSIID.ItemKey); // Retrieve old mod from inventory

        Debug.Log("Adding " + oldModID.BaseItemID + " to inventory");
        Inventory.Instance.AddItem(oldModID);
    }

    protected void AddToCounter(ItemID itemType, int count)
    {
        uniqueItemID.TryAddItemToCounter(itemType, count); // Edit copy in hands

        UpdateUniqueItemID();
    }

    // Updates the copy of the item in the local players inventory
    protected void UpdateUniqueItemID()
    {
        if (isOwner)
            Inventory.Instance.UpdateUniqueItem(inventoryKey, itemKey, uniqueItemID); // Update copy in inventory
    
    }

    public void BuildModel() 
    {
        if (uniqueItemID.BaseItemID != ItemID.Empty)
            uniqueItemModel.BuildModel(uniqueItemID);
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

    protected void AlertEnemiesInRangeOfSound(float soundRange)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, soundRange, LayerMask.GetMask("Enemy NPC"));

        foreach (Collider collider in colliders)
        {
            NavMeshEnemyNPC enemy = collider.GetComponent<NavMeshEnemyNPC>();
            if (enemy != null)
            {
                enemy.OnSoundHeard();
            }
        }
    }
}
