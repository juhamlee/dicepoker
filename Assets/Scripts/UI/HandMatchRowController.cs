using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandMatchRowController : MonoBehaviour
{
    public DiceIconController prefabDiceIcon = null;
    public HorizontalLayoutGroup diceContainer = null;
    public Text nameText = null;
    public Text scoreText = null;
    public GameObject coverObject = null;
    public GameObject selecetObject = null;
    public Button button = null;

    private DiceIconController[] arrDiceIcon = null;

    private void Awake() {
        arrDiceIcon = new DiceIconController[DEFS.DICE_MAX];
        if(prefabDiceIcon != null && diceContainer != null) {
            for(int i = 0; i < DEFS.DICE_MAX; i++) {
                arrDiceIcon[i] = Instantiate<DiceIconController>(prefabDiceIcon, diceContainer.transform);
                arrDiceIcon[i].name = "Dice Icon" + (i + 1).ToString();
            }
        }
    }

    public void SetSelected(bool isSelected) {
        if(isSelected) {
            selecetObject.SetActive(true);
        }
        else {
            selecetObject.SetActive(false);
        }
    }

    public void SetData(string name, int[] arrNumber, int score, bool isComplete) {
        for(int i = 0; i < DEFS.DICE_MAX; i++) {
            SetNumber(i, arrNumber[i]);
        }
        if(nameText != null) {
            nameText.text = name;
        }
        if(scoreText != null) {
            scoreText.text = "+" + score.ToString();
        }
        if(coverObject != null) {
            coverObject.SetActive(!isComplete);
        }
    }

    public void SetNumber(int index, int number) {
        if(index < 0 || DEFS.DICE_MAX <= index) {
            return;
        }
        arrDiceIcon[index].SetFace(number);
    }

    public void SetOpacity(int index, float opacity) {
        if(index < 0 || DEFS.DICE_MAX <= index) {
            return;
        }
        arrDiceIcon[index].SetOpacity(opacity);
    }
}
