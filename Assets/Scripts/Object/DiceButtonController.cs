using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceButtonController : MonoBehaviour
{
    public Transform diceTransform = null;
    public GameObject outlineObject = null;

    public void SetFace(int number) {
        if(number == 1) {
            diceTransform.localRotation = Quaternion.AngleAxis(180.0f, new Vector3(1,0,0));
        }
        else if(number == 2) {
            diceTransform.localRotation = Quaternion.AngleAxis(90.0f, new Vector3(1,0,0));
        }
        else if(number == 3) {
            diceTransform.localRotation = Quaternion.AngleAxis(-90.0f, new Vector3(0,1,0));
        }
        else if(number == 4) {
            diceTransform.localRotation = Quaternion.AngleAxis(90.0f, new Vector3(0,1,0));
        }
        else if(number == 5) {
            diceTransform.localRotation = Quaternion.AngleAxis(-90.0f, new Vector3(1,0,0));
        }
        else if(number == 6) {
            diceTransform.localRotation = Quaternion.identity;
        }
    }

    public void EnableOutline(bool enable) {
        if(outlineObject) {
            outlineObject.SetActive(enable);
        }
    }
}
