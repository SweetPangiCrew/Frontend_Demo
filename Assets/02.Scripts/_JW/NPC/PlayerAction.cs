using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    float h;
    float v;
    float speed = 3;
    Rigidbody2D rigid;

    public float detectionRadius = 0.3f; // Ž�� �ݰ�

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        Vector2 playerPosition = transform.position;

        List<GameObject> detectedTiles = new List<GameObject>();

        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45.0f;
            Vector2 direction = Quaternion.Euler(0, 0, angle) * Vector2.up;
            Vector2 rayOrigin = playerPosition + direction * detectionRadius;

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction, detectionRadius, LayerMask.GetMask("Object"));

            Debug.DrawRay(rayOrigin, direction * detectionRadius, Color.green);

            if (hit.collider != null)
            {
                detectedTiles.Add(hit.collider.gameObject);
            }

        }

    }

    void FixedUpdate()
    {
        rigid.velocity = new Vector2(h * speed, v * speed);
    }


}
