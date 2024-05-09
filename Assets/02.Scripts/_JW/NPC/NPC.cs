using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NPCServer;

public class NPC : MonoBehaviour 
{
    // Move
    public List<Routine> routines;
    public List<LocationTag> locationTags;
    public bool locationTag = true;
    private int currentRoutineIndex = 0;
    private int currentLocationTagIndex = 0;
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

        //Move();
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
            if (!isWaiting)
            {

                if(locationTag)
                {
                    while(currentLocationTagIndex <= locationTags.Count-1)
                    {
                            
                        Transform nextWaypoint = locationTags[currentLocationTagIndex].wayPoint;

                        waitCounter += Time.deltaTime;
                        if (waitCounter <= locationTags[currentLocationTagIndex].waitTime)
                        {
                            isWaiting = true;
                            waitCounter = 0f;

                            Debug.Log(nextWaypoint.name);
                            navMeshAgent.SetDestination(nextWaypoint.position);
                            currentLocationTagIndex = (currentLocationTagIndex + 1) % locationTags.Count; 
                        }
                        //if(currentLocationTagIndex == locationTags.Count-1)
                          //  locationTag = false;
                    }

                    locationTag = false;

                }
                else
                {                        
                    isWaiting = true;
                    SetRoutine();
                }

                navMeshAgent.isStopped = false;

            }
            else
            {
                if (navMeshAgent.remainingDistance <= 0.01)
                {
                    isWaiting = false;
                }
            }
        }


        

        // NPC Perceive
        Perceive();
    }

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
/*    public void Move()
    {
        //if(routines.Count == 0) return;

        if(locationTag)
        {            
            Transform nextWaypoint = locationTags[currentLocationTagIndex].wayPoint;
            Debug.Log(nextWaypoint.name);
            navMeshAgent.SetDestination(nextWaypoint.position);
            currentLocationTagIndex = (currentLocationTagIndex + 1) % locationTags.Count;  // Proper wrap-around increment.
            locationTag = false;
        }
        else
        {
            SetRoutine();
        }

        // Resume NPC movement
        navMeshAgent.isStopped = false;

    }*/

    public void SetRoutine()
    {
        int curr_time = Clock.Instance.GetCurrentTime().Hour;
        foreach(var routine in routines)
        {
            if(routine.startTime == curr_time)
            {
                navMeshAgent.SetDestination(routine.wayPoint.position);
                //Debug.Log(routine.wayPoint.position);
                navMeshAgent.isStopped = false;
            }                 
        }
    }  

    public void AddWaypoint(Transform nl, int time)
    {
        LocationTag locationTag = new LocationTag
        {
            wayPoint = nl,
            waitTime = time,
        };

        locationTags.Insert(currentRoutineIndex, locationTag);
        currentRoutineIndex++;
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
    public int startTime;
}

[System.Serializable]
public struct LocationTag
{
    public Transform wayPoint;
    public int waitTime;
}