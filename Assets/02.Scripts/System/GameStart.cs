using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;
using NPCServer;
using TMPro;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public TMP_InputField baseInput;
    public TMP_InputField nameInput;

    public TMP_Text errText;
    private Button startBtn;
    
    string nextSceneName = "MainTest";
    
    // Start is called before the first frame update
    void Start()
    {
        baseInput.text = "agenti_15";
        Database.Instance.simCode =  "agenti_15";
        
        startBtn = gameObject.GetComponent<Button>();
        baseInput.onValueChanged.AddListener(OnBaseValueChanged);
        nameInput.onValueChanged.AddListener(OnNameValueChanged);
       // startBtn.onSubmit.AddListener(gameStart);
    }

    void OnBaseValueChanged(string newValue)
    {
        Database.Instance.simCode = newValue;
    }

    void OnNameValueChanged(string newValue)
    {
        Database.Instance.gameName = newValue;
        errText.text = "";
    }

    void gameStart()
    {
        StartCoroutine( NPCServerManager.Instance.PostGameStartoroutineWithText(Database.Instance.simCode,Database.Instance.gameName,errText));
      //  Debug.Log(+ );
    }

    public void ClickLocalURL()
    {
        GameURL.NPCServer.Server_URL = GameURL.NPCServer.Local_URL;
    }
    
    public void ClickServerURL()
    {
        GameURL.NPCServer.Server_URL = GameURL.NPCServer.Remote_URL;
    }
    
}
