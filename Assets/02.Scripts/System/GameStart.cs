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

    private Button startBtn;
    
    string nextSceneName = "MainTest";
    
    // Start is called before the first frame update
    void Start()
    {
        baseInput.text = "agenti";
        Database.Instance.simCode =  "agenti";
        
        startBtn = gameObject.GetComponent<Button>();
        baseInput.onValueChanged.AddListener(OnBaseValueChanged);
        nameInput.onValueChanged.AddListener(OnNameValueChanged);
       // startBtn.onClick.AddListener(gameStart);
    }

    void OnBaseValueChanged(string newValue)
    {
        Database.Instance.simCode = newValue;
    }

    void OnNameValueChanged(string newValue)
    {
        Database.Instance.gameName = newValue;
    }

    void gameStart()
    {
        StartCoroutine( NPCServerManager.Instance.PostGameStartoroutine(Database.Instance.simCode,Database.Instance.gameName));
      //  Debug.Log(+ );
    }
}
