using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class Reliability : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<TextMeshProUGUI>().text = "신뢰도: " + Mathf.FloorToInt(PlayerAction.getCurrentReliability()).ToString();
    }
}
