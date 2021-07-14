using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BackgroundVFXController : MonoBehaviour
{
    public Image backgroundImage = null;
    public Volume volume = null;
    [Range(0, 255)]
    public int backgroundAlphaMax = 50;
    public float blurDistanceMin = 0.1f;
    public float blurDistanceMax = 4f;
    public float fadeTime = 0.5f;

    private DepthOfField depthOfField = null;
    private Coroutine recentCoroutine = null;
    private bool isEnable = false;

    private void Awake() {
        if(volume != null) {
            volume.profile.TryGet<DepthOfField>(out depthOfField);
        }

        if(depthOfField != null) {
            depthOfField.focusDistance.value = blurDistanceMax;
        }
        if(backgroundImage != null) {
            backgroundImage.color = new Color(0, 0, 0, 0);
        }
    }

    private void Update() {
    }

    public void EnableEffect(bool enable) {
        if(isEnable == enable) {
            return;
        }
        isEnable = enable;
        
        if(enable) {
            if(recentCoroutine != null) {
                StopCoroutine(recentCoroutine);
            }
            recentCoroutine = StartCoroutine("FadeIn", fadeTime);
        }
        else {
            if(recentCoroutine != null) {
                StopCoroutine(recentCoroutine);
            }
            recentCoroutine = StartCoroutine("FadeOut", fadeTime);
        }
    }

    IEnumerator FadeIn(float targetTime) {
        float elapsedTime = 0f;
        float currentTime = Time.time;
        float recentTime = currentTime;
        float degree = 0f;

        while(degree < 1f) {
            currentTime = Time.time;
            elapsedTime += currentTime - recentTime;
            recentTime = currentTime;

            degree = elapsedTime / targetTime;

            FadeByDegree(degree);

            yield return null;
        }
        FadeByDegree(1f);
    }

    IEnumerator FadeOut(float targetTime) {
        float elapsedTime = 0f;
        float currentTime = Time.time;
        float recentTime = currentTime;
        float degree = 0f;

        while(degree < 1f) {
            currentTime = Time.time;
            elapsedTime += currentTime - recentTime;
            recentTime = currentTime;

            degree = elapsedTime / targetTime;

            FadeByDegree(1f - degree);

            yield return null;
        }
        FadeByDegree(0f);
    }

    void FadeByDegree(float degree) {
        if(backgroundImage != null) {
            Color c = backgroundImage.color;
            c.a = Mathf.Lerp(0, (float)backgroundAlphaMax/255f, degree);
            backgroundImage.color = c;
        }
        if(depthOfField != null) {
            depthOfField.focusDistance.value = Mathf.Lerp(blurDistanceMax, blurDistanceMin, degree);
        }
    }
}
