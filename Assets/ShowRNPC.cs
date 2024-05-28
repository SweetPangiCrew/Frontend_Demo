using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowRNPC : MonoBehaviour
{
    public UnityEngine.UI.Button createTextButton;  // 버튼 참조
    public GameObject parentObject; 
    public GameObject panel; 
    
    public TMP_FontAsset fontAsset;
    
    // Start is called before the first frame update
    void Start()
    {
        createTextButton.onClick.AddListener(CreateText);
    }
    
    void CreateText()
    {

        if(panel.activeSelf){ panel.SetActive(false); return; }
        
        foreach (Transform child in parentObject.transform)
        {
            Destroy(child.gameObject);
        }
        
        panel.SetActive(true);
            
        GameObject[] NPC;
        NPC = GameObject.FindGameObjectsWithTag("NPC");
        foreach (GameObject npc in NPC)
        {
            int rIndex = ReligiousIndexNetworkManager.Instance.RIndexInfo[npc.name];
            if (rIndex >= 30)
            {
                // TextMeshPro 텍스트 오브젝트 생성
                GameObject textObject = new GameObject("Name");
                textObject.transform.SetParent(parentObject.transform);

                // TextMeshPro 컴포넌트 추가 및 설정
                TextMeshProUGUI tmpText = textObject.AddComponent<TextMeshProUGUI>();
                tmpText.text = npc.name;
                tmpText.font = fontAsset;

            }
        }

    }
}
