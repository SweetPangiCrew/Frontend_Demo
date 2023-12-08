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


    #region ?„ë¡œ?¼í‹°
    public List<Persona> CurrentMovementInfo { get => currentMovementInfo;
        set
        {
            currentMovementInfo = value;
        }
    }
    #endregion

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

    public IEnumerator GetMovementCoroutine(int step)
    {
        yield return GetMovement("test4", step);
    }
    
    public IEnumerator PostPerceiveCoroutine(string data, int step)
    {
        yield return PostPerceive(data, "test6", step);
    }
    
    public IEnumerator GetServerTimeCoroutine()
    {
        yield return GetServerTime();
    }
    
    public Coroutine GetServerTime(
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        // ë¡œê·¸??URL??ì¡°í•©
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getServerTime;

        // Newtonsoft.Json ?¨í‚¤ì§€ë¥??´ìš©??Json?ì„±
        JObject jobj = new JObject();
        
        // ?±ê³µ?ˆì„??ì½œë°±
        // ?ˆë¡œ??? ì? ?•ë³´ë¥??¸íŒ…??ë¡œê·¸???”ì²­?„í–ˆê³??±ê³µ?ˆë‹¤ë©???ƒ ?…ë°?´íŠ¸ ?˜ë„ë¡?? ë ¤ê³??´ìª½???•ì˜??
        Action<Result> updateServerTimeInfoAction = (result) =>
        {
            // Newtonsoft.Json ?¨í‚¤ì§€ë¥??´ìš©??Json Parsing
            var resultData = JObject.Parse(result.Json)["serverTime"]; 
            
            Debug.Log("?œë²„?œê°„"+resultData);
            
            
        };

        onSucceed += updateServerTimeInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public Coroutine GetMovement(string simName, int step,
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        // ë¡œê·¸??URL??ì¡°í•©
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getNPCMovement+simName+"/"+step;

        // Newtonsoft.Json ?¨í‚¤ì§€ë¥??´ìš©??Json?ì„±
        JObject jobj = new JObject();
        
        // ?±ê³µ?ˆì„??ì½œë°±
        // ?ˆë¡œ??? ì? ?•ë³´ë¥??¸íŒ…??ë¡œê·¸???”ì²­?„í–ˆê³??±ê³µ?ˆë‹¤ë©???ƒ ?…ë°?´íŠ¸ ?˜ë„ë¡?? ë ¤ê³??´ìª½???•ì˜??
        Action<Result> updateMovementInfoAction = (result) =>
        {
            // Newtonsoft.Json ?¨í‚¤ì§€ë¥??´ìš©??Json Parsing
            var resultData = JObject.Parse(result.Json)["persona"]; 
            
          //  Debug.Log(result);
            
            //resutlData ?œíšŒ?˜ë©´??ê°ê°???•ë³´ë¥?ê°€?¸ì˜´
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
                Debug.Log(newMovementInfo.ToString());
            }
            
            
        };

        onSucceed += updateMovementInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
     public Coroutine PostPerceive(string data, string simName, int step, 
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        // ë¡œê·¸??URL??ì¡°í•©
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.postNPCPercention +simName+"/"+step+"/";

        // Newtonsoft.Json ?¨í‚¤ì§€ë¥??´ìš©??Json?ì„±
        JObject jobj = new JObject();
        
        
        string jsonFilePath = Path.Combine(Application.dataPath, "PercevieAPI.json");
        string jsonFileContent = File.ReadAllText(jsonFilePath);

        jobj = JObject.Parse(jsonFileContent);
        
        // ?±ê³µ?ˆì„??ì½œë°±
        // ?ˆë¡œ??? ì? ?•ë³´ë¥??¸íŒ…??ë¡œê·¸???”ì²­?„í–ˆê³??±ê³µ?ˆë‹¤ë©???ƒ ?…ë°?´íŠ¸ ?˜ë„ë¡?? ë ¤ê³??´ìª½???•ì˜??
        Action<Result> updateMPerceiveInfoAction = (result) =>
        {
            // Newtonsoft.Json ?¨í‚¤ì§€ë¥??´ìš©??Json Parsing
          //  var resultData = JObject.Parse(result.Json)["persona"]; 
            
          //  Debug.Log(result);
            
           // resutlData ?œíšŒ?˜ë©´??ê°ê°???•ë³´ë¥?ê°€?¸ì˜´
             //List<string> personas = new List<string>();
            
            Debug.Log("Post ?„ë£Œ:"+ result.Json);
             // foreach (JProperty property in resultData)
             // {
             //        
             // }
             
            
        };

        onSucceed += updateMPerceiveInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.POST, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
}
}

