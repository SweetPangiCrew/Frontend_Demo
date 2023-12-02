using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPCServer;


public class RestAPI : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //함수 사용 주의 *** 헤더 using NPCServer 선언하고 사용 가능
        //성공적으로 GetMovementInfo를 받아오면 NPCServerManager.Instance.CurrentMovementInfo에 저장됨
        //NPCServerManager의 OnSucceed, OnFailed, OnNetworkFailed를 통해 결과값을 받아올 수 있음
        //OnSucceed에서 다음 스텝의 perceive를 호출 가능하다는 status로 바꿔야할 듯 
       StartCoroutine( NPCServerManager.Instance.GetMovementCoroutine(0));
       StartCoroutine( NPCServerManager.Instance.GetServerTimeCoroutine());
     // StartCoroutine( NPCServerManager.Instance.PostPerceiveCoroutine(,0));

    }

  

    // Update is called once per frame
    void Update()
    {
        // if(NPCServerManager.Instance.CurrentMovementInfo != null)
        //     ;
            //Debug.Log(NPCServerManager.Instance.CurrentMovementInfo[0].Name);
    }
}
