    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using Newtonsoft.Json;

    public class JSONReader : MonoBehaviour
    {
        public TextAsset _textPersona;
        public TextAsset _textSpacialMemory;

        // persona.json 파일 읽어오기


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

        public PersonaList myPersonaList = new PersonaList();



        // spacial_memory.json 파일 읽어오기    
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
        
        public SpacialMemoryList mySpacialMemoryList = new SpacialMemoryList();
        void Start()
        {
            myPersonaList = JsonConvert.DeserializeObject<PersonaList>(_textPersona.text);
            mySpacialMemoryList = JsonConvert.DeserializeObject<SpacialMemoryList>(_textSpacialMemory.text);

            Debug.Log("Cafe List:");
                foreach (string item in mySpacialMemoryList.theVille.hobbsCafe.cafe)
                {
                    Debug.Log(item);
                }

                Debug.Log("Mainroom List:");
                foreach (string item in mySpacialMemoryList.theVille.isabellaRodriguezApartment.mainroom)
                {
                    Debug.Log(item);
                }

                Debug.Log("Bathroom List:");
                foreach (string item in mySpacialMemoryList.theVille.isabellaRodriguezApartment.bathroom)
                {
                    Debug.Log(item);
                }
                
                Debug.Log("Pub List:");
                foreach (string item in mySpacialMemoryList.theVille.theRoseAndCrownPub.pub)
                {
                    Debug.Log(item);
                }
        }

    }
