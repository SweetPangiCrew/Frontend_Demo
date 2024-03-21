using System;

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NPCServer;
using UnityEngine.SceneManagement;


public class ReligiousIndexNetworkManager : HttpServerBase
{
    public static ReligiousIndexNetworkManager Instance { get; private set; }
    
    [SerializeField]
    private  Dictionary<string,int> rIndexInfo = new Dictionary<string, int>();
    
    //update된 info와 load한 Info가 저장됨
    //load 전에 update시 업데이트한 NPC 정보만 저장되어있을 수 있음
    public Dictionary<string,int> RIndexInfo { get => rIndexInfo;
        set
        {
            rIndexInfo = value;
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
        if (GameManager.Instance.usingLocalServer)
        {
            GameURL.NPCServer.Server_URL = GameURL.NPCServer.Local_URL;
        }
        
        //종교 친화 지수 불러오기 API 호출
        StartCoroutine(GetRIndexCoroutine());

        //종교 친화 지수 업데이트 API 호출
        Dictionary<string, int> updateInfo = new Dictionary<string, int>();
        updateInfo["이자식"] = 1; // 이자식의 종교친화 지수를 1 업데이트함.
        
        StartCoroutine(UpdateRIndexCoroutine(updateInfo));
    }
    

    public IEnumerator GetRIndexCoroutine()
    {
        yield return GetRIndex();
    }
    
     public Coroutine GetRIndex(
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getRIndex + Database.Instance.gameName; 

        // Newtonsoft.Json
        JObject jobj = new JObject();
        
        Action<Result> updateInfoAction = (result) =>
        {
            
            var resultData = JObject.Parse(result.Json)["religious_index"]; 
            
            Dictionary<string,int> info = new Dictionary<string,int>();
            
            foreach (JProperty property in resultData)
            { 
                info[property.Name] = int.Parse(property.Value.ToString());
             
            }

            rIndexInfo = info;
           
        };

        onSucceed += updateInfoAction;
        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
     public IEnumerator UpdateRIndexCoroutine(Dictionary<string,int> updateInfo)
     {
         yield return UpdateRIndex(updateInfo);
     }
    
     public Coroutine UpdateRIndex( Dictionary<string,int> updateInfo,
         Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
     {
         string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.updateRIndex + Database.Instance.gameName +"/"; 

         // Newtonsoft.Json
         JObject jobj = new JObject();

         jobj["update_religious_index"] = JObject.FromObject(updateInfo);;
        
         Action<Result> updateInfoAction = (result) =>
         {
            
             var resultData = JObject.Parse(result.Json)["religious_index"]; 
            
             
            //update된것만 rIndexInfo에 적용
             foreach (JProperty property in resultData)
             { 
                 
                 rIndexInfo[property.Name] = int.Parse(property.Value.ToString());
             
             }
             
           
         };

         onSucceed += updateInfoAction;
         return StartCoroutine(SendRequestCor(url, SendType.PUT, jobj, onSucceed, onFailed, onNetworkFailed));
     }
}
