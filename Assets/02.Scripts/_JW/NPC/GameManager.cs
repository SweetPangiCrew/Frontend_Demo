using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using NPCServer;
using TMPro;
using System;
using Panda.Examples.PlayTag;
using System.Linq;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    [SerializeField]
    private TextAsset PerceiveJSONFile;
    private Perceive existingInfo;
    public List<NPC> NPC;


    private string filePath;

    // Get Movement
    private int step;
    public string simCode; 
    public string gameName; 
    public string NPCName; 

    // Chat 
    public DialogueManager dialogueManager;


    void Start()
    {
        filePath = "Assets/NPCPerceiveFile.json";
        LoadExistingInfo(filePath);

        StartCoroutine(InvokePerceive());
        step = 0;;
    }

    private void LoadExistingInfo(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            existingInfo = JsonConvert.DeserializeObject<Perceive>(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load existingInfo from {filePath}. Error: {e.Message}");
           
        }
    }

    private IEnumerator InvokePerceive()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);
            SaveJsonFile();
            GetMovement();
            step++;
        }
    }

    private void GetMovement()
    {
        // get movement api 
        StartCoroutine(NPCServerManager.Instance.GetMovementCoroutine(0));

        // read get movement
        foreach(var movementInfo in NPCServerManager.Instance.CurrentMovementInfo)
        {
            
            foreach (var perceivedInfo in existingInfo.perceived_info)
            {
                int npcIndex = FindNPCIndex(perceivedInfo.persona);

                if(movementInfo.Name == perceivedInfo.persona) // same persona
                {
                    
                    int index = movementInfo.ActAddress.IndexOf('>');
                    
                    // if <persona> tag exists -> start conversation
                    if (index != -1) // <persona> existss
                    {
                        NPCName = movementInfo.ActAddress.Substring(index + 2);


                        // meets NPC -> Stop
                        for (int i = 0; i < perceivedInfo.perceived_tiles.Count; i++)
                        {
                            if(perceivedInfo.perceived_tiles[i].@event[0] == NPCName)
                            {
                                for(int j = 0; j < NPC.Count; j++)
                                {
                                    if(NPC[j].gameObject.name.ToString() == NPCName)
                                    {
                                        NPC[npcIndex].agent.isStopped = true;
                                        NPC[j].agent.isStopped = true;

                                        if (!dialogueManager.isChatting)
                                        {
                                            for (int k = 0; k < movementInfo.Chat.Count; k++)
                                            {
                                                var chat = movementInfo.Chat[k];
                                                string speaker = chat[0].ToString();
                                                string dialogue = chat[1].ToString();

                                                if (dialogueManager.dialogues.Count <= k)
                                                {

                                                    dialogueManager.dialogues.Add(new DialogueData { dialogue = dialogue });
                                                }


                                                //yield return new WaitUntil(()=>dialogueManager.UpdateDialogue());


                                            }
                                        }
                              


                                    }
                                }
                            }
                        }
                    }
                    else // <persona> not exists
                    {
                        foreach(var npc in NPC)
                        {
                            if(npc._name == NPCName)
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
                            while(existingInfo.perceived_info[npcIndex].perceived_tiles.Count < k)
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
        StartCoroutine(NPCServerManager.Instance.PostPerceiveCoroutine(PerceiveData,step));

        // Save the JSON data to a file
        //string filePath = "Assets/NPCPerceiveFile.json";
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