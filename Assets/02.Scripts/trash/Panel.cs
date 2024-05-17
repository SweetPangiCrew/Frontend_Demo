using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Panel : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Show_Panel()
    {
        StartCoroutine("show_panel");
    }

    IEnumerator show_panel()
    {

        yield return new WaitForSeconds(2);

        gameObject.SetActive(false);
    }
}
