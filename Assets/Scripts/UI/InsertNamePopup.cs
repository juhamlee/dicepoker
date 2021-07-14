using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InsertNamePopup : PopupObject
{
    public Text textInputName = null;

    public void ClickOk() {
        GameController.SetPlayerName(textInputName.text);
        SceneManager.LoadScene(1);
    }
}
