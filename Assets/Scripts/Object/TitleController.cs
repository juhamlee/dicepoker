using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    public BackgroundVFXController backgroundVFX = null;
    public GameObject insertNamePopup = null;
    public Transform diceTransform = null;

    void Update()
    {
        diceTransform.Rotate(Vector3.up, 45f * Time.deltaTime);
    }

    public void ClickPlay() {
        backgroundVFX.EnableEffect(true);
        StartCoroutine("DelayedOpenPopup", backgroundVFX.fadeTime);
    }

    IEnumerator DelayedOpenPopup(float delay) {
        yield return new WaitForSeconds(delay);

        insertNamePopup.SetActive(true);
    }
}
