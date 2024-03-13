using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class JSONWriter : MonoBehaviour
{
    private NPC luna;
    public void SaveToJson(){
        GameManager data = new GameManager();
        data.name = luna.name;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.dataPath+"/LunaDataFile.json",json);
        Debug.Log("Succeed!");
    }
}
