using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandMatchPopup : PopupObject
{
    public struct Data {
        public int expectScore;
        public bool isComplete;
    }

    public Text progressText = null;
    public Text scoreText = null;
    public Text scoreSelectedText = null;
    public VerticalLayoutGroup rowContainer = null;
    public HorizontalLayoutGroup handContainer = null;
    public DiceIconController prefabDiceIcon = null;
    public HandMatchRowController prefabRow = null;

    private DiceIconController[] arrHandDiceIcon = null;
    private HandMatchRowController[] arrRow = null;
    private RectTransform rowContainerRect = null;
    private int currentScore = 0;
    private int selectedScore = 0;
    private int currentProgress = 0;
    private int maxProgress = 0;
    private int selectedIndex = -1;
    private float selectedScale = 1f;
    private float unselectedScale = 1f;
    private Data[] arrData = null;

    private GameController gameController = null;
    private PlayerManager playerManager = null;
    private TouchBlocker touchBlocker = null;

    private void Awake() {
        gameController = GameController.getInstance();
        playerManager = PlayerManager.getInstance();
        touchBlocker = TouchBlocker.getInstance();

        arrHandDiceIcon = new DiceIconController[DEFS.DICE_MAX];
        for(int i = 0; i < DEFS.DICE_MAX; i++) {
            arrHandDiceIcon[i] = Instantiate<DiceIconController>(prefabDiceIcon, handContainer.transform);
            arrHandDiceIcon[i].name = "Hand Dice Icon" + (i + 1).ToString();
            arrHandDiceIcon[i].SetSize(150f, 150f);
        }

        int ruleSize = RuleBook.getInstance.GetRuleSize();
        arrRow = new HandMatchRowController[ruleSize];
        for(int i = 0; i < ruleSize; i++) {
            arrRow[i] = Instantiate<HandMatchRowController>(prefabRow, rowContainer.transform);
            arrRow[i].name = "Match Row" + (i + 1).ToString();
            if(arrRow[i].button != null) {
                int param = i;
                arrRow[i].button.onClick.AddListener(delegate { ClickSelect(param); });
            }
        }
        arrData = new Data[ruleSize];

        rowContainer.TryGetComponent<RectTransform>(out rowContainerRect);
        prefabRow.TryGetComponent<RectTransform>(out RectTransform rowRect);
        if(rowContainerRect && rowRect) {
            float containerHeight = rowContainerRect.sizeDelta.y;
            float rowHeight = rowRect.sizeDelta.y;
            float paddingTop = rowContainer.padding.top;
            float paddingBottom = rowContainer.padding.bottom;
            float spacing = rowContainer.spacing;

            float containerSpace = containerHeight - paddingTop - paddingBottom - spacing * (ruleSize - 1) - rowHeight;
            unselectedScale = containerSpace / ((ruleSize - 1) * rowHeight);
        }
    }

    public void ClickSelect(int index) {
        if(selectedIndex == index && arrData[index].isComplete == false) {
            ApplySelect();
        }
        else {
            selectedIndex = index;

            UpdateRow();
            UpdateText();
        }
    }

    private void ApplySelect() {
        PlayerData currentPlayer = playerManager.GetCurrentPlayerData();
        if(currentPlayer != null) {
            currentPlayer.UpdateScoreData(selectedIndex, gameController.arrDiceNumber, selectedScore, true);
        }
        StartCoroutine("ApplyCoroutine", 1.5f);
    }

    public override void Refresh() {
        if(gameController == null || playerManager == null) {
            return;
        }

        maxProgress = RuleBook.getInstance.GetRuleSize();
        currentProgress = 0;
        currentScore = 0;

        PlayerData currentPlayer = playerManager.GetCurrentPlayerData();
        for(int i = 0; i < arrRow.Length; i++) {
            ScoreData scoreData = currentPlayer.arrScoreData[i];
            if(scoreData.isComplete) {
                arrData[i].expectScore = 0;
                arrData[i].isComplete = true;
                arrRow[i].SetData(scoreData.name, scoreData.arrHand, scoreData.score, true);

                currentProgress++;
                currentScore += scoreData.score;
            }
            else {
                int expectScore = RuleBook.getInstance.GetScoreFromHand(i, gameController.arrDiceNumber);
                arrData[i].expectScore = expectScore;
                arrData[i].isComplete = false;
                arrRow[i].SetData(scoreData.name, scoreData.arrHand, expectScore, false);
            }
        }

        for(int i = 0; i < DEFS.DICE_MAX; i++) {
            arrHandDiceIcon[i].SetFace(gameController.arrDiceNumber[i]);
        }

        SelectSmallestIncomplete();

        if(gameController.isAutoPlay) {
            Invoke("SelectHighestIncomplete", 1f);
        }
    }

    public void SelectSmallestIncomplete() {
        int index = 0;
        for(int i = 0; i < arrData.Length; i++) {
            if(arrData[i].isComplete == false) {
                index = i;
                break;
            }
        }
        selectedIndex = index;
        UpdateRow();
        UpdateText();
    }

    public void SelectHighestIncomplete() {
        int index = 0;
        int score = 0;
        for(int i = 0; i < arrData.Length; i++) {
            if(arrData[i].isComplete == false && score <= arrData[i].expectScore) {
                index = i;
                score = arrData[i].expectScore;
            }
        }
        selectedIndex = index;
        UpdateRow();
        UpdateText();

        Invoke("ApplySelect", 1.5f);
    }

    private void UpdateRow() {
        for(int i = 0; i < arrRow.Length; i++) {
            if(i == selectedIndex) {
                arrRow[i].transform.localScale = new Vector3(1, selectedScale, 1);
                arrRow[i].SetSelected(true);
                selectedScore = arrData[i].expectScore;
            }
            else {
                arrRow[i].transform.localScale = new Vector3(1, unselectedScale, 1);
                arrRow[i].SetSelected(false);
            }
        }

        if(rowContainerRect != null) {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rowContainerRect);
        }
    }

    private void UpdateText() {
        if(progressText) {
            progressText.text = currentProgress.ToString() + "/" + maxProgress.ToString();
        }
        if(scoreText) {
            scoreText.text = currentScore.ToString();
        }
        if(scoreSelectedText) {
            scoreSelectedText.text = "+" + selectedScore.ToString();
        }
    }

     IEnumerator ApplyCoroutine(float delay) {
        if(touchBlocker != null) {
            touchBlocker.BlockTouch();
        }

        for(int i = 0; i < DEFS.DICE_MAX; i++) {
            arrRow[selectedIndex].SetNumber(i, gameController.arrDiceNumber[i]);
        }
        arrRow[selectedIndex].coverObject.SetActive(false);
        
        int prevProgress = currentProgress;
        int nextProgress = currentProgress + 1;
        int prevCurrentScore = currentScore;
        int nextCurrentScore = currentScore + selectedScore;
        int prevSelectedScore = selectedScore;
        int nextSelectedScore = 0;

        float elapsedTime = 0f;
        float currentTime = Time.time;
        float recentTime = currentTime;
        float degree = 0f;

        while(degree < 1f) {
            currentTime = Time.time;
            elapsedTime += currentTime - recentTime;
            recentTime = currentTime;

            degree = elapsedTime / delay;
            
            float opacity = Mathf.Lerp(0, 1, degree);
            currentProgress = (int)Mathf.Lerp(prevProgress, nextProgress, degree);
            currentScore = (int)Mathf.Lerp(prevCurrentScore, nextCurrentScore, degree);
            selectedScore = (int)Mathf.Lerp(prevSelectedScore, nextSelectedScore, degree);

            for(int i = 0; i < DEFS.DICE_MAX; i++) {
                arrRow[selectedIndex].SetOpacity(i, opacity);
            }
            UpdateText();

            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        if(touchBlocker != null) {
            touchBlocker.UnblockTouch();
        }

        gameController.SetupNextTurn();
        
        CloseSelf();
    }
}
