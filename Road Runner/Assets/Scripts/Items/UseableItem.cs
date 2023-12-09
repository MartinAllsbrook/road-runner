using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseableItem : MonoBehaviour
{
    [SerializeField] protected AudioSource useAudio;
    [SerializeField] protected bool isUseAudioLocal = true;
    [SerializeField] protected AudioSource seccondaryUseAudio;
    [SerializeField] protected bool isSeccondaryUseAudioLocal = true;
    [SerializeField] protected AudioSource reloadAudio;
    [SerializeField] protected bool isReloadAudioLocal = true;

    protected bool isOwner;
    public bool IsOwner { 
        private get { return isOwner; } 
        set { isOwner = value; }
    }

    protected void EquipItem()
    {

    }

    public virtual void UseItem()
    {

    }

    public virtual void ReloadItem()
    {

    }

    public virtual void SeccondaryUseItem()
    {

    }

    public virtual void UseServerAction()
    {
        useAudio.Play();
    }

    public void SeccondaryUseServerAction()
    {
        seccondaryUseAudio.Play();
    }

    public void ReloadServerAction()
    {
        reloadAudio.Play();
    }
}
