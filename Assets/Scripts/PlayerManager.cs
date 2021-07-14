using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public PlayerData[] arrPlayerData { get; private set; } = new PlayerData[DEFS.PLAYER_MAX];
    private int currentPlayerIndex = 0;

    private static PlayerManager playerManager = null;
    static public PlayerManager getInstance() {
        if(playerManager == null) {
            GameObject gameObject = GameObject.FindWithTag("PlayerManager");    
            if(gameObject != null) {
                playerManager = gameObject.GetComponent<PlayerManager>();
            }
        }
        return playerManager;
    }

    public void SetupPlayer(int idx, string name, bool isAutoPlay = false) {
        if(idx < 0 || arrPlayerData.Length <= idx) {
            return;
        }

        arrPlayerData[idx] = new PlayerData();
        arrPlayerData[idx].Initialize(name, isAutoPlay);
    }

    public PlayerData GetCurrentPlayerData() {
        return arrPlayerData[currentPlayerIndex];
    }

    public PlayerData GetPlayerData(int idx) {
        return arrPlayerData[idx];
    }

    public int GetPlayerSize() {
        return arrPlayerData.Length;
    }

    public bool GetPlayerDoneAll() {
        for(int i = 0; i < arrPlayerData.Length; i++) {
            for(int j = 0; j < arrPlayerData[i].arrScoreData.Length; j++) {
                if(arrPlayerData[i].arrScoreData[j].isComplete == false) {
                    return false;
                }
            }
        }
        return true;
    }

    public void ToNextPlayer() {
        currentPlayerIndex++;
        if(arrPlayerData.Length <= currentPlayerIndex) {
            currentPlayerIndex = 0;
        }
    }

    public bool GetCurrentPlayerAuto() {
        return arrPlayerData[currentPlayerIndex].isAutoPlay;
    }

    public void ResetProperty() {
        currentPlayerIndex = 0;
        for(int i = 0; i < arrPlayerData.Length; i++) {
            arrPlayerData[i].ResetProperty();
        }
    }
}
