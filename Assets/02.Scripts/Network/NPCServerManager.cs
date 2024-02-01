using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

using System.IO;
using System.Linq;

namespace NPCServer{
public class NPCServerManager : HttpServerBase
{
    public static NPCServerManager Instance { get; private set; }

    [SerializeField]
    private List<Persona> currentMovementInfo;

    private bool _serverOpened = false;
    private bool _perceived = false;
    private bool _getReaction = false;
    
    public bool serverOpened { get => _serverOpened; set { _serverOpened = value;  } }
    public bool perceived { get => _perceived; set => _perceived = value; }
    public bool getReaction { get => _getReaction; set => _getReaction = value; }
    
    public List<Persona> CurrentMovementInfo { get => currentMovementInfo;
        set
        {
            currentMovementInfo = value;
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

   
    public IEnumerator GetMovementCoroutine(string simName,int step)
    {
        yield return GetMovement(simName, step);
    }
    
    public IEnumerator PostPerceiveCoroutine(string data,string simName, int step)
    {
        yield return PostPerceive(data, simName, step);
    }
    
    public void gameStart()
    {   
        StartCoroutine( PostGameStartoroutine(Database.Instance.simCode,Database.Instance.gameName));

    }
    
    public IEnumerator GetServerTimeCoroutine()
    {
        yield return GetServerTime();
    }
    
    public Coroutine GetServerTime(
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        // 로그??URL??조합
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getServerTime;

        // Newtonsoft.Json ?�키지�??�용??Json?�성
        JObject jobj = new JObject();
        
        // ?�공?�을??콜백
        // ?�로???��? ?�보�??�팅??로그???�청?�했�??�공?�다�???�� ?�데?�트 ?�도�??�려�??�쪽???�의??
        Action<Result> updateServerTimeInfoAction = (result) =>
        {
            // Newtonsoft.Json ?�키지�??�용??Json Parsing
            var resultData = JObject.Parse(result.Json)["serverTime"]; 
            
            Debug.Log("서버 시간"+resultData);
            
            
        };

        onSucceed += updateServerTimeInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public Coroutine GetMovement(string simName, int step,
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {

        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getNPCMovement+simName+"/"+step;

        // Newtonsoft.Json 
        JObject jobj = new JObject();
        

        Action<Result> updateMovementInfoAction = (result) =>
        {
            // Newtonsoft.Json Json Parsing
            var resultData = JObject.Parse(result.Json)["persona"]; 
            
          //  Debug.Log(result);
            
            List<string> personas = new List<string>();
            List<string> act_address   = new List<string>();
            List<string> pronunciatio  = new List<string>();
            List<string> description  = new List<string>();
            List<List<string>> chats = new List<List<string>>();
            
            foreach (JProperty property in resultData)
            {
                    personas.Add(property.Name);
                    act_address.Add(property.Value["act_address"].ToString());
                    pronunciatio.Add(property.Value["pronunciatio"].ToString());
                    description.Add(property.Value["description"].ToString());
                    Debug.Log(property.Value["chat"].ToString());
                    
                    foreach (var chatlist in property.Value["chat"])
                    {
                        var chatEntry = chatlist.Select(item => item.ToString()).ToList();
                        chats.Add(chatEntry);
                    //chats.Add(chatlist.ToObject<List<string>>());
                }
            }


            for(int i=0; i< personas.Count; i++)
            {
                Persona newMovementInfo = new Persona(personas[i], act_address[i], pronunciatio[i], description[i], chats);
                CurrentMovementInfo.Add(newMovementInfo);
                _getReaction = true;
                Debug.Log(newMovementInfo.ToString());
            }
            
            
        };

        onSucceed += updateMovementInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
     public Coroutine PostPerceive(string data, string simName, int step, 
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        // 로그??URL??조합
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.postNPCPercention +simName+"/"+step+"/";

        // Newtonsoft.Json ?�키지�??�용??Json?�성
        JObject jobj = new JObject();
        
        
        string jsonFilePath = Path.Combine(Application.dataPath, "PercevieAPI.json");
        string jsonFileContent = File.ReadAllText(jsonFilePath);

        jobj = JObject.Parse(jsonFileContent);
        

        Action<Result> updateMPerceiveInfoAction = (result) =>
        {
            Debug.Log("Post :"+ result.Json);
            _perceived = true;


        };

        onSucceed += updateMPerceiveInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.POST, jobj, onSucceed, onFailed, onNetworkFailed));
    }
     
     public Coroutine PostGameStart(string simCode, string gameName,
         Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
     {
         string url = GameURL.NPCServer.Server_URL +"gamestart/";

      
         JObject jobj = new JObject();


         jobj["sim_code"] = simCode;
         jobj["game_name"] = gameName;
        
        
         //call back
         Action<Result> updateStartInfoAction = (result) =>
         {
             // Newtonsoft.Json / Json Parsing
             //  var resultData = JObject.Parse(result.Json)["persona"]; 
             _serverOpened = true;
             //  Debug.Log(result);
             Debug.Log("Post :"+ result.Json);
            
         };

         onSucceed += updateStartInfoAction;

         return StartCoroutine(SendRequestCor(url, SendType.POST, jobj, onSucceed, onFailed, onNetworkFailed));
     }
    
}
}

