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
    public List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private float waitTime = 3f;
    public NavMeshAgent navMeshAgent;
    private float waitCounter = 0f;
    private bool isWaiting = false;

    // Perceive
    public Perceive perceive;
    public PerceivedInfo perceivedInfo;
    public Vector2 location;
    public string locationName;
    public float detectionRadius = 0.65f;
    public List<GameObject> detectedObjects = null;
    Vector2 direction;
    Vector3 rayOrigin;

    // Interact
    private bool isInteracting = false;
    public GameObject ChatBufferButton;

    // Canvas
    public GameObject IconBubble;
    public GameObject SpeechBubble;

    // Animator
    private Animator animator;

    void Start()
    {
        // Nav Mesh Agent
        navMeshAgent = GetComponent<NavMeshAgent>();

        // Animator
        animator = GetComponent<Animator>();

        // Perceive
        perceive = new Perceive()
        {
            perceived_info = new List<PerceivedInfo>(),
        };

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

        // NPC Moving
        if (isWaiting)
        {
            waitCounter += Time.deltaTime;
            if (waitCounter >= waitTime)
            {
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

                // Check if the object is not already in the list before adding
                if (!detectedObjects.Contains(detectedObject))
                {
                    detectedObjects.Add(detectedObject);
                }
            }
               
        }
    }
    #endregion

    #region MOVE
    public void Move()
    {
        if (waypoints.Count == 0) return;

        Transform nextWaypoint = waypoints[currentWaypointIndex];
        navMeshAgent.SetDestination(nextWaypoint.position);
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
        
        // Resume NPC movement
        navMeshAgent.isStopped = false;
    }

    public void AddWaypoint(Transform nl)
    {
        waypoints.Insert(currentWaypointIndex, nl);
    }
    #endregion


}