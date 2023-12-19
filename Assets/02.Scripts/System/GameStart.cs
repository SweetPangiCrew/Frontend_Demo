using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Collections;

public class GameStart : MonoBehaviour
{
    public TMP_InputField baseInput;

    public TMP_InputField nameInput;

    private Button startBtn;
    // Start is called before the first frame update
    void Start()
    {
        startBtn = gameObject.GetComponent<Button>();
        baseInput.onValueChanged.AddListener(OnBaseValueChanged);
        nameInput.onValueChanged.AddListener(OnNameValueChanged);
       // startBtn.onClick.AddListener(gameStart);
    }

    void OnBaseValueChanged(string newValue)
    {
        GameManager.Instance.simCode = newValue;
    }

    void OnNameValueChanged(string newValue)
    {
        GameManager.Instance.gameName = newValue;
    }
    public void gameStart()
    {
        Debug.Log(GameManager.Instance.simCode + GameManager.Instance.gameName);
    }
}
