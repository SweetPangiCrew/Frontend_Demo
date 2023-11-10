using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using NavMeshPlus.Extensions;
using Panda.Examples.Move;
using System.Net;
using NPCServer;
using System.Linq;

public class NPC : MonoBehaviour // later, it will be global NPC Controller
{
    // Persona
    public Persona persona;

    // Move
    public Transform[] _LunaWaypoints;
    private int _currentWaypointIndex = 0;
    private float _waitTime = 3f;
    private NavMeshAgent agent;
    private float _waitCounter = 0f;
    private bool _isWaiting = false;

    // Perceive
    public Perceive _perceive;
    public PerceivedInfo _perceivedInfo;
    public string _name;
    public Vector3 _location;
    public float detectionRadius = 0.65f;
    public List<GameObject> _detectedObject;
    Vector3 direction;
    Vector3 rayOrigin;

    // Interact
    private bool isInteracting = false;
    public GameObject ChatBufferButton;

    void Start()
    {
        // Nav Mesh Agent
        agent = GetComponent<NavMeshAgent>();

        // Persona
        //persona = new Persona(this.name,"garden","walking","luna is walking",null);
        
        // Perceive
        _perceive = new Perceive()
        {
            perceived_info = new List<PerceivedInfo>(),
        };

        Move();
        InvokeRepeating("SaveToJson", 0f,2f);
    }

    void Update()
    {
        // Fix NPC Rotation
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // NPC Moving
        if (_isWaiting)
        {
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime)
            {
                _isWaiting = false;
                _waitCounter = 0f;
                Move();
            }
        }
        else
        {
            if (agent.remainingDistance <= 0.01)
            {
                _isWaiting = true;
            }
        }

        // NPC Perceive
        Perceive();
    }

    #region INTERACT
    public void Interact()
    {
        if (!isInteracting) // Luna is interacting with NPC
        { 

        }
        else // End Interacting
        {
            Debug.Log("End Interact!");

            agent.isStopped = false;
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _LunaWaypoints.Length;
            Move();

        }
    }
    #endregion

    #region PERCEIVE
    public void Perceive()
    {
        // update NPC name and location
        _location = this.GetComponent<Transform>().position;
        //_detectedObject = new List<GameObject>();

        // NPC perceive
        for (int i = 0; i < 10; i++)
        {
            float angle = i * 36.0f;
            RaycastHit2D ObjectHit = Physics2D.Raycast(rayOrigin, direction, detectionRadius, LayerMask.GetMask("InteractableObject"));
            Debug.DrawRay(rayOrigin, direction * detectionRadius, Color.green);

            if (ObjectHit.collider != null)
            {
                _detectedObject.Add(ObjectHit.collider.gameObject);  
                foreach (GameObject _object in _detectedObject)
                {
                    if (_object.CompareTag("NPC")) // if this gameobject perceives NPC
                    {   
                        agent.isStopped = true;
                        ChatBufferButton.SetActive(true);                      

                        // Interact
                    }
                }
            }          

        }

    }
    #endregion

    #region ADDRESS
    // perceive the location of NPC
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer.Equals("Address"))  
        {
            Debug.Log(other.gameObject.tag);   

            //_location = other.gameObject.tag.ToString();
        }
    }
    #endregion

    #region MOVE
    public void Move()
    {
        if (_LunaWaypoints.Length == 0) return;

        Transform nextWaypoint = _LunaWaypoints[_currentWaypointIndex];
        agent.SetDestination(nextWaypoint.position);
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _LunaWaypoints.Length;
        // Resume NPC movement
        agent.isStopped = false;
    }
    #endregion

    #region SAVE JSON
    public void SaveToJson()
    {
        /*
        // Deserialize existing data from the JSON file
        existingInfo = JsonConvert.DeserializeObject<Perceive>(PerceiveJSONFile.text);

        // Find the index of the existing PerceivedInfo with the same persona
        int existingIndex = existingInfo.perceived_info.FindIndex(info => info.persona == gameObject.name);

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
        }*/
        // Persona doesn't exist, add a new PerceivedInfo
        _perceivedInfo = new PerceivedInfo
        {
            persona = gameObject.name,
            curr_address = _location.ToString(),
            perceived_tiles = new List<PerceivedTile>(),
        };

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

    #endregion
}