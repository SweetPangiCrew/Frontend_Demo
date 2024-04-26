using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using JetBrains.Annotations;

public class JSONReader : MonoBehaviour
{
    //public TextAsset _textPersona;
    //public TextAsset _textSpacialMemory;

    [SerializeField]
    private TextAsset _perceive;

    [SerializeField]
    private Perceive myPerceiveList;

    /*
    #region persona.json 파일 읽어오기

            [System.Serializable]
            public class Persona
            {
                public string name;
                public string act_address;
                public string pronunciatio;
                public string description;
                public string chat;
            }

            [System.Serializable]
            public class PersonaList
            {
                public Persona[] persona;
            }
    #endregion
    #region spacial_memory.json 파일 읽어오기    
            [System.Serializable]
            public class TheVille
            {
                public HobbsCafe hobbsCafe;
                public IsabellaRodriguezApartment isabellaRodriguezApartment;
                public TheRoseAndCrownPub theRoseAndCrownPub;
            }

            [System.Serializable]
            public class HobbsCafe
            {
                public List<string> cafe;
            }

            [System.Serializable]
            public class IsabellaRodriguezApartment
            {
                public List<string> mainroom;
                public List<string> bathroom;
            }

            [System.Serializable]
            public class TheRoseAndCrownPub
            {
                public List<string> pub;
            }

            [System.Serializable]
            public class SpacialMemoryList
            {
                public TheVille theVille;
            }
            
    #endregion
    */   

    void Start()
    {
        //myPersonaList = JsonConvert.DeserializeObject<PersonaList>(_textPersona.text);
        //mySpacialMemoryList = JsonConvert.DeserializeObject<SpacialMemoryList>(_textSpacialMemory.text);
        myPerceiveList = JsonConvert.DeserializeObject<Perceive>(_perceive.text);

        if (myPerceiveList != null)
        {
            Debug.Log("Perceive List Deserialized Successfully!");
            Debug.Log($"Number of PerceivedInfo items: {myPerceiveList.perceived_info.Count}");
        }
        else
        {
            Debug.LogError("Failed to Deserialize Perceive List!");
        }
                
    }

}
