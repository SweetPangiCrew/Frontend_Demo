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
    public bool isUsingLocalServer;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("User 정보 불러옴");
            uuid =  PlayerPrefs.GetString("uuid", "a45c234b-f6f8-4ba8-9fa3-0dac1ce521de");
            username = PlayerPrefs.GetString("username", "NULL");

            if (isUsingLocalServer)
            {
                uuid = "5ba7a43d-ce45-4357-b34e-156b2c70fa58";
                username = "로컬이";
            }
            
        }
        else
            Destroy(gameObject);
    }
    
    
    
}
