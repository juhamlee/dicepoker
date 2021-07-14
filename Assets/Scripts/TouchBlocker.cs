using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchBlocker : MonoBehaviour
{
    public GameObject dummy = null;

    private bool isBlocked = false;

    private static TouchBlocker touchBlocker = null;
    static public TouchBlocker getInstance() {
        if(touchBlocker == null) {
            GameObject gameObject = GameObject.FindWithTag("TouchBlocker");    
            if(gameObject != null) {
                touchBlocker = gameObject.GetComponent<TouchBlocker>();
            }
        }
        return touchBlocker;
    }

    public void BlockTouch() {
        if(isBlocked == true) {
            return;
        }
        dummy.SetActive(true);
        isBlocked = true;
    }

    public void UnblockTouch() {
        if(isBlocked == false) {
            return;
        }
        dummy.SetActive(false);
        isBlocked = false;
    }

    public void BlockTouchForSeconds(float seconds) {
        BlockTouch();
        StartCoroutine("UnblockTouchInSeconds", seconds);
    }

    IEnumerator UnblockTouchInSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);
        UnblockTouch();
    }
}
