using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceIconController : MonoBehaviour
{
    public Image faceImage = null;
    private static Sprite[] arrFaceSprite = null;

    private void Awake() {
        arrFaceSprite = Resources.LoadAll<Sprite>("DiceFace");
    }
    public void SetFace(int number) {
        if(arrFaceSprite == null) {
            return;
        }

        if(number <= 0 || 6 < number) {
            faceImage.gameObject.SetActive(false);
        }
        else {
            int index = number - 1;
            faceImage.sprite = arrFaceSprite[index];
            faceImage.gameObject.SetActive(true);
        }
    }

    public void SetSize(float width, float height) {
        if(TryGetComponent<RectTransform>(out RectTransform rect)) {
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }
    }
}
