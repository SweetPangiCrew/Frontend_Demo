using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class NPCMove : MonoBehaviour
{
    public Transform[] waypoints;
    private int _currentWaypointIndex;
    private float _speed = 2f;

    public void Move(){
        Transform wp = waypoints[_currentWaypointIndex];
        if(Vector3.Distance(transform.position, wp.position) < 0.01f){
            _currentWaypointIndex = (_currentWaypointIndex+1)%waypoints.Length;
        }else{
            transform.position = Vector3.MoveTowards(transform.position, wp.position, _speed+Time.deltaTime);
        }
    }
}
