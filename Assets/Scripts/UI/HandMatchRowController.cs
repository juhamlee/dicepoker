using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandMatchRowController : MonoBehaviour
{
    public Text nameText = null;
    public Text scoreText = null;
    public Image checkImage = null;
    public GameObject selecetObject = null;
    public Button button = null;

    private void Awake() {

    }

    public void SetSelected(bool isSelected) {
        if(isSelected) {
            selecetObject.SetActive(true);
        }
        else {
            selecetObject.SetActive(false);
        }
    }

    public void SetData(string name, int score, bool isComplete) {
        if(nameText != null) {
            nameText.text = name;
        }
        if(scoreText != null) {
            if(isComplete) {
                scoreText.text = score.ToString();
            }
            else {
                scoreText.text = "+" + score.ToString();
            }
        }
        if(checkImage != null) {
            if(isComplete) {
                checkImage.gameObject.SetActive(true);
            }
            else {
                checkImage.gameObject.SetActive(false);
            }
        }
    }

    public void SetComplete() {
        checkImage.gameObject.SetActive(true);
        StartCoroutine("CompleteCoroutine", 0.2f);
    }

    IEnumerator CompleteCoroutine(float targetTime) {
        float elapsedTime = 0f;
        float currentTime = Time.time;
        float recentTime = currentTime;
        float degree = 0f;

        while(degree < 1f) {
            currentTime = Time.time;
            elapsedTime += currentTime - recentTime;
            recentTime = currentTime;

            degree = elapsedTime / targetTime;

            float alpha = Mathf.Lerp(0, 1, degree);
            float scale = Mathf.Lerp(2, 1, degree);

            checkImage.color = new Color(1, 1, 1, alpha);
            checkImage.transform.localScale = Vector3.one * scale;

            yield return null;
        }
    }
}
