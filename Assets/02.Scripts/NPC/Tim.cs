using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tim : MonoBehaviour
{
    // Perceive
    private TimData _TimData;
    public string _name;
    public Vector3 _location;
    public float detectionRadius = 0.65f;
    public List<GameObject> _detectedObject;

    void Start()
    {
        _TimData = new TimData();
        //InvokeRepeating("SaveToJson", 0f, 10f);
    }

    void Update()
    {
        // NPC Perceive
        Perceive();
    }

    #region PERCEIVE
    public void Perceive()
    {
        // update NPC name and location
        _name = this.name;
        _location = this.GetComponent<Transform>().position;
        _detectedObject = new List<GameObject>();

        // NPC perceive
        for (int i = 0; i < 10; i++)
        {
            float angle = i * 36.0f;
            Vector3 direction = Quaternion.Euler(1, 1, angle) * Vector2.up;
            Vector3 rayOrigin = _location + direction * detectionRadius;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, detectionRadius, LayerMask.GetMask("InteractableObject"));
            Debug.DrawRay(rayOrigin, direction * detectionRadius, Color.green);

            if (hit.collider != null)
            {
                _detectedObject.Add(hit.collider.gameObject);

                foreach (GameObject _object in _detectedObject)
                {
                    if (_object.CompareTag("NPC"))
                    {
                        //Debug.Log("Hello");
                    }
                }
            }
        }
    }
    #endregion
}
