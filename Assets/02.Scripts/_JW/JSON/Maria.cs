using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPCServer;

public class Maria : MonoBehaviour
{
    public  Persona mariaPersona;
    void Start(){
        mariaPersona = new Persona("Maria",null,null,null,null);
    }

    void Update(){

    }
}
