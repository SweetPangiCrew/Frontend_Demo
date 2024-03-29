using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDead : MonoBehaviour
{
    // 애니메이션 이후 사라짐
    private void Dead()
    {
        gameObject.SetActive(false);
    }
}
