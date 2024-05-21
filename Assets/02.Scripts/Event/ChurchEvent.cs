using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChurchEvent : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI rel_txt;
    
    public GameObject paper;

    private bool isChecked = false;
    public void clickClose()
    {
        paper.SetActive(false);
        if (!isChecked)
        {
            isChecked = true;
            PlayerAction.reliability += 5f;
            panel.SetActive(true);
            panel.GetComponentInChildren<TextMeshProUGUI>().text = "신뢰도가 상승하였습니다!";
            panel.GetComponent<Panel>().Show_Panel();
            //rel_txt.text = "신뢰도: " + Mathf.FloorToInt(PlayerAction.getCurrentReliability()).ToString();
        }

       // Destroy(gameObject);
    }
    
    private bool isPlayerInTrigger = false;

    private void Update()
    {
        if (isPlayerInTrigger)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(!paper.activeSelf);
                if (!paper.activeSelf)
                {
                    paper.SetActive(true);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("esc");
                if (paper.activeSelf) clickClose();
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


