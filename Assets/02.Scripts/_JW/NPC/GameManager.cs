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
    
    //timer
    private DateTime curr_time;
    private int stepTime = 18; // 게임 시간으로 18분 마다 스텝이 업데이트 됨.

    // Get Movement
    private int step;
    //Get Movement from local file, true : only Test Mode available
    public bool isUsingMovementLocalFile = true;
   // public string simCode; 
    public string gameName; 
    public string NPCName; 
    
    // Chat 
    public ChatManager chatManager;
    private int speakerIndex;
    
    // Location
    public List<Transform> location;

    public bool isTest = false;
    void Start()
    {
        filePath = "Assets/NPCPerceiveFile.json";
        LoadExistingInfo(filePath);
        
        //simCode는 게임 베이스, 지금 당장은 필요 없음. 
       // simCode = Database.Instance.simCode;
       if (gameName != "")
       {
           gameName = Database.Instance.gameName;
           step = Database.Instance.StartStep; // 초기화는 0으로, game load한거라면 다름.
       }
       else
       {
           //로컬 서버 관련 코드
          // GameURL.NPCServer.Server_URL = GameURL.NPCServer.Local_URL;
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
        int pStep = 1;//이미 base에서 perceive가 0이 있음.
        while (true)
        {
            curr_time = Clock.Instance.GetCurrentTime();

            int minute = curr_time.Hour * 60 + curr_time.Minute; 
            
//            Debug.Log("0"+NPCServerManager.Instance.serverOpened);
            //server manage에서 서버가 안 열렸을때
            if (!NPCServerManager.Instance.serverOpened & !isTest) { yield return new WaitForSeconds(1f); continue;}
            
           
            if (step == 0)
            {
                GetMovement(step);
                //Debug.Log("1"+step);
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
                    //Debug.Log("perceive"+step);
                    SaveJsonFile(); //perceive 
                    NPCServerManager.Instance.getReaction = false;
                    pStep++;
                }

                //server가 Perceive 파일을 받았을 때 
                if (NPCServerManager.Instance.perceived)
                {
                   //Debug.Log("Get Step "+step);
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
        /* Use Local Movement File */
        if (isUsingMovementLocalFile)
        { 
            string json = File.ReadAllText("Assets/Resources/NPCMovementFile.json");
            
            var resultData = JObject.Parse(json)["persona"]; 
            
            List<string> personas = new List<string>();
            List<string> act_address   = new List<string>();
            List<string> pronunciatio  = new List<string>();
            List<string> description  = new List<string>();
            List<List<string>> chats = new List<List<string>>();
            
            foreach (JProperty property in resultData)
            {
                personas.Add(property.Name);
                act_address.Add(property.Value["act_address"].ToString());
                pronunciatio.Add(property.Value["pronunciatio"].ToString());
                description.Add(property.Value["description"].ToString());
                //Debug.Log(property.Value["chat"].ToString());
                    
                foreach (var chatlist in property.Value["chat"])
                {
                    var chatEntry = chatlist.Select(item => item.ToString()).ToList();
                    chats.Add(chatEntry);
                    //chats.Add(chatlist.ToObject<List<string>>());
                }
            }


            for(int i=0; i< personas.Count; i++)
            {
                Persona newMovementInfo = new Persona(personas[i], act_address[i], pronunciatio[i], description[i], chats);
                personaList.Add(newMovementInfo);
            }
        }
        else
        {
            // get movement api 
            StartCoroutine(NPCServerManager.Instance.GetMovementCoroutine(gameName, stepNumber));
            personaList = NPCServerManager.Instance.CurrentMovementInfo;
        }


        /* read get movement */ 
        foreach(var movementInfo in personaList)
        { 
            foreach (var perceivedInfo in existingInfo.perceived_info)
            {
                int npcIndex = FindNPCIndex(perceivedInfo.persona);

                // NPC에 해당하는 persona가 movement file에 존재함
                if(movementInfo.Name == perceivedInfo.persona)
                {
                    /* --- ACT ADDRESS --- */

                    // tag extract
                    (string tag, string content) = ExtractTagAndContent(movementInfo.ActAddress);
                    // <persona> 
                    if(tag == "persona")
                    {
                        NPCName = content;

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

                           
                                        if (!chatManager.isChatting)
                                        {
                                            NPC[npcIndex].IconBubble.SetActive(true);
                                            NPC[j].IconBubble.SetActive(true);

                                            for (int k = 0; k < movementInfo.Chat.Count; k++)
                                            {
                                                var chat = movementInfo.Chat[k];
                                                string speaker = chat[0].ToString();
                                                string dialogue = chat[1].ToString();

                                                if (chatManager.dialogues.Count <= k)
                                                {
                                                    if (k % 2 == 0)
                                                        speakerIndex = 1;
                                                    else                                                   
                                                        speakerIndex = 0;
                                                
                                                    chatManager.dialogues.Add(new DialogueData { 
                                                        dialogue = dialogue,
                                                        name = speaker,
                                                        speakerIndex = speakerIndex
                                                        });

                                                    chatManager.isFirst = true;

                                                }


                                                //yield return new WaitUntil(()=>dialogueManager.UpdateDialogue());


                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // <location>
                    else if(tag == "location")
                    {
                        GetLocation(content, npcIndex);
                    }
                    
                    /* --- PRONUNCIATIO --- */
                    NPC[npcIndex].IconBubble.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text += movementInfo.Pronunciatio;
                }
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
                int npcIndex = FindNPCIndex(perceivedInfo.persona);
                
                if (npcIndex != -1)
                {
                    /*  UPDATE curr_address */
                    //curr_address 에 ": : :" 요형태 있어야 오류 안남. ""(빈스트링)도 안됨.
                    existingInfo.perceived_info[npcIndex].curr_address = "home:home:home:home";//NPC[npcIndex]._locationName;

                    for (int k = 0; k < NPC[npcIndex].detectedObject.Count; k++)
                    {
         
                        if (NPC[npcIndex].detectedObject[k].CompareTag("NPC"))
                        {
                            while(existingInfo.perceived_info[npcIndex].perceived_tiles.Count < k + 1)
                            {
                                existingInfo.perceived_info[npcIndex].perceived_tiles.Add(new PerceivedTile());
                            }

                            try
                            {
                                /*  UPDATE perceived_tiles.dist */ 
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].dist =
                                    Vector2.Distance(NPC[npcIndex].transform.position, NPC[npcIndex].detectedObject[k].transform.position);

                            }
                            catch (Exception  e)
                            {
                                Console.WriteLine(e);
                                //Debug.Log("Index "+ npcIndex);
                                throw;
                            }
                            
                            /*  UPDATE perceived_tiles.@event */ 
                            if(existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event == null)
                            {
                                //이벤트는 무조건 요소 4개 가지고 있는 배열이라 리스트에서 바꿈
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event =
                                    new string[4]; //new List<string>();
                               
                                existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event[0] = NPC[npcIndex].detectedObject[k].gameObject.name;
                            }

                            //지워도 될듯. 리스트를 배열로 바꾸면서 필요 없어짐.
                            // while (existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event.Count <= 3)
                            // {
                            //     existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event.Add(null);
                            // }

                            //사람 이름 넣는 코드 -> key "node_1"에러 나서 주석처리해둠
                           //existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event[0] = NPC[npcIndex]._detectedObject[k].gameObject.name.ToString();

                           //지워도 될듯. 리스트를 배열로 바꾸면서 필요 없어짐.
                            // Set the rest of @event to null
                            for (int j = 1; j < 4; j++)
                            {
                                //existingInfo.perceived_info[npcIndex].perceived_tiles[k].@event[j] = null;
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

        // Convert LunaData to JSON
        string PerceiveData = JsonConvert.SerializeObject(existingInfo, Formatting.Indented);
        
        // Save the JSON data to a file
        //string filePath = "Assets/NPCPerceiveFile.json";
        // Debug.Log(PerceiveData);
        File.WriteAllText(filePath, PerceiveData);
       
        //recall Perceive Post API
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