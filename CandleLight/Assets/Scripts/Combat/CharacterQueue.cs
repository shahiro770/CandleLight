/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The CharacterQueue class stores the order in which character take actions in combat.
* It handles all logic to determine who's turn is next in combat.
*
*/

using Characters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    TO DO
    - Determine the best way to handle fast and slow characters
    - If we give fast guys a second entry
        - When adding a character, redo everyone's second entries, checking after speed stat changes
        - When speed bosts occur, resort the queue, remembering whos' current turn it is
            - Figure out where in the queue the current person's turn resumes from
 */

namespace Combat {

    public class CharacterQueue {
        
        private int monsterNumber;              /// <value> Number of monsters in queue </value>
        private int partyMemberNumber;          /// <value> Number of party members in queue </value>
        private int queueLength = 0;            /// <value> Length of the queue </value>
        private int queuePos = 0;               /// <value> Position in the queue </value>
        //private int dexConst = 0;               /// <value> Speed constant to beat in order to get a second turn in one queue </value>
        private QueueNodeComparer characterOrder = new QueueNodeComparer();     /// <value> Character comparer to determine queue position </value>
        private List<QueueNode> combatQueue;    /// <value> List of characters, ordererd by the characterOrder </value>

        /// <summary>
        /// Constructor
        /// </summary>
        public CharacterQueue() {
            combatQueue = new List<QueueNode>();
        }
        
        /// <summary>
        /// Adds a character to the queue
        /// </summary>
        /// <param name="c"> Character to be added, either a monster or a partyMember </param>
        public void AddCharacter(Character c) {
            if (c is Monster) {
                Monster m = (Monster)c;
                combatQueue.Add(new QueueNode(m));//, false));
                monsterNumber++;
            }
            else if (c is PartyMember) {
                PartyMember pm = (PartyMember)c;
                combatQueue.Add(new QueueNode(pm));//, false));
                partyMemberNumber++;
            }

            queueLength++;
        }

        /* 
        /// <summary>
        /// Adds a character's second entry to the queue under certain conditions
        /// </summary>
        private void AddSecondEntries() {
            int avgMonsterDEX = 0;
            int avgPartyMemberDEX = 0;
            foreach (QueueNode q in combatQueue) {
                if (q.c is Monster) {
                    avgMonsterDEX += q.c.DEX;
                }
                else {
                    avgPartyMemberDEX += q.c.DEX;
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
        */

        /// <summary>
        /// Finalizes the queue order by making any special adjustments and then sorting
        /// </summary>
        public void FinalizeQueue() {
            //AddSecondEntries();
            combatQueue.Sort(characterOrder);
        }

        /// <summary>
        /// Returns the next character in the queue
        /// </summary>
        /// <returns></returns>
        public Character GetNextCharacter() {
            return combatQueue[queuePos++ % queueLength].c;
        }

        /// <summary>
        /// Removes a character from the queue, including all of their entries
        /// </summary>
        /// <param name="ID"> Unique ID of the character to be removed </param>
        public void RemoveCharacter(int ID) {
            List<QueueNode> finalCombatQueue = combatQueue.ConvertAll(q => new QueueNode(q));

            foreach (QueueNode q in combatQueue) {
                if (q.c.ID == ID) {
                    if (q.c is Monster) {
                        monsterNumber--;
                    }
                    else {
                        partyMemberNumber--;
                    }
                    finalCombatQueue.Remove(q);
                }
            }

            combatQueue = finalCombatQueue;
        }

        /// <summary>
        /// Checks if there are no alive party members
        /// </summary>
        /// <returns> Returns true if no partyMembers left in the queue </returns>
        public bool CheckPartyDefeated() {
            return partyMemberNumber == 0;
        }

        /// <summary>
        /// Checks if there are no alive monsters
        /// </summary>
        /// <returns> Returns true if no monsters left in the queue </returns>
        public bool CheckMonstersDefeated() {
            return monsterNumber == 0;
        }

        /// <summary>
        /// Returns the first party member in the queue
        /// </summary>
        /// <returns></returns>
        public PartyMember GetFirstPM() {
            foreach (QueueNode q in combatQueue) {
                if (q.c is PartyMember) {
                    return (PartyMember)q.c;
                }
            }

            return null;
        }

        /// <summary>
        /// Logs the contents of the combat queue
        /// </summary>
        public void LogQueue() {
            foreach (QueueNode q in combatQueue) {
                if (q.c is Monster) {
                    Monster m = (Monster)q.c;
                    Debug.Log("Priority: " + q.priority + " MonsterName: " + m.monsterDisplayName);
                }
                else {
                    PartyMember pm = (PartyMember)q.c;
                    Debug.Log("Priority: " + q.priority + " ClassName " + pm.className);
                }
            }
        }

        /// <summary>
        /// Node to encapsulate each character and additional information in the queue
        /// for easy sorting and logic.
        /// </summary>
        /// <typeparam name="QueueNode"> Node to store characters and other info </typeparam>
        /// <remark> isSecond related code is commented out for now due to uncertainty of queueing system </remark>
        private class QueueNode : System.IEquatable<QueueNode>{
            
            public Character c { get; set; }        /// <value> Character (PartyMember or Monster) </value>
            //public bool isSecond { get; set; }      /// <value> Boolean for if the character is the second entry of another character with same ID </value>
            public int priority { get; set; }       /// <value> Priority to order characters by </value>

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="c"> Character to encapsulate </param>
            public QueueNode(Character c) {//, bool isSecond) {
                this.c = c;
                //this.isSecond = isSecond;
                //if (!isSecond) {
                priority = c.DEX;       // For now, priority is based on a character's DEX
                // }
                // else {
                //     priority = c.DEX / 2;
                // }
            }

            /// <summary>
            /// Copy constructor
            /// </summary>
            /// <param name="q"> QueueNode to copy </param>
            public QueueNode(QueueNode q) {
                this.c = q.c;
                //this.isSecond = q.isSecond;
                this.priority = q.priority;
            }

            /// <summary>
            /// Determines if two QueueNodes are equal
            /// </summary>
            /// <param name="q"> Other QueueNode to compare against </param>
            /// <returns> Returns true if queueNodes have the same ID, false otherwise </returns>
            public bool Equals(QueueNode q) {
                if (q == null) { // don't have to compare at run time as type doesn't change
                    return false;
                }
                
                if (q.c.ID == c.ID) {//&& q.isSecond == isSecond) {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Comparer to compare QueueNode objects based on their priority
        /// </summary>
        /// <typeparam name="QueueNode"></typeparam>
        private class QueueNodeComparer : Comparer<QueueNode> {

            /// <summary>
            /// Compares two QueueNodes, returning 1,0, or -1 depending on which has a higher value
            /// </summary>
            /// <param name="q1"> First QueueNode to compare </param>
            /// <param name="q2"> Second QueueNode to compare </param>
            /// <returns> Returns 1 if first node has higher priority, -1 if lower, and 0 if equal </returns>
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
}

