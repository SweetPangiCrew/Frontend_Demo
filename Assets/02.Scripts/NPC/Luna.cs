using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using System.IO;

public class Luna : MonoBehaviour // later, it will be global NPC Controller
{
    private NPCMove _LunaMove;
    public Transform[] _LunaWaypoints;
    private int _currentWaypointIndex;
    private float _speed = 2f;
    private NPCData _LunaData;
    public string _name; 
    public Vector3 _location;
    public float detectionRadius = 0.3f; 
    public List<GameObject> detectedTiles;

    void Start(){
        _LunaData = new NPCData();
    }

    void Update(){
        // fix NPC Rotation
        transform.rotation = Quaternion.Euler(0, 0, 0);

        // NPC Moving
        Move();

        // update NPC name and location
        _name = this.name;
        _location = this.GetComponent<Transform>().position;

        // update what NPC detected
        detectedTiles = new List<GameObject>();    
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45.0f;
            Vector3 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            Vector3 rayOrigin = _location + direction * detectionRadius;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, detectionRadius, LayerMask.GetMask("InteractableObject"));
            Debug.DrawRay(rayOrigin, direction * detectionRadius, Color.green);

            if (hit.collider != null){
                detectedTiles.Add(hit.collider.gameObject);

                foreach (GameObject tile in detectedTiles){                    
                    if(hit.collider.CompareTag("Player")){  // if ray hits the player
                        // Interact!
                        Debug.Log("I FOUND PLAYER!!!!!!!!");
                    }
                }                
            }
        }
    }

    public void Move(){
        Transform wp = _LunaWaypoints[_currentWaypointIndex];

        if(Vector3.Distance(transform.position, wp.position) < 0.01f){
            _currentWaypointIndex = (_currentWaypointIndex+1)%_LunaWaypoints.Length;
        }else{
            transform.position = Vector3.MoveTowards(transform.position, wp.position, _speed+Time.deltaTime);
        }
    }
    
    public void SaveToJson(){
        _LunaData.name = this.name;
        _LunaData.location = this.transform.position;

        // Convert LunaData to JSON
        string jsonData = JsonUtility.ToJson(_LunaData);

        // Save the JSON data to a file (you can specify the path)
        string filePath = Application.dataPath+"/LunaDataFile.json";
        File.WriteAllText(filePath, jsonData);

        Debug.Log("name: " + _name + ", location: " + _location);
        Debug.Log("Saved JSON data to: " + filePath);
    }
}
