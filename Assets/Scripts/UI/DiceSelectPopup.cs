using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceSelectPopup : PopupObject
{
    public DiceButtonController[] arrDiceButton = new DiceButtonController[DEFS.DICE_MAX];
    public Button buttonNext = null;
    public Button buttonReroll = null;
    
    public bool[] arrSelected { get; private set; } = new bool[DEFS.DICE_MAX];

    private GameController gameController = null;

    private void Awake() {
        gameController = GameController.getInstance();

        for(int i = 0; i < DEFS.DICE_MAX; i++) {
            arrDiceButton[i].SetFace(1);
            arrDiceButton[i].EnableOutline(true);
            Button button = arrDiceButton[i].GetComponent<Button>();
            if(button != null) {
                int param = i;
                button.onClick.AddListener(delegate { ClickDiceButton(param); });
            }
            arrSelected[i] = true;
        }
        UpdateButton();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ClickDiceButton(int index) {
        if(index < 0 || DEFS.DICE_MAX <= index) {
            return;
        }

        bool newSelected = !arrSelected[index];
        arrSelected[index] = newSelected;
        arrDiceButton[index].EnableOutline(newSelected);

        UpdateButton();
    }

    public override void Refresh() {
        if(gameController == null) {
            return;
        }

        for(int i = 0; i < DEFS.DICE_MAX; i++) {
            int number = gameController.arrDiceNumber[i];
            arrDiceButton[i].SetFace(number);
            arrDiceButton[i].EnableOutline(true);
            arrSelected[i] = true;
        }
        UpdateButton();
    }

    public void ClickReroll() {
        gameController.SetupReroll(arrSelected);

        CloseSelf();
    }

    public void ClickNext() {        
        gameController.SetupHandMatch();

        CloseSelf();
    }

    private void UpdateButton() {
        if(buttonNext == null || buttonReroll == null) {
            return;
        }

        bool isSelectedAll = true;
        for(int i = 0; i < arrSelected.Length; i++) {
            if(arrSelected[i] == false) {
                isSelectedAll = false;
                break;
            }
        }

        if(isSelectedAll) {
            buttonNext.gameObject.SetActive(true);
            buttonReroll.gameObject.SetActive(false);
        }
        else {
            buttonNext.gameObject.SetActive(false);
            buttonReroll.gameObject.SetActive(true);
        }
    }
}
