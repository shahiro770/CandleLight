/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store information about the Monster. 
* It is always attached to a Monster gameObject.
*
*/

using AssetManagers;
using Combat;
using General;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UIEffects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Characters {

    public class Monster : Character {
        
        public Animator effectsAnimator;    /// <value> Animator for all effects played over-top of monster </value>
        public Animator monsterAnimator;    /// <value> Animator for monster's sprite </value>
        public Bar HPBar;                   /// <value> Monster's health points display </value>
        public Button b;                    /// <value> Button to make monster selectable </value>
        public ButtonTransitionState bts;   /// <value> Button's visual state controller </value>
        public Canvas monsterCanvas;        /// <value> Monster's personal canvas to display UI elements and minimize repainting </value>
        public DamageText dt;               /// <value> Text display to show how much damage taken by an attack </value>
        public Image monsterSprite;         /// <value> Monster's sprite </value>
        public RectTransform monsterSpriteHolder;       /// <value> Holds monster's sprite and button, resized to prevent animations from repositioning </value>
        public Vector2 vectorSize;          /// <value> Size of monster's sprite </value>
        public int attackNum = 0;           /// <value> Number of attacks monster has (max 4) </value>
        public int selectedAttackIndex;     /// <value> Index of attack selected </value>
        
        [field: SerializeField] public string monsterArea { get; private set; }             /// <value> Area where monster can be found </value>
        [field: SerializeField] public string monsterSize { get; private set; }             /// <value> String constant describing size of monster's sprite </value>
        [field: SerializeField] public string monsterNameID { get; private set; }           /// <value> NameID as referenced in database </value>
        [field: SerializeField] public string monsterDisplayName { get; private set; }      /// <value> Monster name <value>
        [field: SerializeField] public string monsterAI { get; private set; }               /// <value> Monster's behaviour </value>
        [field: SerializeField] public float spriteWidth { get; private set; }              /// <value> Width of sprite rect transform </value>
        [field: SerializeField] public bool isReady { get; private set; } = false;          /// <value> Monster finished loading </value>
        
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
        public IEnumerator Init(string monsterNameID, string monsterSpriteName, string monsterDisplayName, string monsterArea, string monsterSize, string monsterAI, int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
            base.Init(LVL, HP, MP, stats, attacks);            
            bts = b.GetComponent<ButtonTransitionState>();
            this.monsterNameID = monsterNameID;
            this.monsterDisplayName = monsterDisplayName;
            this.monsterArea = monsterArea;
            this.monsterAI = monsterAI;

            /* WHEN WORKING WITH WIFI AND/OR WANT TO USE ASSETBUNDLES */

            //CoroutineWithData cd = new CoroutineWithData(this, AssetBundleManager.instance.LoadSpriteAssetFromBundle("monster", monsterSpriteName));
            //yield return cd.coroutine;
            //monsterSprite.sprite = (Sprite)cd.result;

            /**********************************************************/

            /* WHEN NO WIFI */

            string spritePath = "Sprites/Enemies/" + monsterArea + "/" +  monsterSpriteName; 

            /****************/

            monsterSprite.sprite = Resources.Load<Sprite>(spritePath);  // sprite path will always be inside resources folder
            this.monsterSize = monsterSize;
            SetSize(monsterSize);  
            SetHealthBar();

            foreach (Attack a in attacks) {
                if (a.name != "none") {
                    attackNum++;
                }
            }

            SetCamera(); 
            SetMonsterAnimatorClips();
            
            this.isReady = true;

            yield break;
        }

        /// <summary>
        /// Sets the health bar size of the monster
        /// Currently used due to monster's health bar being inaccurate upon cloning
        /// </summary>
        public void SetHealthBar() {
            if (monsterSize == "small") {
                HPBar.SetMaxAndCurrent(HP, CHP, new Vector2(100, 100));
            } 
            else if (monsterSize == "medium") {
                HPBar.SetMaxAndCurrent(HP, CHP, new Vector2(150, 150));
            }
             else if (monsterSize == "large") {
                HPBar.SetMaxAndCurrent(HP, CHP, vectorSize);
            }
            
        }

        /// <summary>
        /// Sets the sprite and canvas size of the monster, and UI elements 
        /// such as the health point bar.
        /// The Monster's sprite is repositioned depending on its size.
        /// Need to make this account for if the monster is floating and etc.
        /// </summary>
        /// <param name="monsterSize"> Size of the monster (small, medium, large) </param>
        /// <remark> Monster's image is repositioned depending on its sprite size </remark>
        private void SetSize(string monsterSize) {
            RectTransform monsterRect = monsterSpriteHolder.GetComponent<RectTransform>();
            if (monsterSize == "small") {
                vectorSize = new Vector2(170, 170);
                monsterRect.sizeDelta = vectorSize;
                monsterRect.anchoredPosition = new Vector3(0, -15); // (170 - 200) / 2
            } else if (monsterSize == "medium") {
                vectorSize = new Vector2(200, 200);
                monsterRect.sizeDelta = vectorSize;
                monsterRect.anchoredPosition = new Vector3(0, 0);  // (200 - 200) / 2
            } else if (monsterSize == "large") {
                vectorSize = new Vector2(230, 230);
                monsterRect.sizeDelta = vectorSize;
                monsterRect.anchoredPosition = new Vector3(0, 15);  // (230 - 200) / 2
            }

            spriteWidth = monsterRect.rect.width;
        }
        
        /// <summary>
        /// Sets all monster attack clips in monster animator controller.
        /// </summary>
        /// <remark> 
        /// In Unity, the only way to load in different animations is to create an AnimatorOverrideController, 
        /// set the animations of the AOC, and then set it to be the runtimeAnimatorController of the given animator 
        /// Overriding a clip in unity overrides all instances of a clip in an animator controller's
        /// state machine. If you don't want to take a fat L, make sure to use individual 
        /// placeholders for each clip you want overridden.
        /// </remark>
        public void SetMonsterAnimatorClips() {
            AnimatorOverrideController aoc = new AnimatorOverrideController(monsterAnimator.runtimeAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>>(); // first clip is old clip to override, second is new clip

            for (int i = 0; i < attackNum; i++) {
                AnimationClip oldClip = aoc.animationClips[i];
                string animationClipPath = "AnimationsAndControllers/Animations/" + attacks[i].animationClipName;
                AnimationClip newClip = Resources.Load<AnimationClip>(animationClipPath);
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip> (oldClip, newClip));
            }
            
            aoc.ApplyOverrides(anims);
            monsterAnimator.runtimeAnimatorController = aoc;   
        }

        /// <summary>
        /// Sets the animation clip (.anim files cause animation and animationClip are two ****ing different things) 
        /// for the effects animator for the "attackedEffect" state. Used to show the animation of a partyMember's attack.
        /// </summary>
        /// <param name="animationClipName"> Name of animation clip to load </param>
        /// <remark> 
        /// In the future, will need to know which state is being 
        /// changed as a parameter when effects has more than 1 state 
        /// </remark>
        public void SetEffectsAnimatorClip(string animationClipName) {
            int attackedAnimationStateIndex = 0;    // constant index of the state in the animator that is triggered by the "attacked" trigger

            AnimatorOverrideController aoc = new AnimatorOverrideController(effectsAnimator.runtimeAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>>(); // first clip is old clip to override, second is new clip

            AnimationClip newClip = Resources.Load<AnimationClip>("AnimationsAndControllers/Animations/" + animationClipName);

            for (int i = 0; i < aoc.animationClips.Length; i++) {
                AnimationClip oldClip = aoc.animationClips[i];

                if (i == attackedAnimationStateIndex) {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip> (oldClip, newClip)); 
                } else {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip> (oldClip, oldClip));
                }   
            }
            
            aoc.ApplyOverrides(anims);
            effectsAnimator.runtimeAnimatorController = aoc;   
        }

        /// <summary>
        /// Set button's onclick function to the passed in function
        /// </summary>
        /// <param name="smd"> Delegate function only takes in a monster as a parameter </param>
        public void AddSMDListener(SelectMonsterDelegate smd) {
            b.onClick.AddListener(() => smd(this));
        }

        /// <summary>
        /// Visually select monster with pressed colour
        /// </summary>
        public void SelectMonsterButton() {
            bts.SetColor("pressed");
        }

        /// <summary>
        /// Visually deselect monster
        /// </summary>
        public void DeselectMonsterButton() {
            bts.SetColor("normal");
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
        /// <param name="direction"> direction input to navigate to b2 </param>
        /// <param name="b2"> Button to navigate to </param>
        public void SetNavigation(string direction, Button b2) {
            Navigation n = b.navigation;
            
            if (direction == "up") {
                n.selectOnUp = b2;
                b.navigation = n;
            }
            else if (direction == "right") {
                n.selectOnRight = b2;
                b.navigation = n;
            }
            else if (direction == "down") {
                n.selectOnDown = b2;
                b.navigation = n;
            }
            else if (direction == "left") {
                n.selectOnLeft = b2;
                b.navigation = n;
            }

            b.navigation = n;
        }

        /// <summary>
        /// Resets the horizontal navigation of monster's button
        /// </summary>
        public void ResetNavigation() {
             Navigation n = b.navigation;
             n.selectOnRight = null;
             n.selectOnLeft = null;

             b.navigation = n;
        }
        
        /// <summary>
        /// Returns the monster's selected attack based on its AI
        /// </summary>
        /// <returns> An Attack object to be used </returns>
        public Attack SelectAttack() {
            if (monsterAI == "random" || monsterAI == "weakHunter") {
                selectedAttackIndex = Random.Range(0, attackNum);
            }

            return attacks[selectedAttackIndex];  
        }

        /// <summary>
        /// Plays the start turn animation of a monster
        /// </summary>
        /// <returns> IEnumerator, waiting for the animation to finish </returns>
        public IEnumerator PlayStartTurnAnimation() {
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "startTurn")));
        }
        

        /// <summary>
        /// Plays the attack animation of a monster
        /// </summary>
        /// <returns> IEnumerator, waiting for the animation to finish </returns>
        public IEnumerator PlayAttackAnimation() {
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "attack" + selectedAttackIndex)));
        }

        public IEnumerator PlayDeathAnimation() {
            yield return (StartCoroutine(PlayTwoAnimations(monsterAnimator, HPBar.barAnimator, "death", "death")));
        }

        /// <summary>
        /// Reduce monster's HP
        /// </summary>
        /// <param name="amount"> Amount of HP to lose, not negative </param>
        /// <param name="animationClipName"> Name of clip to play when monster is attacked </param>
        /// <returns> Starts coroutine of monster being attacked, before yielding control </returns>
        public IEnumerator LoseHP(int amount, string animationClipName) {
            CHP -= amount;
            if (CHP < 0) {
                CHP = 0;
            }
            
            SetEffectsAnimatorClip(animationClipName);
            yield return (StartCoroutine(PlayAnimation(effectsAnimator, "attacked")));
            dt.ShowDamage(amount);
            HPBar.SetCurrent(CHP);
            yield return (StartCoroutine(PlayTwoAnimations(monsterAnimator, dt.textAnimator, "damaged", "showDamage")));
            dt.HideDamage();
        }

        /// <summary>
        /// Destroys the monster
        /// </summary>
        /// <returns> Starts coroutine for monster death animation to play</returns>
        public IEnumerator Die() {
            yield return (StartCoroutine(PlayDeathAnimation()));
            Destroy(gameObject);
        }

        /// <summary>
        /// Plays an animation
        /// </summary>
        /// <param name="a"> Name of animator (effectsAnimator, monsterAnimator, etc.) </param>
        /// <param name="trigger"> Animation trigger </param>
        /// <returns> Stops all actions while monster's animation plays </returns>
        /// <remark> 
        /// All animators assumed to have Idle as their default state, and
        /// Base Layer as the name of their base animator layer. 
        /// </remark>
        public IEnumerator PlayAnimation(Animator a, string trigger) {
            a.SetTrigger(trigger);
            do {
                yield return null;    
            } while (a.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            a.ResetTrigger(trigger);
        }

        /// <summary>
        /// Plays two animations at the same time
        /// </summary>
        /// <param name="a1"> Name of animator (effectsAnimator, monsterAnimator, etc.) </param>
        /// <param name="a2"> Name of animator (effectsAnimator, monsterAnimator, etc.) </param>
        /// <param name="trigger1"> Animation trigger </param>
        /// <param name="trigger2"> Animation trigger </param>
        /// <returns> Stops all other actions while animations are playing </returns>
        /// <remark> 
        /// All animators assumed to have Idle as their default state, and
        /// Base Layer as the name of their base animator layer.
        /// Will change this to accept an array of animators and triggers if its needed in the future.
        /// </remark>
        public IEnumerator PlayTwoAnimations(Animator a1, Animator a2, string trigger1, string trigger2) {
            a1.SetTrigger(trigger1);
            a2.SetTrigger(trigger2);
            do {
                yield return null;    
            } while (a1.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false ||
                a2.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            a1.ResetTrigger(trigger1);
            a2.ResetTrigger(trigger2);
        }

        /// <summary>
        /// Check if monster is dead
        /// </summary>
        /// <returns> True if monster is dead, false otherwise</returns>
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
