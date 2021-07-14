using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeController : MonoBehaviour
{
    public Text noticeText = null;

    private string message = "";
    private int dotCount = 1;
    private float elapsedTIme = 0;

    void Update()
    {
        elapsedTIme += Time.deltaTime;
        if(1f <= elapsedTIme) {
            elapsedTIme = elapsedTIme % 1f;
            dotCount++;
            if(3 < dotCount) {
                dotCount = 1;
            }
            string text = message;
            for(int i = 0; i < dotCount; i++) {
                text += ".";
            }
            noticeText.text = text;
        }
    }

    public void SetMessage(string _message) {
        message = _message;
    }
}
