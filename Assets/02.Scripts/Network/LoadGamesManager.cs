using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using NPCServer;
using System.IO;
using System.Linq;
using TMPro;


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
        GameURL.NPCServer.Server_URL = GameURL.NPCServer.Local_URL;
        StartCoroutine(GetExistingGamesCoroutine());
    }

    private void clickBtnloadGame()
    {
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
                GameObject prefab = Instantiate(prefabToLoad);

                prefab.transform.parent = transform;

                Button btn = prefab.GetComponent<Button>();
                prefab.transform.GetComponentInChildren<TextMeshPro>().text = game.Key;
                Database.Instance.simCode = game.Key;
                Database.Instance.StartStep = game.Value;
                
              //  btn.onClick.AddListener(clickBtnloadGame);
               


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
        
        Action<Result> updateGameNamesInfoAction = (result) =>
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

        onSucceed += updateGameNamesInfoAction;
        return StartCoroutine(SendRequestCor(url, SendType.GET, jobj, onSucceed, onFailed, onNetworkFailed));
    }
    
    
}
