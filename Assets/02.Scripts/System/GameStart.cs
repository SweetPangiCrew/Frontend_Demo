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
    private UnityEngine.UI.Button startBtn;

    public GameObject back, tuto;
    string nextSceneName = "MainTest";
    
    // Start is called before the first frame update
    void Start()
    {
        ClickLocalURL();
        baseInput.text = "agenti_15";
        Database.Instance.simCode =  "agenti_15";
        
        startBtn = gameObject.GetComponent<UnityEngine.UI.Button>();
        startBtn.interactable = false;
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
        
        if(newValue!="") startBtn.interactable = true;
    }

    void gameStart()
    {
        errText.text = ""; 
        StartCoroutine( NPCServerManager.Instance.PostGameStartoroutineWithText(Database.Instance.simCode,Database.Instance.gameName,errText,back,tuto));
        Invoke("onTutorial", 1f);
    }
    void onTutorial()
    {
        if (errText.text == "")
        {
            back.SetActive(false);
            tuto.SetActive(true);
        }
    }

    public void testStartButton()
    {
        
        SceneManager.LoadScene("Map");
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
