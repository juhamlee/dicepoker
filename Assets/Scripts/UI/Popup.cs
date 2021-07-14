using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupObject : MonoBehaviour
{
    public enum TYPE {
        POPUP = 0,
        SLIDE,
    }

    public TYPE popupType { get; protected set; } = TYPE.POPUP;
    
    protected static PopupManager popupManager = null;

    public virtual void Open() {
        popupManager = PopupManager.getInstance();
        
        gameObject.SetActive(true);
        Refresh();
    }

    public virtual void Close() {
        gameObject.SetActive(false);
    }

    public virtual void Refresh() {
    }

    public void CloseSelf() {
        if(popupManager != null) {
            popupManager.ClosePopup(this);
        }
    }
}