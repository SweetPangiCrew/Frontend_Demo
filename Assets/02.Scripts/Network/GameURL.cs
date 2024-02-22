using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameURL : MonoBehaviour
{
    public static class NPCServer
    {
        public static string Server_URL = "http://52.14.83.66:8000/";
        public static readonly string Remote_URL = "http://52.14.83.66:8000/";
        public static readonly string Local_URL = "http://127.0.0.1:8000/";
    
        public static readonly string getServerTime = "servertime/";
        public static readonly string getExistingGames = "existingGames/";
        public static readonly string getNPCMovement = "npc/movement/";
        public static readonly string postNPCPercention = "npc/perceive/";
        
        public static readonly string sendUserMessage = "chat/send/";
        public static readonly string getChatLists = "chat/list/";

    }

    // 회원가입, 로그인 등을 담당할 서버
    public static class AuthServer
    {
        public static readonly string Server_URL = "http://**.***.***.***:52531";

        public static readonly string userLogInPath = "/user_login";
    }
}
