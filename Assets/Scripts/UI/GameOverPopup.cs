using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPopup : PopupObject
{
    public Text resultText = null;
    public Text scoreText = null;

    GameController gameController = null;
    PlayerManager playerManager = null;

    private void Awake() {
        gameController = GameController.getInstance();
        playerManager = PlayerManager.getInstance();
    }

    public override void Refresh() {
        int size = playerManager.GetPlayerSize();
        string[] names = new string[size];
        int[] scores = new int[size];
        for(int i = 0; i < size; i++) {
            names[i] = playerManager.GetPlayerData(i).name;
            scores[i] = playerManager.GetPlayerData(i).currentScore;
        }

        if(scores[0] == scores[1]) {
            resultText.text = "DRAW";
        }
        else if(scores[1] < scores[0]) {
            resultText.text = names[0] + " WIN";
        }
        else {
            resultText.text = names[1] + " WIN";
        }
        string text = names[0] + " : " + scores[0].ToString() + "\n" + names[1] + " : " + scores[1].ToString();
        scoreText.text = text;
    }

    public void ClickNewGame() {
        gameController.SetupNewGame();

        CloseSelf();
    }
}
