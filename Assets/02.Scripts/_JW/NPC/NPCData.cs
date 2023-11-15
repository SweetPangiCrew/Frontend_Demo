using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class NPCData : MonoBehaviour
{
    [SerializeField]
    private TextAsset PerceiveJSONFile;
    private Perceive existingInfo;
    public List<NPC> NPC;

    void Start()
    {
        existingInfo = JsonConvert.DeserializeObject<Perceive>(PerceiveJSONFile.text);
        StartCoroutine(InvokePerceive());
    }

    private IEnumerator InvokePerceive()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            SaveJsonFile();
        }
    }

    public void SaveJsonFile()
    {
        if (existingInfo.perceived_info.Count <= 0)
        {
            Debug.Log("Json file crashed");
        }
        else
        {
            foreach (var info in existingInfo.perceived_info)
            {
                int npcIndex = FindNPCIndex(info.persona);

                if (npcIndex != -1)
                {
                    /*  UPDATE curr_address */ 
                    existingInfo.perceived_info[npcIndex].curr_address = NPC[npcIndex]._locationName;

                    for (int k = 0; k < NPC[npcIndex]._detectedObject.Count; k++)
                    {
                        if (NPC[npcIndex]._detectedObject[k].CompareTag("NPC"))
                        {
                            while (existingInfo.perceived_info[npcIndex].perceived_tiles.Count <= k)
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles.Add(new PerceivedTile());
                            }

                            /*  UPDATE perceived_tiles.dist */ 
                            existingInfo.perceived_info[npcIndex].perceived_tiles[k].dist =
                                Vector2.Distance(NPC[npcIndex].transform.position, NPC[npcIndex]._detectedObject[k].transform.position);

                            /*  UPDATE perceived_tiles.@event */ 
                            while (existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event == null)
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event = new List<string>();
                            }

                            while (existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event.Count <= 3)
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event.Add(null);
                            }

                            existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event[0] =
                                NPC[npcIndex]._detectedObject[k].gameObject.name.ToString();

                            // Set the rest of @event to null
                            for (int j = 1; j < 4; j++)
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event[j] = "null";
                            }
                        }
                    }
                }
            }
        }

        // Convert LunaData to JSON
        string PerceiveData = JsonConvert.SerializeObject(existingInfo, Formatting.Indented);

        // Save the JSON data to a file
        string filePath = Application.dataPath + "/LunaPerceiveFile.json";
        File.WriteAllText(filePath, PerceiveData);
    }

    private int FindNPCIndex(string persona)
    {
        for (int i = 0; i < NPC.Count; i++)
        {
            if (NPC[i].name == persona)
            {
                return i;
            }
        }
        return -1; // Return -1 if not found
    }
}
