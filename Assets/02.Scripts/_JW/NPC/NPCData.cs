using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using NPCServer;

public class NPCData : MonoBehaviour
{
    [SerializeField]
    private TextAsset PerceiveJSONFile;
    private Perceive existingInfo;
    public List<NPC> NPC;
    
    // Get Movement
    private int step;
    private string personaTag;
    private string value; 


    void Start()
    {
        existingInfo = JsonConvert.DeserializeObject<Perceive>(PerceiveJSONFile.text);
        StartCoroutine(InvokePerceive());
        step = 0;
    }

    private IEnumerator InvokePerceive()
    {
        while (true)
        {
            //조건 두개를 만족해야 함 1. 시간 제한 2. 이전 스텝의 GetMovementInfo를 받은 후에
            yield return new WaitForSeconds(3f);
            SaveJsonFile();
            GetMovement();
            step++;
        }
    }

      private void GetMovement()
    {
        StartCoroutine(NPCServerManager.Instance.GetMovementCoroutine(0));

        foreach(var movementInfo in NPCServerManager.Instance.CurrentMovementInfo)
        {
            foreach(var perceivedInfo in existingInfo.perceived_info)
            {
                int npcIndex = FindNPCIndex(perceivedInfo.persona);

                if(movementInfo.Name == perceivedInfo.persona) // same persona
                {
                    int index = movementInfo.ActAddress.IndexOf('>');

                    if (index != -1) // <persona> exist
                    {
                        value = movementInfo.ActAddress.Substring(index + 1);  // name

                        for(int i = 0; i < perceivedInfo.perceived_tiles.Count; i++)
                        {
                            if(perceivedInfo.perceived_tiles[i].@event[0] == value)
                            {
                                NPC[npcIndex].agent.isStopped = true;
                                Debug.Log("succeess!!!!!!!!!!!");

                            }
                        }


                    }
                    else // <persona> not exists
                    {
                        foreach(var npc in NPC)
                        {
                            if(npc._name == value)
                            {
                                
                            }
                        }
                    }
                }

            }    

            
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
            foreach (var perceivedInfo in existingInfo.perceived_info)
            {
                int npcIndex = FindNPCIndex(perceivedInfo.persona);

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
        
        //recall Perceive Post API
        StartCoroutine( NPCServerManager.Instance.PostPerceiveCoroutine(PerceiveData,step));

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
