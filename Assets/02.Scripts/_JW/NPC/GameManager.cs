using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using NPCServer;
using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using TMPro;

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
    public List<Persona> personaList = new List<Persona>();
    private string filePath;

    // timer
    private DateTime curr_time;
    private int stepTime = 18; // 게임 시간으로 18분 마다 스텝이 업데이트 됨.

    // Get Movement
    private int step;
    // Get Movement from local file, true : only Test Mode available
    public bool isUsingMovementLocalFile = true;
    public string gameName; 
    public string NPCName; 
    
    // Chat 
    public ChatManager chatManager;
    private int npcIndex;
    private int speakerIndex;
    private HashSet<string> conversationPairs = new HashSet<string>();
    
    // Location
    public List<Transform> location;

    public bool isTest = false;
    void Start()
    {
        filePath = "Assets/NPCPerceiveFile.json";
        LoadExistingInfo(filePath);
        
        if (gameName != "")
        {
            gameName = Database.Instance.gameName;
            step = Database.Instance.StartStep; // 초기화는 0으로, game load한거라면 다름.
        }
        else
        {
            gameName = "game1";
            isTest = true;
        }
       
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
        int pStep = 1; //이미 base에서 perceive가 0이 있음.
        while (true)
        {
            curr_time = Clock.Instance.GetCurrentTime();

            int minute = curr_time.Hour * 60 + curr_time.Minute; 
            
            //server manage에서 서버가 안 열렸을때
            if (!NPCServerManager.Instance.serverOpened & !isTest) { yield return new WaitForSeconds(1f); continue;}
            
            if (step == 0)
            {
                GetMovement(step);
                step++;
                lasttime = minute;
                NPCServerManager.Instance.perceived = false;
                yield return new WaitForSeconds(1f); 
                continue;
            }
            
            if (minute - lasttime >= stepTime || pStep > step)
            {
                //step이 올라가는 타이밍이 왔을 때 딱 한번만 호출
                if (pStep == step && NPCServerManager.Instance.getReaction)
                {
                    lasttime = minute;
                    SaveJsonFile(); //perceive 
                    NPCServerManager.Instance.getReaction = false;
                    pStep++;
                }

                //server가 Perceive 파일을 받았을 때 
                if (NPCServerManager.Instance.perceived)
                {
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
        if (isUsingMovementLocalFile)
        { 
            string json = File.ReadAllText("Assets/Resources/NPCMovementFile.json");
            
            var resultData = JObject.Parse(json)["persona"]; 
            
            List<string> personas = new List<string>();
            List<string> act_address   = new List<string>();
            List<string> pronunciatio  = new List<string>();
            List<string> description  = new List<string>();
            List<List<List<string>>> chats= new List<List<List<string>>>();
            
            foreach (JProperty property in resultData)
            {
                List<List<string>> chat = new List<List<string>>();
              
                personas.Add(property.Name);
                act_address.Add(property.Value["act_address"].ToString());
                pronunciatio.Add(property.Value["pronunciatio"].ToString());
                description.Add(property.Value["description"].ToString());
                    
                foreach (var chatlist in property.Value["chat"])
                {
                    var chatEntry = chatlist.Select(item => item.ToString()).ToList();
                    chat.Add(chatEntry);
                    chats.Add(chat);
                }
            }

            for(int i=0; i< personas.Count; i++)
            {
                Persona newMovementInfo = new Persona(personas[i], act_address[i], pronunciatio[i], description[i], chats[i]);
                personaList.Add(newMovementInfo);
            }
        }
        else
        {
            StartCoroutine(NPCServerManager.Instance.GetMovementCoroutine(gameName, stepNumber));
            personaList = NPCServerManager.Instance.CurrentMovementInfo;
        }

        if (personaList == null || NPC == null)
        {
            Debug.LogError("personaList or NPC list is null.");
            return;
        }
            
        foreach (var perceivedInfo in existingInfo.perceived_info)
        {            
            npcIndex = FindNPCIndex(perceivedInfo.persona);
            
            if(personaList[npcIndex].Name == NPC[npcIndex].name)
            {
                /* --- ACT ADDRESS --- */
                (string tag, string content) = ExtractTagAndContent(personaList[npcIndex].ActAddress);
                
                if(tag == "persona")
                {
                    NPCName = content;

                    for (int i = 0; i < perceivedInfo.perceived_tiles.Count; i++)
                    {
                        if (perceivedInfo.perceived_tiles[i].@event[0] == NPCName) 
                        {
                            int otherNpcIndex = FindNPCIndex(NPCName);
                            NPC[npcIndex].navMeshAgent.isStopped = true;
                                        
                            string otherNPCName = perceivedInfo.perceived_tiles[i].@event[0]; 
                            string firstNPCName = NPC[npcIndex].gameObject.name.CompareTo(otherNPCName) < 0 ? NPC[npcIndex].gameObject.name : otherNPCName;
                            string secondNPCName = NPC[npcIndex].gameObject.name.CompareTo(otherNPCName) < 0 ? otherNPCName : NPC[npcIndex].gameObject.name;
                                        
                            string conversationPair = $"{firstNPCName}-{secondNPCName}";

                            if (conversationPairs.Contains(conversationPair))
                            {
                                continue; 
                            }
                            else
                            {                                
                                var chatList = personaList[npcIndex].Chat;
                                        
                                for (int k = 0; k < chatList.Count; k++)
                                {
                                    var chat = chatList[k];
                                    string speaker = chat[0].ToString();
                                    string dialogue = chat[1].ToString();

                                    Debug.Log("발화자 : " + speaker + personaList[npcIndex].Name);
                                    Debug.Log("내용 : "  +dialogue);
                                    
                                    
                                    if (chatManager.dialogues.Count < npcIndex+1)
                                    {
                                        for (int j = 0; j < npcIndex+1; j++)
                                        {   
                                            chatManager.dialogues.Add(new DialoguesList());
                                        }
                                    }

                                    chatManager.dialogues[npcIndex].dialogues.Add(new DialogueData
                                    {
                                        dialogue = dialogue,
                                        name = speaker,
                                        speakerIndex = k % 2
                                    });
                                      

                                    NPC[npcIndex].IconBubble.SetActive(true);
                                    NPC[otherNpcIndex].IconBubble.SetActive(true);
                                    chatManager.isChatting = true;
                                }

                                chatManager.npcIndex = npcIndex;
                                chatManager.isFirst = true;
                            }

                            conversationPairs.Add(conversationPair);
                        }
                    }
                }

                else if(tag == "location")
                {
                    GetLocation(content, npcIndex);
                }
                    
                /* --- PRONUNCIATIO --- */
                NPC[npcIndex].IconBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += personaList[npcIndex].Pronunciatio;  

                /* --- DESCRIPTION --- */
                NPC[npcIndex].DescriptionBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = personaList[npcIndex].Description;
            }
        }                
    } 

    private void GetLocation(string content, int npcIndex)
    {
        string[] parts = content.Split(':');
        string nextLocation = parts[2] != "null" ? parts[2] : parts[1];
        AddLocation(nextLocation, npcIndex);
    }       

    private void AddLocation(string nextLocation, int npcIndex)
    {
        foreach(var nl in location)
        {
            if(nl.gameObject.name == nextLocation)
            {
                NPC[npcIndex].AddWaypoint(nl);      
                break; 
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
                npcIndex = FindNPCIndex(perceivedInfo.persona);
                
                if (npcIndex != -1)
                {
                    existingInfo.perceived_info[npcIndex].curr_address = "home:home:home:home";

                    for (int k = 0; k < NPC[npcIndex].detectedObjects.Count; k++)
                    {
                        if (NPC[npcIndex].detectedObjects[k].CompareTag("NPC"))
                        {
                            while(existingInfo.perceived_info[npcIndex].perceived_tiles.Count < k + 1)
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles.Add(new PerceivedTile());
                            }

                            existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event = new string[4]; 

                            try
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].dist =
                                    Vector2.Distance(NPC[npcIndex].transform.position, NPC[npcIndex].detectedObjects[k].transform.position);

                            }
                            catch (Exception  e)
                            {
                                Console.WriteLine(e);
                                throw;
                            }
                            
                            existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event[0] = NPC[npcIndex].detectedObjects[k].gameObject.name;

                            for (int j = 1; j < 4; j++)
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event[j] = null;
                            }
                        }
                        else
                        {
                            existingInfo.perceived_info[npcIndex].perceived_tiles = null;
                        }
                    }
                }
            }
        }

        string PerceiveData = JsonConvert.SerializeObject(existingInfo, Formatting.Indented);
        File.WriteAllText(filePath, PerceiveData);
       
        StartCoroutine(NPCServerManager.Instance.PostPerceiveCoroutine(PerceiveData,gameName,step));
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

    private (string tag, string content) ExtractTagAndContent(string input)
    {
        string pattern = @"<(persona|location)>\s*([^<]*)"; 

        Match match = Regex.Match(input, pattern); 

        if (match.Success) 
        {
            string tag = match.Groups[1].Value.Trim(); 
            string content = match.Groups[2].Value.Trim(); 
            return (tag, content);
        }
        else
        {
            return (null, null); 
        }
    }  
}
