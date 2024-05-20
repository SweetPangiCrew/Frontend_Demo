using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class trash : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI rel_txt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlayerAction.reliability += 1f;
                panel.SetActive(true);
                panel.GetComponentInChildren<TextMeshProUGUI>().text = "신뢰도가 상승하였습니다!";
                panel.GetComponent<Panel>().Show_Panel();
                //rel_txt.text = "신뢰도: " + Mathf.FloorToInt(PlayerAction.getCurrentReliability()).ToString();
                Destroy(gameObject);
            }
        }

    }


}
