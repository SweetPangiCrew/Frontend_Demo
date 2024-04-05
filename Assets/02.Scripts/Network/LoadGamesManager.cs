using System;

using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NPCServer;
using UnityEngine.SceneManagement;


public class LoadGamesManager : HttpServerBase
{
    public static LoadGamesManager Instance { get; private set; }
    
    [SerializeField]
    private  Dictionary<string,int> existingGameInfo;
    
    public Dictionary<string,int> ExistingGameInfo { get => existingGameInfo;
        set
        {
            existingGameInfo = value;
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
        StartCoroutine(GetExistingGamesCoroutine());
    }

    private void clickBtnloadGame(string gamekey,int step)
    {
  
        
        Database.Instance.gameName = gamekey;
        Database.Instance.StartStep = step;
        
        NPCServerManager.Instance.gameStart();
        SceneManager.LoadScene("MainTest");
        
    }

    private void loadGameButtons()
    {
        GameObject prefabToLoad = Resources.Load<GameObject>("gameBtn");

        if (prefabToLoad != null)
        {

            foreach (var game in existingGameInfo)
            {
                // 불러온 프리팹을 동적으로 생성
                GameObject prefab = Instantiate(prefabToLoad,transform);
                UnityEngine.UI.Button btn = prefab.GetComponent<UnityEngine.UI.Button>();
                prefab.transform.GetComponentInChildren<TextMeshProUGUI>().text = game.Key;
               
                btn.onClick.AddListener(()=>clickBtnloadGame(game.Key,game.Value));
              

            }
           
        }
        else
        {
            Debug.LogError("Failed to load the prefab from Resources.");
        }
    }

    public IEnumerator GetExistingGamesCoroutine()
    {
        yield return GetExistingGames();
    }
    
     public Coroutine GetExistingGames(
        Action<Result> onSucceed = null, Action<Result> onFailed = null, Action<Result> onNetworkFailed = null)
    {
        string url = GameURL.NPCServer.Server_URL + GameURL.NPCServer.getExistingGames; 

        // Newtonsoft.Json
        JObject jobj = new JObject();
        
        Action<Result> updateInfoAction = (result) =>
        {
            
            var resultData = JObject.Parse(result.Json)["games"]; 
            
            Dictionary<string,int> game_names = new Dictionary<string,int>();
            
            foreach (JProperty property in resultData)
            { 
                game_names[property.Name] = int.Parse(property.Value["step"].ToString());
             
            }

            existingGameInfo = game_names;
            loadGameButtons();
        };

        onSucceed += updateInfoAction;
        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
    
}
