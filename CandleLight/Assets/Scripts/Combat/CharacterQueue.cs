using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterQueue {
    
    private int monsterNumber;
    private int partyMemberNumber;
    private QueueNodeComparer characterOrder = new QueueNodeComparer();
    private List<QueueNode> combatQueue;

    public CharacterQueue() {
        combatQueue = new List<QueueNode>();
    }
    
    public void AddCharacter(Character c) {
        if (c is Monster) {
            Monster m = (Monster)c;
            combatQueue.Add(new QueueNode(m, false));
            monsterNumber++;
        }
        else if (c is PartyMember) {
           PartyMember pm = (PartyMember)c;
           combatQueue.Add(new QueueNode(pm, false));
           partyMemberNumber++;
        }
    }

    private void AddSecondEntries() {
        int avgMonsterDEX = 0;
        int avgPartyMemberDEX = 0;
        foreach (QueueNode q in combatQueue) {
            if (!q.isSecond) {
                if (q.c is Monster) {
                    avgMonsterDEX += q.c.DEX;
                }
                else {
                    avgPartyMemberDEX += q.c.DEX;
                }
            }
        }

        avgMonsterDEX /= monsterNumber;
        avgPartyMemberDEX  /= partyMemberNumber;

        List<QueueNode> finalCombatQueue = combatQueue.ConvertAll(q => new QueueNode(q));

        foreach (QueueNode q in combatQueue) {
            if (q.c is Monster) {
                if (q.c.DEX > avgPartyMemberDEX) {
                    finalCombatQueue.Add(new QueueNode(q.c, true));
                }
            }
            else {
                if (q.c.DEX > avgMonsterDEX) {
                    finalCombatQueue.Add(new QueueNode(q.c, true));
                }
            }
        }

        combatQueue = finalCombatQueue;
    }

    public void FinalizeQueue() {
        AddSecondEntries();
        combatQueue.Sort(characterOrder);
    }

    public void RemoveCharacter(int ID) {
        foreach (QueueNode q in combatQueue) {
            if (q.c.ID == ID) {
                combatQueue.Remove(q);
            }
        }
    }

    public PartyMember GetNextPM() {
        foreach (QueueNode q in combatQueue) {
            if (q.c is PartyMember) {
               return (PartyMember)q.c;
            }
        }

        return null;
    }

    public void LogQueue() {
        foreach (QueueNode q in combatQueue) {
            if (q.c is Monster) {
                Monster m = (Monster)q.c;
                Debug.Log("Priority: " + q.priority + " MonsterName: " + m.monsterName);
            }
            else {
                PartyMember pm = (PartyMember)q.c;
                Debug.Log("Priority: " + q.priority + " ClassName " + pm.className);
            }
        }
    }

    private class QueueNode : System.IEquatable<QueueNode>{
        
        public Character c { get; set; }
        public bool isSecond { get; set; }
        public int priority { get; set; }

        public QueueNode(Character c, bool isSecond) {
            this.c = c;
            this.isSecond = isSecond;
            if (!isSecond) {
                priority = c.DEX;
            }
            else {
                priority = c.DEX / 2;
            }
        }

        public QueueNode(QueueNode q) {
            this.c = q.c;
            this.isSecond = q.isSecond;
            this.priority = q.priority;
        }

        public bool Equals(QueueNode q) {
            if (q == null) { // don't have to compare at run time as type doesn't change
                return false;
            }
            
            if (q.c.ID == c.ID && q.isSecond == isSecond) {
                return true;
            }

            return false;
        }
    }

    private class QueueNodeComparer : Comparer<QueueNode> {

        public override int Compare(QueueNode q1, QueueNode q2) {
            if (q1.priority < q2.priority) {
                return 1;
            }
            if (q1.priority > q2.priority) {
                return -1;
            }

            return 0;
        }
    }
}

