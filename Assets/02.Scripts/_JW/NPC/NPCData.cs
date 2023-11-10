// Object of this class will hold the data
// And then this object will be converted to JSON
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class NPCData : MonoBehaviour
{
    [SerializeField]
    private TextAsset PerceiveJSONFile;
    public List<PerceivedInfo> NPC_Perceive = new List<PerceivedInfo>();
    private Perceive existingInfo;
    public List<NPC> NPC;
    private int NPC_Index;
    void Start()
    {
        existingInfo = JsonConvert.DeserializeObject<Perceive>(PerceiveJSONFile.text);
    }

    public void ReadJsonFile()
    {
        if(existingInfo.perceived_info.Count <= 0) // no data files
        {
            Debug.Log("Json file crashed");
        }
        else // data file exists
        {
            // Find the index of the existing PerceivedInfo with the same persona
            int existingIndex = existingInfo.perceived_info.FindIndex(info => info.persona == gameObject.name);
            if (existingIndex != -1)
            {
                // Persona exists, update curr_address
                existingInfo.perceived_info[existingIndex].curr_address = NPC[NPC_Index]._location.ToString();
            }
        }



        /*
        else
        {
            // Persona doesn't exist, add a new PerceivedInfo
            _perceivedInfo = new PerceivedInfo
            {
                persona = gameObject.name,
                curr_address = _location.ToString(),
                perceived_tiles = new List<PerceivedTile>(),
            };

            existingInfo.perceived_info.Add(_perceivedInfo);
        }*
        // Persona doesn't exist, add a new PerceivedInfo
        _perceivedInfo = new PerceivedInfo
        {
            persona = gameObject.name,
            curr_address = _location.ToString(),
            perceived_tiles = new List<PerceivedTile>(),
        };*/

        //existingInfo.perceived_info.Add(_perceivedInfo);

        // Combine NPC's data with existing data
        //_perceive.perceived_info.AddRange();

        // Convert LunaData to JSON
        //string PerceiveData = JsonConvert.SerializeObject(_perceive, Formatting.Indented);

        // Save the JSON data to a file
        //string filePath = Application.dataPath + "/LunaPerceiveFile.json";
        //File.WriteAllText(filePath, PerceiveData);
        //Debug.Log("JSON written");


/*
        // Deserialize existing data from the JSON file
        existingInfo = JsonConvert.DeserializeObject<Perceive>(PerceiveJSONFile.text);

        // Find the index of the existing PerceivedInfo with the same persona
        int existingIndex = existingInfo.perceived_info.FindIndex(info => info.persona == gameObject.name);

        Debug.Log(existingInfo.perceived_info[existingIndex].persona);

        if (existingIndex != -1)
        {
            // Persona exists, update curr_address
            existingInfo.perceived_info[existingIndex].curr_address = _location.ToString();
        }
        else
        {
            // Persona doesn't exist, add a new PerceivedInfo
            _perceivedInfo = new PerceivedInfo
            {
                persona = gameObject.name,
                curr_address = _location.ToString(),
                perceived_tiles = new List<PerceivedTile>(),
            };

            existingInfo.perceived_info.Add(_perceivedInfo);
        }

        // Convert LunaData to JSON
        string PerceiveData = JsonConvert.SerializeObject(existingInfo, Formatting.Indented);

        // Save the JSON data to a file (you can specify the path)
        string filePath = Application.dataPath + "/LunaPerceiveFile.json";
        File.WriteAllText(filePath, PerceiveData);
        Debug.Log("JSON written");
*/
    }

    public void PostJsonToServer()
    {

    }

}
