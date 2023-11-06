using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

namespace NPCServer{
public class NPCServerManager : HttpServerBase
{
    public static NPCServerManager Instance { get; private set; }

    [SerializeField]
    private List<Persona> currentMovementInfo;


    #region 프로퍼티
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

    public Coroutine GetMovement(string sim_names, int step, 
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        // 로그인 URL을 조합
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getNPCMovement;

        // Newtonsoft.Json 패키지를 이용한 Json생성
        JObject jobj = new JObject();
        // jobj["email"] = email;
        // jobj["nickname"] = nickName;
      

        // 성공했을때 콜백
        // 새로운 유저 정보를 세팅함 로그인 요청을했고 성공했다면 항상 업데이트 되도록 할려고 이쪽에 정의함
        Action<Result> updateMovementInfoAction = (result) =>
        {
            // Newtonsoft.Json 패키지를 이용한 Json Parsing
            var resultData = JObject.Parse(result.Json)["persona"]; 

            //resutlData 순회하면서 각각의 정보를 가져옴
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
                    chats.Add(property.Value["chat"].ToObject<List<string>>());
            }


            for(int i=0; i< personas.Count; i++)
            {
                Persona newMovementInfo = new Persona(personas[i], act_address[i], pronunciatio[i], description[i], chats[i]);
                CurrentMovementInfo.Add(newMovementInfo);
            }

        };

        onSucceed += updateMovementInfoAction;

        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
}
}