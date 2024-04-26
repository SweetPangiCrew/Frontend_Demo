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
using System.Diagnostics.Tracing;

public class NPC : MonoBehaviour // later, it will be global NPC Controller
{
    // Move
    public List<Routine> routines;
    private int currentWaypointIndex = 0;
    public NavMeshAgent navMeshAgent;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    // Perceive
    private Vector2 location;
    private float detectionRadius = 0.75f;
    public List<GameObject> detectedObjects = null;
    Vector2 direction;
    Vector3 rayOrigin;

    // Canvas
    public GameObject IconBubble;
    public GameObject SpeechBubble;
    public GameObject DescriptionBubble;

    // Animator
    private Animator animator;

    // Location
    public string curr_address = "none";

    void Start()
    {
        // Nav Mesh Agent
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Animator
        animator = GetComponent<Animator>();

        Move();
    }
    void FixedUpdate()
    {
        if(animator)
        {
            Vector3 velocity = navMeshAgent.velocity;
            bool isMoving = velocity.magnitude > 0.1f; // 임의의 최소 속도 설정
            
            animator.SetBool("isWalk", isMoving);
            animator.SetBool("walk_f", velocity.z < -0.1f); // 아래로 이동
            animator.SetBool("walk_l", velocity.x < -0.1f); // 왼쪽으로 이동
            animator.SetBool("walk_r", velocity.x > 0.1f);  // 오른쪽으로 이동
            animator.SetBool("walk_b", velocity.z > 0.1f);  // 위로 이동
        }
    }

    void Update()
    {
        // Fix NPC Rotation
        transform.rotation = Quaternion.Euler(0, 0, 0);

        if(routines.Count != 0)
        {   
            // NPC Moving
            if (isWaiting)
            {   
                //Debug.Log(currentWaypointIndex);
                waitCounter += Time.deltaTime;
                if (waitCounter >= routines[currentWaypointIndex].waitTime)
                {
                    Debug.Log(waitCounter);
                    isWaiting = false;
                    waitCounter = 0f;
                    Move();
                }
            }
            else
            {
                if (navMeshAgent.remainingDistance <= 0.01)
                {
                    isWaiting = true;
                }
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
        location = this.GetComponent<Transform>().position;

        // NPC perceive
        for (int i = 0; i < 36; i++)
        {
            float angle = i * 10.0f;

            // Initialize rayOrigin and direction based on NPC position and angle
            direction = Quaternion.Euler(1, 1, angle) * Vector2.up;
            rayOrigin = location + direction * detectionRadius;

            RaycastHit2D ObjectHit = Physics2D.Raycast(rayOrigin, direction, detectionRadius, LayerMask.GetMask("InteractableObject"));
            Debug.DrawRay(rayOrigin, direction * detectionRadius, Color.red);   
            
            if(ObjectHit.collider != null)
            {
                GameObject detectedObject = ObjectHit.collider.gameObject;
                if(detectedObject.name == "플레이어" || detectedObject.name == gameObject.name)
                    continue;
                else if(!detectedObjects.Contains(detectedObject))
                    detectedObjects.Add(detectedObject);
            }
        }
    }
    #endregion

    #region MOVE
    public void Move()
    {
        if (routines.Count == 0) return;

        Transform nextWaypoint = routines[currentWaypointIndex].wayPoint;
        navMeshAgent.SetDestination(nextWaypoint.position);
        currentWaypointIndex = (currentWaypointIndex + 1) % routines.Count;
        
        // Resume NPC movement
        navMeshAgent.isStopped = false;
    }

    public void AddWaypoint(Transform nl, int time)
    {
        Routine newRoutine = new Routine
        {
            wayPoint = nl,
            waitTime = time
        };

        routines.Insert(currentWaypointIndex, newRoutine);
    }
    #endregion

    #region LOCATION
    void OnTriggerEnter2D(Collider2D other)
    {
        curr_address = other.gameObject.name;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        curr_address = "none";
    }  
    #endregion
}

[System.Serializable]
public struct Routine
{
    public Transform wayPoint;
    public int waitTime;
}