using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameURL : MonoBehaviour
{
    public static class NPCServer
    {
        public static readonly string Server_URL = "http://3.135.8.77:8000/";
    
        public static readonly string getServerTime = "servertime/";
        
        public static readonly string getNPCMovement = "npc/movement/";
        
        public static readonly string postNPCPercention = "npc/perceive/";

    }

    // 회원가입, 로그인 등을 담당할 서버
    public static class AuthServer
    {
        public static readonly string Server_URL = "http://**.***.***.***:52531";

        public static readonly string userLogInPath = "/user_login";
    }
}
