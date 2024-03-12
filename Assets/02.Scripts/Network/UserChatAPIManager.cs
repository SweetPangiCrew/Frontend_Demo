using System;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NPCServer;
using System.Linq;
using UnityEngine.SceneManagement;


public class UserChatAPIManager : HttpServerBase
{
    public static UserChatAPIManager Instance { get; private set; }
    
    [SerializeField]
    private  Dictionary<string,string> _responseMessageInfo;
    
    public Dictionary<string,string> ResponseMessageInfo { get => _responseMessageInfo;
        set
        {
            _responseMessageInfo = value;
        }
    }

    public bool isResponded = false;
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
        //GameURL.NPCServer.Server_URL = GameURL.NPCServer.Local_URL;
       // StartCoroutine(SendMessageCoroutine(Database.Instance.gameName,"이자식","hi",0));
    }

   
    public IEnumerator SendMessageCoroutine(string game_name, string persona, string message,int _round)
    {
        yield return SendMessage(game_name,persona,message,_round);
    }
    
     public Coroutine SendMessage(string game_name, string persona, string message,int round,
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.sendUserMessage + game_name; 

        // Newtonsoft.Json
        JObject jobj = new JObject();
        Dictionary<string,string> jsonFileContent =  new Dictionary<string,string>();
        
        jsonFileContent["persona"] = persona;
        jsonFileContent["message"] = message;
        jsonFileContent["round"] = round.ToString();
        
        string jsonString = JsonConvert.SerializeObject(jsonFileContent);

        //{ "persona" : "이자식", "message": "안녕하세요!", "round": 0}
        jobj = JObject.Parse(jsonString);
        isResponded = false;
        
        Action<Result> updateInfoAction = (result) =>
        {
           //{"response": "안녕하세요!",round: "0", end: false}
            var resultData = JObject.Parse(result.Json)["body"]; 
            
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            
            resultDic["response"] = (string)resultData["response"];
            resultDic["round"]= (string)resultData["round"]; 
            resultDic["end"] = (string)resultData["end"];
            
            _responseMessageInfo = resultDic;

            isResponded = true;
        };

        onSucceed += updateInfoAction;
        return StartCoroutine(SendRequestCor(url, SendType.POST, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
    
}
