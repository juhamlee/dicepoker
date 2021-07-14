using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    private BackgroundVFXController backgroundVFX = null;
    private TouchBlocker touchBlocker = null;
    private List<PopupObject> listPopupObject = new List<PopupObject>();

    private static PopupManager popupManager = null;
    static public PopupManager getInstance() {
        if(popupManager == null) {
            GameObject gameObject = GameObject.FindWithTag("PopupManager");    
            if(gameObject != null) {
                popupManager = gameObject.GetComponent<PopupManager>();
            }
        }
        return popupManager;
    }
    
    private void Awake() {
        GameObject gameObject = GameObject.FindWithTag("BackgroundVFX");
        if(gameObject != null) {
            backgroundVFX = gameObject.GetComponent<BackgroundVFXController>();
        }
        touchBlocker = TouchBlocker.getInstance();
    }

    public void OpenPopup(PopupObject popupObject) {
        if(popupObject == null) return;

        if(popupObject.popupType == PopupObject.TYPE.SLIDE) {
            OpenSlide((SlideObject)popupObject);
        }
        else {
            for(int i = 0; i < listPopupObject.Count; i++) {
                if(listPopupObject[i].popupType == PopupObject.TYPE.POPUP) {
                    listPopupObject[i].gameObject.SetActive(false);
                }
            }
            listPopupObject.Add(popupObject);

            if(listPopupObject.Count == 1) {
                StartCoroutine("OpenRecentPopup", backgroundVFX.fadeTime);
                touchBlocker.BlockTouchForSeconds(backgroundVFX.fadeTime);
            }
            else {
                popupObject.Open();
            }
        }

        if(listPopupObject.Count == 1) {
            if(backgroundVFX != null) {
                backgroundVFX.EnableEffect(true);
            }
        }
    }

    public void ClosePopup(PopupObject popupObject) {
        if(listPopupObject.Count == 0 || popupObject == null) {
            return;
        }

        if(popupObject.popupType == PopupObject.TYPE.SLIDE) {
            CloseSlide((SlideObject)popupObject);
        }
        else {
            listPopupObject.Remove(popupObject);
            popupObject.Close();

            if(0 < listPopupObject.Count) {
                listPopupObject[listPopupObject.Count - 1].gameObject.SetActive(true);
            }
        }

        if(listPopupObject.Count == 0) {
            if(backgroundVFX != null) {
                backgroundVFX.EnableEffect(false);
            }
        }
    }

    private void OpenSlide(SlideObject slideObject) {
        listPopupObject.Add(slideObject);
        slideObject.Open();

        if(touchBlocker != null) {
            touchBlocker.BlockTouchForSeconds(slideObject.slideDelay);
        }
    }

    private void CloseSlide(SlideObject slideObject) {
        float delay = slideObject.slideDelay;
        listPopupObject.Remove(slideObject);
        slideObject.Close();

        if(touchBlocker != null) {
            touchBlocker.BlockTouchForSeconds(slideObject.slideDelay);
        }
    }

    IEnumerator OpenRecentPopup(float delay) {
        yield return new WaitForSeconds(delay);

        if(0 < listPopupObject.Count) {
            listPopupObject[listPopupObject.Count - 1].Open();
        }
    }
}
