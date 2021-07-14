using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    public GameObject dicePrefab = null;
    public Transform diceContainer = null;
    
    private DiceController[] arrDice = null;
    private int rollingCount = 0;
    private ObjectPooler dicePooler = null;
    private GameController gameController = null;

    private readonly static Vector3[][] arrDiceInitialPosition = new Vector3[][] {
        new Vector3[] {new Vector3(0, 0.6f, -0.5f)},
        new Vector3[] {new Vector3(-1.2f, 0.6f, -0.5f), new Vector3(1.2f, 0.6f, -0.5f)},
        new Vector3[] {new Vector3(-2.4f, 0.6f, -0.5f), new Vector3(0, 0.6f, -0.5f), new Vector3(2.4f, 0.6f, -0.5f)},
        new Vector3[] {new Vector3(-0.6f, 0.6f, -2.0f), new Vector3(1.8f, 0.6f, -2.0f), new Vector3(-1.8f, 0.6f, 1.0f), new Vector3(0.6f, 0.6f, 1.0f)},
        new Vector3[] {new Vector3(-1.2f, 0.6f, -2.0f), new Vector3(1.2f, 0.6f, -2.0f), new Vector3(-2.4f, 0.6f, 1.0f), new Vector3(0, 0.6f, 1.0f), new Vector3(2.4f, 0.6f, 1.0f)},
    };

    private static TableManager tableManager = null;
    static public TableManager getInstance() {
        if(tableManager == null) {
            GameObject gameObject = GameObject.FindWithTag("TableManager");    
            if(gameObject != null) {
                tableManager = gameObject.GetComponent<TableManager>();
            }
        }
        return tableManager;
    }

    private void Awake() {
        dicePooler = new ObjectPooler();
        dicePooler.Initialize(dicePrefab, "Dice", 10, diceContainer);
        
        gameController = GameController.getInstance();

        rollingCount = 0;
    }

    public bool Roll() {
        if(CheckDiceStopAll() == false) {
            return false;
        }

        for(int i = 0; i < arrDice.Length; i++) {
            arrDice[i].Roll();
        }

        return true;
    }

    public void SpawnDice(int count) {
        count = Mathf.Clamp(count, 1, DEFS.DICE_MAX);

        RemoveDiceAll();

        Array.Resize<DiceController>(ref arrDice, count);
        for(int i = 0; i < count; i++) {
            GameObject gameObject = dicePooler.Pop();
            gameObject.transform.position = arrDiceInitialPosition[count - 1][i];
            arrDice[i] = gameObject.GetComponent<DiceController>();
        }
    }

    public void RemoveDiceAll() {
        if(arrDice == null) {
            return;
        }

        for(int i = 0; i < arrDice.Length; i++) {
            if(arrDice[i] != null) {
                dicePooler.Push(arrDice[i].gameObject);
            }
        }
    }

    private bool CheckDiceStopAll() {
        for(int i = 0; i < arrDice.Length; i++) {
            if(CheckDiceStop(i) == false) {
                return false;
            }
        }
        return true;
    }

    private bool CheckDiceStop(int index) {
        if(arrDice[index].isRolling) {
            return false;
        }
        return true;
    }

    public void IncreaseRollingCount() {
        rollingCount++;
    }

    public void DecreaseRollingCount() {
        rollingCount--;
        if(rollingCount == 0) {
            int[] arrNumber = new int[arrDice.Length];
            for(int i = 0; i < arrDice.Length; i++) {
                arrNumber[i] = arrDice[i].number;
            }
            gameController.RollResult(arrNumber);
        }
    }
}
