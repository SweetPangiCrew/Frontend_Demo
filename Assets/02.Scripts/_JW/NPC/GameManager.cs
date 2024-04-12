using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    
    public bool usingLocalServer = false;
    private string filePath;

    // timer
    private DateTime curr_time;
    private int stepTime = 18; // 게임 ?�간?�로 18�?마다 ?�텝???�데?�트 ??

    // Get Movement
    private int step;
    // Get Movement from local file, true : only Test Mode available
    public bool isUsingMovementLocalFile = true;
    public string gameName; 
    public string NPCName; 
    
    // Chat 
    public ChatManager chatManager;
    private int npcIndex;
    private int otherNpcIndex;
    private HashSet<string> conversationPairs = new HashSet<string>();
    
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
           if (usingLocalServer)
           {
               GameURL.NPCServer.Server_URL = GameURL.NPCServer.Local_URL;
           }

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
        int pStep = 1; //?��? base?�서 perceive가 0???�음.
        while (true)
        {
            curr_time = Clock.Instance.GetCurrentTime();

            int minute = curr_time.Hour * 60 + curr_time.Minute; 
            
            //server manage?�서 ?�버가 ???�렸?�때
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
                //step???�라가???�?�밍???�을 ?????�번�??�출
                if (pStep == step && NPCServerManager.Instance.getReaction || isTest)
                {
                    lasttime = minute;
                    SaveJsonFile(); //perceive 
                    NPCServerManager.Instance.getReaction = false;
                    pStep++;
                }

                //server가 Perceive ?�일??받았????
                if (NPCServerManager.Instance.perceived)
                {
                    GetMovement(step);
                    step++;
                    NPCServerManager.Instance.perceived = false;
                }
            }

            yield return new WaitForSeconds(1f); //10초�? 18�??�눴????0.55556?�라 0.5??반복?�면 1�??�위 게임 ?�간??모두 체크??
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
                  
                }
                chats.Add(chat);
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

        if (personaList.Count==0 ||personaList == null || NPC == null)
        {
            Debug.LogWarning("personaList or NPC list is null.");
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
                            otherNpcIndex = FindNPCIndex(NPCName);
                            NPC[npcIndex].navMeshAgent.isStopped = true;
                            NPC[otherNpcIndex].navMeshAgent.isStopped = true;
                                        
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
                                conversationPairs.Add(conversationPair);      
                                chatManager.LoadDialogue(personaList[npcIndex].Chat, npcIndex, otherNpcIndex); 

                                NPC[npcIndex].IconBubble.SetActive(false);
                                NPC[otherNpcIndex].IconBubble.SetActive(false);
                            }
                        }
                    }
                }

                else if(tag == "location")
                {
                    GetLocation(content, npcIndex);
                }
                    
                /* --- PRONUNCIATIO --- */
                string[] emoji = personaList[npcIndex].Pronunciatio.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                NPC[npcIndex].IconBubble.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = NPC[npcIndex].gameObject.name + ":";

                for(int i = 0; i < emoji.Length; i++)
                {
                    string extension = Path.GetExtension(emoji[i]);
                    
                    string fileNameWithoutExtension = emoji[i].Replace(extension, "");
                    string imagePathWithName = "Pronunciatio/" + fileNameWithoutExtension;
                    Sprite sprite = Resources.Load<Sprite>(imagePathWithName);

                    if (sprite != null)
                        NPC[npcIndex].IconBubble.transform.GetChild(0).GetChild(i).GetComponent<Image>().sprite = sprite;
                }

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
                NPC[npcIndex].AddWaypoint(nl, 10);      // 임시로 10초 넣음
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
                    existingInfo.perceived_info[npcIndex].curr_address = NPC[npcIndex].curr_address+":home:home:home";

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
