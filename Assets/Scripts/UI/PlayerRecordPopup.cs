using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRecordPopup : SlideObject
{
    public Text nameText = null;
    public Text progressText = null;
    public Text scoreText = null;
    public Text buttonNameText = null;
    public VerticalLayoutGroup rowContainer = null;
    public PlayerRecordRowController prefabRow = null;

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
            arrRow[i] = Instantiate<PlayerRecordRowController>(prefabRow, rowContainer.transform);
            arrRow[i].name = "Record Row" + (i + 1).ToString();
        }

        PlayerData playerData = playerManager.GetPlayerData(0);
        if(playerData != null) {
            buttonNameText.text = playerData.name;
        }
    }

    public void ClickMark(int index) {
        if(isOpen == false) {
            currentIndex = index;
            gameController.OpenPlayerSheet(currentIndex);
        }
        else {
            if(currentIndex != index) {
                currentIndex = index;
                RefreshByPlayerIndex(currentIndex);
            }
        }
    }

    public void RefreshByPlayerIndex(int index) {
        PlayerData playerData = playerManager.GetPlayerData(index);
        if(playerData == null) {
            return;
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

        nameText.text = playerData.name;
        progressText.text = currentProgress.ToString() + "/" + maxProgress.ToString();
        scoreText.text = currentScore.ToString();
    }
}
