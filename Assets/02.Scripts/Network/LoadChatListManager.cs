using System;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NPCServer;
using System.Linq;
using UnityEngine.Serialization;

public class LoadChatListManager :  HttpServerBase
{
    public static LoadChatListManager Instance { get; private set; }
    
    [FormerlySerializedAs("Info")] [SerializeField]
    private Dictionary<string,List<List<List<string>>>> existingInfo;

    public ChatManager chatManager;
    public GameObject chatPanel,chatListPanel;
    public Dictionary<string,List<List<List<string>>>> ExistingGameInfo { get => existingInfo;
        set
        {
            existingInfo = value;
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
      

        // ClickButtonLoadChatList();
    }

    private void OnEnable()
    {
        ClickButtonLoadChatList();
    }

    private void OnDisable()
    {
        // 자식 GameObject를 모두 파괴합니다.
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
 
    }

    public void ClickButtonLoadChatList()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        
        StartCoroutine(GetExistingChatListsCoroutine());
    }
    
    private void loadChatListButtons()
    {
        GameObject prefabToLoad = Resources.Load<GameObject>("DateChatList");
        GameObject prefabChat = Resources.Load<GameObject>("PN_Chats");
        if (prefabToLoad != null)
        {

            foreach (var chatList in existingInfo)
            {
                // 불러온 프리팹을 동적으로 생성
                GameObject prefab = Instantiate(prefabToLoad,transform);
              
                prefab.GetComponentInChildren<TextMeshProUGUI>().text = chatList.Key;
                
               foreach (var chat in chatList.Value)
               {     GameObject chatPrefab = Instantiate(prefabChat,prefab.transform);
                   try
                   {
                       chatPrefab.transform.Find("Names").GetComponentInChildren<TextMeshProUGUI>().text = chat[0][0]+", "+chat[1][0];
                       chatPrefab.transform.Find("Explain").GetComponentInChildren<TextMeshProUGUI>().text = chat[0][1];
                       UnityEngine.UI.Button btn = chatPrefab.GetComponent<UnityEngine.UI.Button>();
                       btn.onClick.AddListener(()=>clickBtnloadChat(chat));
                       
                   }
                   catch (Exception e)
                   {
                       Destroy(chatPrefab);
                       Console.WriteLine(e);
                    
                   }
                  
               }


            }
           
        }
        else
        {
            Debug.LogError("Failed to load the prefab from Resources.");
        }
    }

    private void clickBtnloadChat(List<List<string>> chat)
    {
        chatManager.isChattingHistory = true;
        chatManager.showDialogue(chat);
        chatPanel.SetActive(true);
        chatListPanel.SetActive(false);
    }
    
   
    public IEnumerator GetExistingChatListsCoroutine()
    {
        yield return GetExistingChatLists();
    }
    
     public Coroutine GetExistingChatLists(
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getChatLists + Database.Instance.gameName+"/"+Database.Instance.uuid; 

        // Newtonsoft.Json
        JObject jobj = new JObject();
        
        Action<Result> updateInfoAction = (result) =>
        {
            
            var resultData = JObject.Parse(result.Json)["chat_list"]; 
            
            Dictionary<string,List<List<List<string>>>> Chat_list = new Dictionary<string,List<List<List<string>>>>();
      
            foreach (JToken property in resultData)
            {
                
                string date = GetStringBeforeSecondComma( property[0]["date"].ToString());

                if (!Chat_list.ContainsKey(date))
                {
                    Chat_list.Add(date,new List<List<List<string>>>());
                }
                List<List<string>> chats = new List<List<string>>();
                foreach (var chatlist in  property[0]["chat"])
                {
                    var chatEntry = chatlist.Select(item => item.ToString()).ToList();
                    chats.Add(chatEntry);
                }
                

                Chat_list[date].Add(chats);
            }

            existingInfo = Chat_list;
            loadChatListButtons();
        };

        onSucceed += updateInfoAction;
        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
     
     string GetStringBeforeSecondComma(string input)
     {
         int firstCommaIndex = input.IndexOf(',');
         if (firstCommaIndex != -1)
         {
             int secondCommaIndex = input.IndexOf(',', firstCommaIndex + 1);
             if (secondCommaIndex != -1)
             {
                 return input.Substring(0, secondCommaIndex);
             }
         }
         return input; // 두 번째 쉼표가 없는 경우 전체 문자열을 반환
     }
    
}
