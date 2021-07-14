using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideObject : PopupObject
{
    public float slideDelay { get; protected set; } = 0.5f;
    public float slideMinX { get; protected set; } = -950f;
    public float slideMaxX { get; protected set; } = 0f;
    public bool isOpen { get; protected set; } = false;
    private Coroutine recentSlideCoroutine = null;

    protected virtual void Awake() {
        popupType = PopupObject.TYPE.SLIDE;
    }

     public override void Open() {
        if(isOpen) {
            return;
        }

        if(recentSlideCoroutine != null) {
            StopCoroutine(recentSlideCoroutine);
        }
        recentSlideCoroutine = StartCoroutine("SlideOpen", slideDelay);

        isOpen = true;

        base.Open();
    }

    public override void Close() {
        if(isOpen == false) return;
        else isOpen = false;

        if(recentSlideCoroutine != null) {
            StopCoroutine(recentSlideCoroutine);
        }
        recentSlideCoroutine = StartCoroutine("SlideClose", slideDelay);
    }

    IEnumerator SlideOpen(float delay) {
        float elapsedTime = 0f;
        float currentTime = Time.time;
        float recentTime = currentTime;
        float degree = 0f;

        while(degree < 1f) {
            currentTime = Time.time;
            elapsedTime += currentTime - recentTime;
            recentTime = currentTime;

            degree = elapsedTime / delay;

            SlideByDegree(degree);

            yield return null;
        }
    }

    IEnumerator SlideClose(float delay) {
        float elapsedTime = 0f;
        float currentTime = Time.time;
        float recentTime = currentTime;
        float degree = 0f;

        while(degree < 1f) {
            currentTime = Time.time;
            elapsedTime += currentTime - recentTime;
            recentTime = currentTime;

            degree = elapsedTime / delay;

            SlideByDegree(1f - degree);

            yield return null;
        }
    }

    private void SlideByDegree(float degree) {
        float x = Mathf.Lerp(slideMinX, slideMaxX, degree);
        transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
    }
}
