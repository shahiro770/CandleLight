using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace General {

    [System.Serializable]
    public class SaveData {

        public PartyMember[] partyMembers;
        public int WAX;

        public SaveData() {
            
        }
    }
}