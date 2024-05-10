using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingCollider : MonoBehaviour
{
    // 종교 집회에 모인 npc 리스트
    public static List<string> assembled_npc = new List<string>();
    public int assembled_num;

    // Start is called before the first frame update
    void Start()
    {
        assembled_num = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name != "나주교" && collision.name != "플레이어")
        {
            assembled_num++;
            assembled_npc.Add(collision.gameObject.name);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.name != "나주교" && collision.name != "플레이어")
        {
            assembled_num--;
            assembled_npc.Remove(collision.gameObject.name);
        }

    }
}
