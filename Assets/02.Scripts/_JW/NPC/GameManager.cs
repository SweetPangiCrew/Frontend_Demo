using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using NPCServer;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private TextAsset PerceiveJSONFile;
    private Perceive existingInfo;
    public List<NPC> NPC;
    
    // Get Movement
    private int step;
    public string NPCName; 

    // Chat 
    public TextMeshProUGUI IsabellaDialogueText;
    public TextMeshProUGUI MariaDialogueText;
    public int IsabellaDialogueIndex = 1;
    public int MariaDialogueIndex = 1;
    public DialogueManager dialogueManager;
    public GameObject IsabellaDialoguePanel;
    public GameObject MariaDialoguePanel;
    
    private bool isStartConversation = true;
    void Start()
    {
        existingInfo = JsonConvert.DeserializeObject<Perceive>(PerceiveJSONFile.text);
        StartCoroutine(InvokePerceive());
        step = 0;
        IsabellaDialoguePanel.SetActive(false);
        MariaDialoguePanel.SetActive(false);
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
        // get movement api 
        StartCoroutine(NPCServerManager.Instance.GetMovementCoroutine(0));

        // read get movement
        foreach(var movementInfo in NPCServerManager.Instance.CurrentMovementInfo)
        {
            foreach(var perceivedInfo in existingInfo.perceived_info)
            {
                int npcIndex = FindNPCIndex(perceivedInfo.persona);

                if(movementInfo.Name == perceivedInfo.persona) // same persona
                {
                    int index = movementInfo.ActAddress.IndexOf('>');
                    
                    // if <persona> tag exists -> start conversation
                    if (index != -1) // <persona> exists
                    {
                        NPCName = movementInfo.ActAddress.Substring(index + 2);  
                        //dialogueManager.GenerateData(NPCName, movementInfo.Chat);
                        
                        // meets NPC -> Stop
                        for(int i = 0; i < perceivedInfo.perceived_tiles.Count; i++)
                        {
                            if(perceivedInfo.perceived_tiles[i].@event[0] == NPCName)
                            {
                                for(int j = 0; j < NPC.Count; j++)
                                {
                                    if(NPC[j].gameObject.name.ToString() == NPCName)
                                    {
                                        NPC[npcIndex].agent.isStopped = true;
                                        NPC[j].agent.isStopped = true;                            
                                    }
                                }
                            }
                        }


                        // start conversation with <persona> NPC Name            

                        /*
                        if (NPCName == "Isabella Rodriguez")
                        {
                            IsabellaDialoguePanel.SetActive(true);
                            MariaDialoguePanel.SetActive(false);
                            Conversation(NPCName, IsabellaDialogueText, ref IsabellaDialogueIndex);
                        }
                        else if (NPCName == "Maria Lopez")
                        {
                            IsabellaDialoguePanel.SetActive(false);
                            MariaDialoguePanel.SetActive(true);
                            Conversation(NPCName, MariaDialogueText, ref MariaDialogueIndex);
                        }*/

                        ChatSystem.isChatting = 1;

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

     void Conversation(string NPCname, TextMeshProUGUI dialogueText, ref int dialogueIndex)
    {
        dialogueText.text = dialogueManager.GetDialogue(NPCname, dialogueIndex);

        if (dialogueText.text == null)
        {
            dialogueIndex = 1;
            isStartConversation = true;
            IsabellaDialoguePanel.SetActive(false);
            MariaDialoguePanel.SetActive(false);
        }
        else
        {
            dialogueIndex++ ;
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
        string filePath = "Assets/01.Scenes/_JW/NPCPerceive/NPCPerceiveSaveFile" + step + ".json";
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
