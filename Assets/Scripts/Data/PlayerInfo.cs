using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
    public string name { set; get; }
    public int currentScore { set; get; }
    public ScoreData[] arrScoreData = null;
    public bool isAutoPlay { get; private set; }

    public void Initialize(string _name, bool _isAutoPlay) {
        name = _name;
        currentScore = 0;
        isAutoPlay = _isAutoPlay;

        int ruleSize = RuleBook.getInstance.GetRuleSize();
        arrScoreData = new ScoreData[ruleSize];
        for(int i = 0; i < ruleSize; i++) {
            arrScoreData[i] = new ScoreData();
            string ruleName = RuleBook.getInstance.GetRuleName(i);
            arrScoreData[i].Initialize(ruleName, DEFS.DICE_MAX);
        }
    }

    public void ResetProperty() {
        currentScore = 0;

        for(int i = 0; i < arrScoreData.Length; i++) {
            arrScoreData[i].ResetProperty();
        }
    }

    public void UpdateScoreData(int index, int[] arrHand, int score, bool isComplete) {
        if(arrScoreData.Length <= index) {
            return;
        }
        arrScoreData[index].UpdateData(arrHand, score, isComplete);

        currentScore = 0;
        for(int i = 0; i < arrScoreData.Length; i++) {
            currentScore += arrScoreData[i].score;
        }

    }
}

public class ScoreData {
    public string name { get; private set;}
    public int[] arrHand { get; private set;}
    public int score { get; private set;}
    public bool isComplete { get; private set;}

    public void Initialize(string _name, int _size) {
        name = _name;
        arrHand = new int[_size];
        
        ResetProperty();
    }

    public void ResetProperty() {
        for(int i = 0; i < arrHand.Length; i++) {
            arrHand[i] = 0;
        }
        score = 0;
        isComplete = false;
    }

    public void UpdateData(int[] _arrHand, int _score, bool _isComplete) {
        Array.Copy(_arrHand, arrHand, DEFS.DICE_MAX);
        score = _score;
        isComplete = _isComplete;
    }
}