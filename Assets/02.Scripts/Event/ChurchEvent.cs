using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChurchEvent : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI rel_txt;
    public GameObject speakPanel;
    public GameObject paper;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI decriptTxt;

    private bool isChecked = false;
    private bool is1Checked = false;
    public void clickClose()
    {
        paper.SetActive(false);
        if (!isChecked)
        {
          
            PlayerAction.reliability += 5f;
            panel.SetActive(true);
            panel.GetComponentInChildren<TextMeshProUGUI>().text = "신뢰도가 상승하였습니다!";
            panel.GetComponent<Panel>().Show_Panel();
            
            
            nameTxt.text = Database.Instance.username;
            decriptTxt.text = "뭐야,,,무섭잖아! 빨리 사람들이 종교 집회에 못가게 막아야 겠어! \n(사람들과 대화하자) [space] ";
            speakPanel.SetActive(true);
            isChecked = true;
            is1Checked = true;
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
                if (!paper.activeSelf&&!is1Checked)
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
            is1Checked = false;
        }
    }
    
    


}


