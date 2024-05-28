using System;

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NPCServer;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;


public class UserNetworkManager : HttpServerBase
{
    public static UserNetworkManager Instance {get; private set;}
    
    [FormerlySerializedAs("uuid")] [SerializeField]
    private string uuid;
    
    public TMP_InputField inputField;
    public UnityEngine.UI.Button submitButton;
    public GameObject uiPanel;
    
    public string ExistingInfo { get => uuid;
        set
        {
            uuid = value;
        }
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
           
        }
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        
        string local_uuid = LoadString("uuid");

        if (local_uuid != "NULL")
        {
          
            
            uiPanel.SetActive(false);
            
            
        }
        
        inputField.onValueChanged.AddListener(ValidateInput);
    }
    
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) )
        {
            if (!string.IsNullOrEmpty(inputField.text))
            {
                ReadInputField();
                uiPanel.SetActive(false);
            }
        }
    }
    void ValidateInput(string text)
    {
        // 입력된 텍스트의 유효성을 검사하고, 텍스트가 비어있으면 버튼을 비활성화합니다.
        submitButton.interactable = !string.IsNullOrEmpty(text);
    }
        
    public void ReadInputField()
    {
        string inputText = inputField.text; // InputField의 텍스트 값을 가져옵니다.
        
        StartCoroutine(PostUserNameCoroutine(inputText));
        Debug.Log("Entered Text: " + inputText); // 콘솔에 텍스트 출력
    }
    
    public IEnumerator PostUserNameCoroutine(string name)
    {
        yield return PostUserName(name);
    }
    
     public Coroutine PostUserName(string name,
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.registUserName; 

        //Newtonsoft.Json
        JObject jobj = new JObject();

        jobj["username"] = name;
        SaveString("username", name);
        Database.Instance.username = name;
        Action<Result> updateInfoAction = (result) =>
        {

            var resultData = JObject.Parse(result.Json);
            uuid = resultData["uuid"].Value<string>();
            Database.Instance.uuid = uuid;
            SaveString("uuid", uuid);

        };

        onSucceed += updateInfoAction;
        return StartCoroutine(SendRequestCor(url, SendType.POST, jobj, onSucceed, onFailed, onNetworkFailed));
    }
     
     public void SaveString(string key, string value)
     {
         PlayerPrefs.SetString(key, value);
         PlayerPrefs.Save(); // 변경사항을 디스크에 즉시 저장
     }
     
     public string LoadString(string key)
     {
         return PlayerPrefs.GetString(key, "NULL");
     }
     void OnDestroy()
     {
         // 리스너를 제거하여 리소스 누수를 방지합니다.
         inputField.onValueChanged.RemoveListener(ValidateInput);
     }
    
     
     
    
}
