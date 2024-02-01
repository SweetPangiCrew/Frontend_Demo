using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NPCServer;
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
        GameManager.Instance.simCode = newValue;
    }

    void OnNameValueChanged(string newValue)
    {
        GameManager.Instance.gameName = newValue;
    }

    void gameStart()
    {
        StartCoroutine( NPCServerManager.Instance.PostGameStartoroutine(GameManager.Instance.simCode,GameManager.Instance.gameName));
      //  Debug.Log(+ );
    }
}
