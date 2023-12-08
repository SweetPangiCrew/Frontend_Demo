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
    public NavMeshAgent agent;
    private float _waitCounter = 0f;
    private bool _isWaiting = false;

    // Perceive
    public Perceive _perceive;
    public PerceivedInfo _perceivedInfo;

    public string _name;
    public Vector2 _location;
    public string _locationName;
    public float detectionRadius = 0.65f;
    public List<GameObject> _detectedObject = null;
    Vector2 direction;
    Vector3 rayOrigin;

    // Interact
    private bool isInteracting = false;
    public GameObject ChatBufferButton;

    void Start()
    {
        // Nav Mesh Agent
        agent = GetComponent<NavMeshAgent>();
        
        // Perceive
        _perceive = new Perceive()
        {
            perceived_info = new List<PerceivedInfo>(),
        };

        Move();
 
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

/*
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
*/
    #region PERCEIVE
    public void Perceive()
    {
        // update NPC name and location
        _location = this.GetComponent<Transform>().position;

        // NPC perceive
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10.0f;

            // Initialize rayOrigin and direction based on NPC position and angle
            direction = Quaternion.Euler(1, 1, angle) * Vector2.up;
            rayOrigin = _location + direction * detectionRadius;

            RaycastHit2D ObjectHit = Physics2D.Raycast(rayOrigin, direction, detectionRadius, LayerMask.GetMask("InteractableObject"));
            Debug.DrawRay(rayOrigin, direction * detectionRadius, Color.red);   
            
            if(ObjectHit.collider != null)
            {
                GameObject detectedObject = ObjectHit.collider.gameObject;

                // Check if the object is not already in the list before adding
                if (!_detectedObject.Contains(detectedObject))
                {
                    _detectedObject.Add(detectedObject);
                }
            }
               
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


}