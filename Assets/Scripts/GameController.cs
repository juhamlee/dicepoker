using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    enum POPUP {
        DICE_SELECT,
        HAND_MATCH,
        PLAYER_INFO,
        GAME_OVER,

        POPUP_SIZE,
    };

    private static string PlayerName = "";

    public TextAsset ruleScript = null;
    public Button buttonRoll = null;
    [Header("POPUP / SLIDE")]
    public PopupObject[] arrPopupObject = new PopupObject[(int)POPUP.POPUP_SIZE];
    public NoticeController noticeMessage = null;
    
    public int[] arrDiceNumber { get; private set; } = new int[DEFS.DICE_MAX];
    public bool isAutoPlay { get; private set; }= false;
    private bool isRerollable = true;

    private PopupManager popupManager = null;
    private TableManager tableManager = null;
    private PlayerManager playerManager = null;

    private static GameController gameController = null;
    public static GameController getInstance() {
        if(gameController == null) {
            GameObject gameObject = GameObject.FindWithTag("GameController");    
            if(gameObject != null) {
                gameController = gameObject.GetComponent<GameController>();
            }
        }
        return gameController;
    }

    public static void SetPlayerName(string name) {
        PlayerName = name;
    }

    private void Awake() {
        RuleBook.getInstance.ParseData(ruleScript);

        popupManager = PopupManager.getInstance();
        tableManager = TableManager.getInstance();
        playerManager = PlayerManager.getInstance();
    }

    private void Start() {
        SetupGame(PlayerName);
    }

    public void SetupGame(string name) {
        playerManager.SetupPlayer(0, name);
        playerManager.SetupPlayer(1, "COM", true);

        isAutoPlay = playerManager.GetCurrentPlayerAuto();

        ResetProperty();
        SetupTable();

        PopupObject popup = GetPopupObject(POPUP.PLAYER_INFO);
        popup.gameObject.SetActive(true);

        CheckAutoPlay();
    }

    public void Roll() {
        bool ret = tableManager.Roll();
        if(ret) {
            buttonRoll.gameObject.SetActive(false);
        }
    }

    public void SetupReroll(bool[] selected) {
        if(isRerollable == false) {
            return;
        }
        isRerollable = false;

        for(int i = 0; i < arrDiceNumber.Length; i++) {
            if(selected[i] == false) {
                arrDiceNumber[i] = 0;
            }
        }
        Array.Sort(arrDiceNumber);
        SetupTable();
    }

    public void RollResult(int[] arrResult) {
        Array.Copy(arrResult, arrDiceNumber, arrResult.Length);
        Array.Sort(arrDiceNumber);

        if(isRerollable) {
            SetupDiceSelect();
        }
        else {
            SetupHandMatch();
        }
    }

    public void SetupDiceSelect() {
        OpenPopup(POPUP.DICE_SELECT);
    }
    public void SetupHandMatch() {
        OpenPopup(POPUP.HAND_MATCH);
    }

    public void SetupNextTurn() {
        if(playerManager.GetPlayerDoneAll()) {
            noticeMessage.gameObject.SetActive(false);
            OpenPopup(POPUP.GAME_OVER);
        }
        else {
            playerManager.ToNextPlayer();

            isAutoPlay = playerManager.GetCurrentPlayerAuto();

            ResetProperty();
            SetupTable();

            CheckAutoPlay();
        }
    }

    public void SetupNewGame() {
        playerManager.ResetProperty();

        isAutoPlay = playerManager.GetCurrentPlayerAuto();

        ResetProperty();
        SetupTable();

        CheckAutoPlay();
    }

    private void ResetProperty() {
        for(int i = 0; i < arrDiceNumber.Length; i++) {
            arrDiceNumber[i] = 0;
        }
        isRerollable = true;
    }

    private void SetupTable() {
        int spawnCount = 0;
        for(int i = 0; i < arrDiceNumber.Length; i++) {    
            if(arrDiceNumber[i] == 0) {
                spawnCount++;
            }
        }
        tableManager.SpawnDice(spawnCount);
        buttonRoll.gameObject.SetActive(true);
    }

    private void CheckAutoPlay() {
        if(isAutoPlay) {
            isRerollable = false;
            buttonRoll.gameObject.SetActive(false);
            if(noticeMessage) {
                noticeMessage.SetMessage("COM is playing");
                noticeMessage.gameObject.SetActive(true);
            }

            Invoke("Roll", 2f);
        }
        else {
            if(noticeMessage) {
                noticeMessage.gameObject.SetActive(false);
            }
        }
    }

    public void OpenPlayerSheet(int index) {
        PopupObject popup = GetPopupObject(POPUP.PLAYER_INFO);
        if(popup.TryGetComponent<PlayerRecordPopup>(out PlayerRecordPopup playerRecord)) {
            playerRecord.RefreshByPlayerIndex(index);
            popupManager.OpenPopup(popup);
        }
    }

    private void OpenPopup(POPUP p) {
        if(popupManager == null) {
            return;
        }

        PopupObject popup = GetPopupObject(p);
        popupManager.OpenPopup(popup);
    }

    private PopupObject GetPopupObject(POPUP p) {
        if(arrPopupObject == null) {
            return null;
        }
        int index = (int)p;
        return arrPopupObject[index];
    }
}
