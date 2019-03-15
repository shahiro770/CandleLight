/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store information about the Monster. 
* It is always attached to a Monster gameObject.
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Combat {
    public class Monster : Character {
        
        public Canvas monsterCanvas;        /// <value> Monster's personal canvas to display UI elements and minimize repainting </value>
        public Image monsterSprite;         /// <value> Monster's sprite </value>
        public RectTransform monsterSpriteHolder;       /// <value> Holds monster's sprite and button, resized to prevent animations from repositioning </value>
        public Animator effectsAnimator;    /// <value> Animator for all effects played overtop of monster </value>
        public Animator monsterAnimator;    /// <value> Animator for monster's sprite </value>
        public Button b;                    /// <value> button to make monster selectable </value>
        public Bar HPBar;                   /// <value> Monster's health points display </value>
        public string monsterDisplayName { get; private set; }  /// <value> Monster name <value>
        public string monsterAI { get; private set; }              /// <value> Monster' behaviour </value>

        private Vector2 size;               /// <value> Size of monster's sprite </value>
        private ButtonTransitionState bts;  /// <value> Button's visual state controller </value>
        private int attackNum = 0;          /// <value> Number of attacks monster has (max 4) </value>
        private bool selected = false;      /// <value> Flag for if monster is selected </value>
        
        /// <summary>
        /// Initializes the monster's properties and display
        /// </summary>
        /// <param name="monsterSpriteName"> Name of monster's sprite, castle case </param>
        /// <param name="monsterDisplayName"> Name of monster </param>
        /// <param name="monsterArea"> Area of monster to get file path to sprite, castle case </param>
        /// <param name="monsterSize"> Size of monster (small, medium, large) </param>
        /// <param name="monsterAI"> Pattern for how monster attacks </param>
        /// <param name="LVL"> Power level </param>
        /// <param name="HP"> Max health points </param>
        /// <param name="MP"> Max mana points </param>
        /// <param name="stats"> STR, DEX, INT, LUK </param>
        /// <param name="attacks"> List of known attacks (length 4) </param>
        public void Init(string monsterSpriteName, string monsterDisplayName, string monsterArea, string monsterSize, string monsterAI, int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
            base.Init(LVL, HP, MP, stats, attacks);
            
            bts = b.GetComponent<ButtonTransitionState>();
            this.monsterDisplayName = monsterDisplayName;
            this.monsterAI = monsterAI;

            string spritePath = "Sprites/Enemies/" + monsterArea + "/" +  monsterSpriteName; 
            monsterSprite.sprite = Resources.Load<Sprite>(spritePath);  // sprite path will always be inside resources folder
            
            SetSize(monsterSize);  
            HPBar.Init(HP, CHP, size);

            foreach (Attack a in attacks) {
                if (a.name != "none") {
                    attackNum++;
                }
            }

            SetCamera(); 
        }

        /// <summary>
        /// Sets the sprite and canvas size of the monster, and UI elements 
        /// such as the health point bar.
        /// The Monster's sprite is repositioned depending on its size.
        /// </summary>
        /// <param name="monsterSize"> Size of the monster (small, medium, large) </param>
        /// <remark> Monster's image is repositioned depending on its sprite size </remark>
        private void SetSize(string monsterSize) {
            RectTransform monsterRect = monsterSpriteHolder.GetComponent<RectTransform>();
            RectTransform canvasRect = monsterCanvas.GetComponent<RectTransform>();
            if (monsterSize == "small") {
                size = new Vector2(170, 170);
                monsterRect.sizeDelta = size;
                monsterRect.anchoredPosition = new Vector3(0, -15); // (170 - 200) / 2
            } else if (monsterSize == "medium") {
                size = new Vector2(200, 200);
                monsterRect.sizeDelta = size;
                monsterRect.anchoredPosition = new Vector3(0, 0);  // (200 - 200) / 2
            } else if (monsterSize == "large") {
                size = new Vector2(230, 230);
                monsterRect.sizeDelta = size;
                monsterRect.anchoredPosition = new Vector3(0, 15);  // (230 - 200) / 2
            }
        }

        /// <summary>
        /// Set button's onclick function to the passed in function
        /// </summary>
        /// <param name="smd"> Delegate function only takes in a monster as a parameter </param>
        public void AddSMDListener(SelectMonsterDelegate smd) {
            //Debug.Log("we adding");
            //Debug.Log(smd);
            b.onClick.AddListener(() => smd(this));
        }

        /// <summary>
        /// Visually select monster with pressed colour
        /// </summary>
        public void SelectMonsterButton() {
            bts.SetColor("pressed");
            selected = true;
        }

        /// <summary>
        /// Visually deselect monster
        /// </summary>
        public void DeselectMonsterButton() {
            bts.SetColor("normal");
            selected = false;
        }

        /// <summary>
        /// Enable the monster button
        /// </summary>
        public void EnableInteraction() {
            b.interactable = true;
            monsterSprite.raycastTarget = true;
        }

        /// <summary>
        /// Disable the monster button
        /// </summary>
        public void DisableInteraction() {
            b.interactable = false;
            monsterSprite.raycastTarget = false;
        }

        /// <summary>
        /// Allow navigation to the monster button
        /// </summary>
        /// <param name="b2"> Button to navigate to </param>
        public void SetNavigation(Button b2) {
            Navigation n = b.navigation;
            n.selectOnDown = b2;
            b.navigation = n;
        }

        // need to put yielding logic higher up, combat manager should select attack targets
        public IEnumerator Attack(List<PartyMember> partyMembers) {
            int atkChoice = Random.Range(0, attackNum);
            int targetChoice = Random.Range(0, partyMembers.Count);
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "attack")));
            yield return (StartCoroutine(partyMembers[targetChoice].LoseHP(attacks[atkChoice].damage)));
        }

        /// <summary>
        /// Reduce monster's HP
        /// </summary>
        /// <param name="amount"> Amount of HP to lose, not negative </param>
        /// <returns> Starts coroutine of monster being attacked, before yielding control </returns>
        public override IEnumerator LoseHP(int amount) {
            CHP -= amount;
            if (CHP < 0) {
                CHP = 0;
            }

            yield return StartCoroutine(PlayAnimation(effectsAnimator, "attacked"));
            HPBar.SetCurrent(CHP);
        }

        /// <summary>
        /// Destroys the monster
        /// </summary>
        /// <returns> Starts coroutine for monster death animation to play</returns>
        public IEnumerator Die() {
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "death")));
            Destroy(gameObject);
        }

        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="a"> Name of animator (effectsAnimator, monsterAnimator) </param>
        /// <param name="trigger"> Animation trigger </param>
        /// <returns> Stops all actions while monster's animation plays </returns>
        public IEnumerator PlayAnimation(Animator a, string trigger) {
            a.SetTrigger(trigger);
            yield return null;      // wait a frame because animation transition takes a frame
            float waitTime = a.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(waitTime);
        }

        /// <summary>
        /// Check if monster is dead
        /// </summary>
        /// <returns></returns>
        public bool CheckDeath() {
            return CHP == 0;
        }

        /// <summary>
        /// Sets the main camera
        /// </summary>
        /// <remark> This might not be needed </remark>
        public void SetCamera() {
            monsterCanvas.worldCamera = GameManager.instance.mainCamera;
        }

        /// <summary>
        /// Logs information for debugging
        /// </summary>
        public override void LogStats() {
            Debug.Log(monsterDisplayName);
            base.LogStats();
        }

        /// <summary>
        /// Logs name for debugging
        /// </summary>
        public override void LogName() {
            Debug.Log("Name " + monsterDisplayName);
        }
    }
}
