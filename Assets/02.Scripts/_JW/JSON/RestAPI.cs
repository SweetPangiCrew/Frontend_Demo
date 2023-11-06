using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPCServer;


public class RestAPI : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetMovementCoroutine());
    }

    private IEnumerator GetMovementCoroutine()
    {
        yield return NPCServerManager.Instance.GetMovement("test4", 1);
    }

    // Update is called once per frame
    void Update()
    {
        // if(NPCServerManager.Instance.CurrentMovementInfo != null)
        //     ;
            //Debug.Log(NPCServerManager.Instance.CurrentMovementInfo[0].Name);
    }
}
