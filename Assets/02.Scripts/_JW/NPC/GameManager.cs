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
    [SerializeField]
    private TextAsset PerceiveJSONFile;
    private Perceive existingInfo;
    public List<NPC> NPC;


    private string filePath;

    // Get Movement
    private int step;
   // public string simCode; 
    public string gameName; 
    public string NPCName; 

    // Chat 
    public DialogueManager dialogueManager;


    void Start()
    {
        filePath = "Assets/NPCPerceiveFile.json";
        LoadExistingInfo(filePath);
        //simCode는 게임 베이스, 지금 당장은 필요 없음. 
       // simCode = Database.Instance.simCode;
        gameName = Database.Instance.gameName; 
        step = 0;
        StartCoroutine(InvokePerceive());
        
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
        
        int lasttime = curr_time.Hour * 60 + curr_time.Minute;
        int pStep = 1;//이미 base에서 perceive가 0이 있음.
        while (true)
        {
            curr_time = Clock.Instance.GetCurrentTime();

            int minute = curr_time.Hour * 60 + curr_time.Minute; 
            
//            Debug.Log("0"+NPCServerManager.Instance.serverOpened);
            //server manage에서 서버가 안 열렸을때
            if (!NPCServerManager.Instance.serverOpened) { yield return new WaitForSeconds(1f); continue;}
            
           
            if (step == 0)
            {
                GetMovement(step);
                Debug.Log("1"+step);
                step++;
                lasttime = minute;
                NPCServerManager.Instance.perceived = false;
                yield return new WaitForSeconds(1f); 
                continue;
            }
            
            //Debug.Log("2 "+step);
            if (minute - lasttime >= stepTime || pStep > step)
            {
                //step이 올라가는 타이밍이 왔을 때 딱 한번만 호출
                if (pStep == step && NPCServerManager.Instance.getReaction)
                {
                    lasttime = minute;
                    Debug.Log("perceive"+step);
                    SaveJsonFile(); //perceive 
                    NPCServerManager.Instance.getReaction = false;
                    pStep++;
                }

                //server가 Perceive 파일을 받았을 때 
                if (NPCServerManager.Instance.perceived&false)
                {
                    Debug.Log("Get Step "+step);
                    GetMovement(step);
                    step++;
                    NPCServerManager.Instance.perceived = false;

                }
            }
            yield return new WaitForSeconds(1f); //10초를 18로 나눴을 때 0.55556이라 0.5씩 반복하면 1분 단위 게임 시간을 모두 체크함.
           
            
            
        }
    }

    private void GetMovement(int stepNumber)
    {
        // get movement api 
        StartCoroutine(NPCServerManager.Instance.GetMovementCoroutine(gameName,stepNumber));

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
        StartCoroutine(NPCServerManager.Instance.PostPerceiveCoroutine(PerceiveData,gameName,step));

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