using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class trash : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI rel_txt;

    private bool isPlayerInTrigger = false;

    private void Update()
    {
        if (isPlayerInTrigger)
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isPlayerInTrigger = false;
        }
    }

  


}
