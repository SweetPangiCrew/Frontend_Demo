using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

public class JSONWriter : MonoBehaviour
{
    private NPC luna;
    public void SaveToJson(){
        NPCData data = new NPCData();
        data.name = luna._name;

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.dataPath+"/LunaDataFile.json",json);
        Debug.Log("Succeed!");
    }
}
