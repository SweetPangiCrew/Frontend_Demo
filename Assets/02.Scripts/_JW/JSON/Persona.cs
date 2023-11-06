using System;
using System.Collections.Generic;
using UnityEngine;

namespace NPCServer
{
    [System.Serializable]
    public class Persona
    {
        // BEGIN: ed8c6549bwf9
        private string name;
        private string act_address;
        private string pronunciatio;
        private string description;
        private List<string> chat;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string ActAddress
        {
            get { return act_address; }
            set { act_address = value; }
        }

        public string Pronunciatio
        {
            get { return pronunciatio; }
            set { pronunciatio = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public List<string> Chat
        {
            get { return chat; }
            set { chat = value; }
        }
        // END: ed8c6549bwf9

        public Persona(string name, string act_address, string pronunciatio, string description, List<string> chat)
        {
            this.name = name;
            this.act_address = act_address;
            this.pronunciatio = pronunciatio;
            this.description = description;
            this.chat = chat;
        }
    }
}
    //     private string name;
    //     private string act_address;
    //     private string pronunciatio;
    //     private string description;
    //     private List<string> chat;

    //     public Persona(string name, string act_address, string pronunciatio, string description, List<string> chat)
    //      {
    //     this.name = name;
    //     this.act_address = act_address;
    //     this.pronunciatio = pronunciatio;
    //     this.description = description;
    //     this.chat = chat;
    //     }
    // }


    // [System.Serializable]
    // public class PersonaList
    // {
    //     public Persona[] persona;

    //     public PersonaList(Persona[] persona)
    //     {
    //         this.persona = persona;
    //     }


    
    // }
    

    



