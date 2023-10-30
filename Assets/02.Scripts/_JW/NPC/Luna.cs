using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;
using UnityEngine.AI;
using NavMeshPlus.Extensions;
using Panda.Examples.Move;
using System.Net;

public class Luna : MonoBehaviour // later, it will be global NPC Controller
{
    // Move
    public Transform[] _LunaWaypoints;
    private int _currentWaypointIndex = 0;
    private float _waitTime= 3f;
    private NavMeshAgent agent;
    private float _waitCounter = 0f;
    private bool _isWaiting = false;


    // Perceive
    private NPCData _LunaData;
    public string _name; 
    public Vector3 _location;
    public float detectionRadius = 0.5f; 
    public List<GameObject> _detectedObject;
    private bool _isFound = false;

    void Start(){
        _LunaData = new NPCData();
        agent = GetComponent<NavMeshAgent>();
        GoToNextWaypoint();
    }

    void Update(){

        // Fix NPC Rotation
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // NPC Moving
        if(_isWaiting){
            _waitCounter += Time.deltaTime;
            if (_waitCounter >= _waitTime){
                _isWaiting = false;
                _waitCounter = 0f;
                GoToNextWaypoint();
                Debug.Log("Move");
            }              
        }else{
            if (agent.remainingDistance <= 0.01 || _isFound){
                _isWaiting = true;
            }
        }
        
        // update NPC name and location
        _name = this.name;
        _location = this.GetComponent<Transform>().position;


        // update what NPC detected
        _detectedObject = new List<GameObject>();    
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45.0f;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            Vector3 rayOrigin = _location + direction * detectionRadius;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, detectionRadius, LayerMask.GetMask("InteractableObject"));
            Debug.DrawRay(rayOrigin, direction * detectionRadius, Color.green);

            if (hit.collider != null){
                _detectedObject.Add(hit.collider.gameObject);

                foreach (GameObject _object in _detectedObject){                    
                    if(_object.CompareTag("NPC")){  // if ray hits the player
                        _isFound = true;

                        // Interact
                        _isWaiting = false;
                        agent.isStopped = true;
                        Debug.Log("I FOUND NPC!!!!!!!!");
                        SaveToJson();
                        
                    }
                }                
            }
        }
    }

    void GoToNextWaypoint(){
        if(_LunaWaypoints.Length == 0) return;

        Transform nextWaypoint = _LunaWaypoints[_currentWaypointIndex];
        agent.SetDestination(nextWaypoint.position);
        _currentWaypointIndex = (_currentWaypointIndex + 1) % _LunaWaypoints.Length;
        agent.isStopped = false;   
    }
    
    public void SaveToJson(){

        _LunaData.name = this.name;
        _LunaData.location = this.transform.position;
        _LunaData.detectedObject = this._detectedObject;

        // Convert LunaData to JSON
        string jsonData = JsonUtility.ToJson(_LunaData);

        // Save the JSON data to a file (you can specify the path)
        string filePath = Application.dataPath+"/LunaDataFile.json";
        File.WriteAllText(filePath, jsonData);

        //Debug.Log("name: " + _name + ", location: " + _location);
        //Debug.Log("Saved JSON data to: " + filePath);
        foreach(GameObject objects in _LunaData.detectedObject)
        Debug.Log("Luna found "+ objects.name);
    }
}
