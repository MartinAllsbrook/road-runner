using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class UseableItem : MonoBehaviour
{
    protected bool isOwner; // Used to determine if the local player owns this item
    public bool IsOwner // Property used by UseableItemController to set isOwner
    { 
        private get { return isOwner; } 
        set { isOwner = value; }
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
