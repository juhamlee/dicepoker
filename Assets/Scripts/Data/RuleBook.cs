using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class RuleBook
{
    private static RuleBook instance = null;
    public static RuleBook getInstance { 
        get { if(instance == null) instance = new RuleBook(); return instance; }
    }

    private Rule[] arrRule;

    public void ParseData(TextAsset data) {
        if(data == null) return;
        
        JSONNode root = JSON.Parse(data.text);
        
        arrRule = new Rule[root.Count];
        for(int i = 0; i < root.Count; i++) {
            arrRule[i].InitializeFromJSON(root[i]);
        }
    }

    public string GetRuleName(int index) {
        if(index < 0 || arrRule.Length <= index) return "";
        return arrRule[index].name;
    }

    public int GetRuleSize() {
        return arrRule.Length;
    }

    public int GetScoreFromHand(int index, int[] arrHand) {
        if(index < 0 || arrRule.Length <= index) {
            return -1;
        }

        int score = arrRule[index].GetScore(arrHand);
        return score;
    }
}

public struct Rule {
    public enum MATCH_TYPE {
        HIT_ALL = 0,
        HIT_PART,
        INCLUDING,
        NONE
    }
    public enum SCORE_TYPE {
        SUM_HITS = 0,
        SUM_ALL,
        FIXED
    }

    public int index { get; private set; }
    public string name { get; private set; }
    public MATCH_TYPE matchType { get; private set; }
    public SCORE_TYPE scoreType { get; private set; }
    public int fixedScore { get; private set; }
    public int[][] arrHand { get; private set; }

    public void InitializeFromJSON(JSONNode json) {
        index = json["index"];
        name = json["name"];
        string matchtype = json["matchtype"];
        switch (matchtype) {
            case "hit_all": matchType = Rule.MATCH_TYPE.HIT_ALL; break;
            case "hit_part": matchType = Rule.MATCH_TYPE.HIT_PART; break;
            case "including": matchType = Rule.MATCH_TYPE.INCLUDING; break;
            case "none": matchType = Rule.MATCH_TYPE.NONE; break;
        }
        string scoretype = json["scoretype"];
        switch (scoretype) {
            case "sum_hits": scoreType = Rule.SCORE_TYPE.SUM_HITS; break;
            case "sum_all": scoreType = Rule.SCORE_TYPE.SUM_ALL; break;
            case "fixed": scoreType = Rule.SCORE_TYPE.FIXED; break;
        }
        if (scoreType == Rule.SCORE_TYPE.FIXED) {
            fixedScore = json["score"];
        }

        JSONNode node = json["hands"];
        arrHand = new int[node.Count][];
        for (int i = 0; i < node.Count; i++) {
            arrHand[i] = new int[node[i].Count];
            for (int j = 0; j < node[i].Count; j++) {
                arrHand[i][j] = node[i][j];
            }
        }
    }

    public override string ToString() {
        string ret = "name: " + name;

        switch(matchType) {
            case MATCH_TYPE.HIT_ALL:    ret += ", matchtype: " + "HIT ALL"; break;
            case MATCH_TYPE.HIT_PART:   ret += ", matchtype: " + "HIT PART"; break;
            case MATCH_TYPE.INCLUDING:  ret += ", matchtype: " + "INCLUDING"; break;
            case MATCH_TYPE.NONE:       ret += ", matchtype: " + "NONE"; break;
            default: break;
        }

        switch(scoreType) {
            case SCORE_TYPE.SUM_HITS:   ret += ", scoretype: " + "SUM HITS"; break;
            case SCORE_TYPE.SUM_ALL:    ret += ", scoretype: " + "SUM ALL"; break;
            case SCORE_TYPE.FIXED:      ret += ", scoretype: " + "FIXED"; break;
        }

        if(0 < fixedScore) {
            ret += ", score: " + fixedScore;
        }

        for(int i = 0; i < arrHand.Length; i++) {
            ret += "\n { " + string.Join(",", arrHand[i]) + " }";
        }
        
        return ret;
    }

    public int GetScore(int[] arrTargetHand) {
        switch(matchType) {
            case MATCH_TYPE.HIT_ALL:    return GetScoreByHitAll(arrTargetHand);
            case MATCH_TYPE.HIT_PART:   return GetScoreByHitPart(arrTargetHand);
            case MATCH_TYPE.INCLUDING:  return GetScoreByIncluding(arrTargetHand);
            case MATCH_TYPE.NONE:       return GetScoreByNone(arrTargetHand);
            default: return 0;
        }
    }

    public int GetScoreByHitAll(int[] arrTargetHand) {
        int[] arrHit = new int[arrTargetHand.Length];
        bool isMatch = false;
        for(int i = 0; i < arrHand.Length; i++) {
            bool isFind = true;
            for(int j = 0; j < arrHand[i].Length; j++) {
                if(arrTargetHand[j] != arrHand[i][j]) {
                    isFind = false;
                    break;
                }
            }
            if(isFind) {
                Array.Copy(arrTargetHand, arrHit, arrTargetHand.Length);
                isMatch = true;
                break;
            }
        }
        
        return GetScoreByScoreType(arrHit, arrTargetHand, isMatch);
    }

    public int GetScoreByHitPart(int[] arrTargetHand) {
        int[] arrHit = new int[arrTargetHand.Length];
        for(int i = 0; i < arrHand.Length; i++) {
            int number = arrHand[i][0];
            for(int j = 0; j < arrHand[i].Length; j++) {
                if(arrHand[i][j] == arrTargetHand[j]) {
                    arrHit[j] = arrHand[i][j];
                }
            }
        }

        return GetScoreByScoreType(arrHit, arrTargetHand);
    }

    public int GetScoreByIncluding(int[] arrTargetHand) {
        int[] arrHit = new int[arrTargetHand.Length];
        for(int i = 0; i < arrHand.Length; i++) {
            int startIndex = 0;
            for(int j = 0; j < arrTargetHand.Length; j++) {
                if(arrHand[i][0] == arrTargetHand[j]) {
                    startIndex = j;
                    break;
                }
            }

            bool isMatch = true;
            for(int j = 0; j < arrHand[i].Length; j++) {
                if(arrTargetHand.Length <= startIndex + j) {
                    isMatch = false;
                    break;
                }
                if(arrTargetHand[startIndex + j] != arrHand[i][j]) {
                    isMatch = false;
                    break;
                }
                arrHit[j] = arrHand[i][j];
            }

            if(isMatch) {
                return GetScoreByScoreType(arrHit, arrTargetHand);
            }
        }
        return 0;
    }

    public int GetScoreByNone(int[] arrTargetHand) {
        int[] arrHit = new int[arrTargetHand.Length];
        Array.Copy(arrTargetHand, arrHit, arrTargetHand.Length);
        
        return GetScoreByScoreType(arrHit, arrTargetHand);
    }

    private int GetScoreByScoreType(int[] arrHit, int[] arrTargetHand, bool isMatch = true) {
        int score = 0;
        if(scoreType == SCORE_TYPE.SUM_HITS) {
            for(int i = 0; i < arrHit.Length; i++) {
                score += arrHit[i];
            }
        }
        else if(scoreType == SCORE_TYPE.SUM_ALL) {
            for(int i = 0; i < arrTargetHand.Length; i++) {
                score += arrTargetHand[i];
            }
        }
        else if(scoreType == SCORE_TYPE.FIXED && isMatch) {
            score = fixedScore;
        }
        return score;
    }
} 