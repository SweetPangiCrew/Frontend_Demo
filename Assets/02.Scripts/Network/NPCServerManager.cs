using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;
using System.Net.Mime;

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

   
    public IEnumerator GetMovementCoroutine(string simName,int step,Action<Result> onSucceed = null)
    {
        yield return GetMovement(simName, step,onSucceed);
    }
    
    public IEnumerator PostPerceiveCoroutine(string data,string simName, int step)
    {
        
        yield return PostPerceive(data, simName, step);
    }
    
    public void gameStart()
    {   
        StartCoroutine( PostGameStartoroutine(Database.Instance.simCode,Database.Instance.gameName));

    }
    
    public IEnumerator PostGameStartoroutine(string simCode, string gameName)
    {
        yield return PostGameStart(simCode, gameName);
    }
    
    public IEnumerator PostGameStartoroutineWithText(string simCode, string gameName, TMP_Text errText, GameObject back, GameObject tuto)
    {
        yield return PostGameStart(simCode, gameName,errText,  back, tuto);
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

        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getNPCMovement+simName+"/"+step+"/"+Database.Instance.uuid;

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
            List<List<List<string>>> chats= new List<List<List<string>>>();
            
            foreach (JProperty property in resultData)
            {
                List<List<string>> chat = new List<List<string>>();
                    personas.Add(property.Name);
                    act_address.Add(property.Value["act_address"].ToString());
                    pronunciatio.Add(property.Value["pronunciatio"].ToString());
                    description.Add(property.Value["description"].ToString());
                    //Debug.Log(property.Value["chat"].ToString());
                    
                    
                    foreach (var chatlist in property.Value["chat"])
                    {
                        var chatEntry = chatlist.Select(item => item.ToString()).ToList();
                        chat.Add(chatEntry);
                  
                    }
                    chats.Add(chat);
                
            }

            currentMovementInfo = new List<Persona>();
            for(int i=0; i< personas.Count; i++)
            {
                Persona newMovementInfo = new Persona(personas[i], act_address[i], pronunciatio[i], description[i], chats[i]);
                CurrentMovementInfo.Add(newMovementInfo);
                _getReaction = true;
                
                //if(chats[i]!=null)
                // Debug.Log(newMovementInfo.ToString());
            }
            
            
        };

        updateMovementInfoAction += onSucceed;
      

        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, updateMovementInfoAction, onFailed, onNetworkFailed));
    }
    
     public Coroutine PostPerceive(string data, string simName, int step, 
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        // 로그??URL??조합
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.postNPCPercention +simName+"/"+step+"/"+Database.Instance.uuid;

      
        JObject jobj = new JObject();
       //현재 로컬 파일로 부르는 중..? daa 쓸 필요 없나?  
       // string jsonFilePath = Path.Combine(Application.dataPath, "NPCPerceiveFile.json");//NPCPerceiveFile  PercevieAPI 이전버전
      //  string jsonFileContent = File.ReadAllText(jsonFilePath);
        
        //넘어온 매개변수 data 이용
       string jsonFileContent = data;


        jobj =  JObject.Parse(jsonFileContent);

        jobj["meta"] = new JObject();
        jobj["meta"]["curr_time"] = Clock.Instance.GetCurrentTime();

        Action<Result> updateMPerceiveInfoAction = (result) =>
        {
            Debug.Log("Post :"+ result.Json);
            _perceived = true;


        };

        onSucceed += updateMPerceiveInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.POST, jobj, onSucceed, onFailed, onNetworkFailed));
    }
     
     public Coroutine PostGameStart(string simCode, string gameName,TMP_Text errText = null,GameObject back=null, GameObject tuto=null, 
         Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
     {
         string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.createGame;

      //{
      // 'user': self.user.uuid, #모델이기 때문에 uuid만 보내도 충분!
      // 'game_name': 'TestGame4',
      // 'sim_code': 'agenti',
      // 'is_completed': False #필수 필드 아님
      // }
      
         JObject jobj = new JObject();

         jobj["user"] = Database.Instance.uuid;
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
        
         Action<Result> showLog = (result) =>
         {
            
             var resultData = JObject.Parse(result.Json)["error"];
             errText.text = resultData.ToString();
             Debug.Log("Faile");
             back.SetActive(true);
             tuto.SetActive(false);

         };
         onSucceed += updateStartInfoAction;
         onNetworkFailed += showLog;
         onFailed += showLog;

         return StartCoroutine(SendRequestCor(url, SendType.POST, jobj, onSucceed, onFailed, onNetworkFailed));
     }
    
}
}

