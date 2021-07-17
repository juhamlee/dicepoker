using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecordPopup : SlideObject
{
    public Text progressText = null;
    public Text scoreText = null;
    public Text buttonNameText = null;
    public GameObject[] arrButtonSelectObeject = null;
    public VerticalLayoutGroup rowContainer = null;
    public PlayerRecordRowController prefabRow = null;
    public GameObject prefabSeperator = null;
    public GameObject[] arrVerticalSeperator = null;

    private PlayerRecordRowController[] arrRow = null;
    private int currentIndex = 0;

    private GameController gameController = null;
    private PlayerManager playerManager = null;

    protected override void Awake() {
        base.Awake();
        gameController = GameController.getInstance();
        playerManager = PlayerManager.getInstance();

        int ruleSize = RuleBook.getInstance.GetRuleSize();
        arrRow = new PlayerRecordRowController[ruleSize];
        for(int i = 0; i < ruleSize; i++) {
            Instantiate<GameObject>(prefabSeperator, rowContainer.transform);
            arrRow[i] = Instantiate<PlayerRecordRowController>(prefabRow, rowContainer.transform);
            arrRow[i].name = "Record Row" + (i + 1).ToString();
        }
        Instantiate<GameObject>(prefabSeperator, rowContainer.transform);

        float totalHeight = 0;
        if(prefabSeperator.TryGetComponent<RectTransform>(out RectTransform rectSeperator)) {
            if(prefabRow.TryGetComponent<RectTransform>(out RectTransform rectRow)) {
                totalHeight += rectSeperator.sizeDelta.y * (ruleSize + 1);
                totalHeight += rectRow.sizeDelta.y * ruleSize;
                totalHeight += 80f;

                for(int i = 0; i < arrVerticalSeperator.Length; i++) {
                    arrVerticalSeperator[i].TryGetComponent<RectTransform>(out RectTransform rectVertical);
                    if(rectVertical) {
                        rectVertical.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);
                    }
                }
            }
        }

        PlayerData playerData = playerManager.GetPlayerData(0);
        if(playerData != null) {
            buttonNameText.text = playerData.name;
        }
    }

    public void ClickSlide(int index) {
        if(isOpen == false) {
            currentIndex = index;
            gameController.OpenPlayerSheet(currentIndex);
        }
        else {
            CloseSelf();
        }
    }

    public void ClickMark(int index) {
        if(currentIndex != index) {
            currentIndex = index;
            RefreshByPlayerIndex(currentIndex);
        }
    }

    public void RefreshByPlayerIndex(int index) {
        PlayerData playerData = playerManager.GetPlayerData(index);
        if(playerData == null) {
            return;
        }

        for(int i = 0; i < arrButtonSelectObeject.Length; i++) {
            if(i == index) {
                arrButtonSelectObeject[i].SetActive(true);
            }
            else {
                arrButtonSelectObeject[i].SetActive(false);
            }
        }

        int maxProgress = RuleBook.getInstance.GetRuleSize();
        int currentProgress = 0;
        int currentScore = 0;
        for(int i = 0; i < arrRow.Length; i++) {
            ScoreData scoreData = playerData.arrScoreData[i];
            arrRow[i].SetData(scoreData.name, scoreData.arrHand, scoreData.score, scoreData.isComplete);
            if(scoreData.isComplete) {
                currentProgress++;
                currentScore += scoreData.score;
            }
        }
        progressText.text = currentProgress.ToString() + "/" + maxProgress.ToString();
        scoreText.text = "Score " + currentScore.ToString();
    }
}
