using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database Instance { get; private set; }
    
    public string simCode; 
    public string gameName;
    public int StartStep = 0;
    public string uuid;
    public string username;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("User 정보 불러옴");
            Database.Instance.uuid =  PlayerPrefs.GetString("uuid", "NULL");
            Database.Instance.username = PlayerPrefs.GetString("username", "NULL");
        }
        else
            Destroy(gameObject);
    }
    
    
    
}
