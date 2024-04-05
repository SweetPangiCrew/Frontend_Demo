    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using TMPro;

    public class PlayerAction : MonoBehaviour
    {
        float h;
        float v;
        float speed = 3;

        Rigidbody2D rigid;
        Animator animator;

        public float detectionRadius = 0.3f; // Ž�� �ݰ�
        public static float reliability;    //유저 신뢰도


        // Canvas_Location
        public GameObject Canvas_Location;
        public TextMeshProUGUI Txt_Location;

        // Start is called before the first frame update
        void Awake()
        {
            reliability = 0;
            rigid = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
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

        void OnTriggerEnter2D(Collider2D other)
        {
            Canvas_Location.GetComponent<Animator>().Play("Location_Tag_Active");
            Txt_Location.text = other.gameObject.name;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            Canvas_Location.GetComponent<Animator>().Play("Location_Tag_Inactive");
        }   


        void FixedUpdate()
        {
            rigid.velocity = new Vector2(h * speed, v * speed);

            //move animation
            animator.SetBool("isWalk", rigid.velocity.x != 0 || rigid.velocity.y != 0);   
            animator.SetBool("walk_f", rigid.velocity.x == 0 && rigid.velocity.y < 0);   //down
            animator.SetBool("walk_l", rigid.velocity.x < 0);   //left
            animator.SetBool("walk_r", rigid.velocity.x > 0);   //right
            animator.SetBool("walk_b", rigid.velocity.x == 0 && rigid.velocity.y > 0);   //up
        }

        public static float getCurrentReliability()
        {
            return reliability;
        }

    }
