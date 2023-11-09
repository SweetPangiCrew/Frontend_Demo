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
        
        //ToString Method
        public override string ToString()
        {
            string str = "";
            str += "name : " + name + "\n";
            str += "act_address : " + act_address + "\n";
            str += "pronunciatio : " + pronunciatio + "\n";
            str += "description : " + description + "\n";
            str += "chat : " + chat.ToString() + "\n";
            return str;
        }
        

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
   
    



