/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store and manipulate information about the Monster. 
* It is always attached to a Monster gameObject.
*
*/

using Combat;
using General;
using PlayerUI;
using System.Collections;
using System.Collections.Generic;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace Characters {

    public class MonsterDisplay : MonoBehaviour {
        
        /* external component references */
        public Animator effectsAnimator;    /// <value> Animator for all effects played over-top of monster </value>
        public Animator monsterAnimator;    /// <value> Animator for monster's sprite </value>
        public Bar HPBar;                   /// <value> Monster's health points display </value>
        public Button b;                    /// <value> Button to make monster selectable </value>
        public ButtonTransitionState bts;   /// <value> Button's visual state controller </value>
        public Canvas monsterCanvas;        /// <value> Monster's personal canvas to display UI elements and minimize repainting </value>
        public DamageText dt;               /// <value> Text display to show how much damage taken by an attack </value>
        public Image monsterImage;          /// <value> Monster's sprite </value>
        public RectTransform monsterSpriteHolder;       /// <value> Holds monster's sprite and button, resized to prevent animations from repositioning </value>
        public Tooltip t;

        [field: SerializeField] private Monster displayedMonster;
        [field: SerializeField] public Vector2 vectorSize { get; private set; }         /// <value> Size of monster's sprite </value>
        [field: SerializeField] public float spriteWidth { get; private set; }          /// <value> Width of sprite rect transform </value>

        #region [ Initialization ] Initialization

        public IEnumerator Init(Monster m) {          
            // when monster is an adjacent selected by a multi-scoping attack, give it a different button colour
            ColorBlock monsterAltSelectColorBlock = b.colors;  
            monsterAltSelectColorBlock.normalColor = new Color32(255, 255, 255, 200);
            monsterAltSelectColorBlock.highlightedColor = monsterAltSelectColorBlock.normalColor;
            monsterAltSelectColorBlock.pressedColor = monsterAltSelectColorBlock.normalColor;
            bts.SetColorBlock("normalAlternate", monsterAltSelectColorBlock);

            /* WHEN WORKING WITH WIFI AND/OR WANT TO USE ASSETBUNDLES */

            //CoroutineWithData cd = new CoroutineWithData(this, AssetBundleManager.instance.LoadSpriteAssetFromBundle("monster", monsterSpriteName));
            //yield return cd.coroutine;
            //monsterSprite.sprite = (Sprite)cd.result;

            /**********************************************************/

            /* WHEN NO WIFI */

            string spritePath = "Sprites/Monsters/" + displayedMonster.monsterArea + "/" +  displayedMonster.monsterSpriteName; 

            /****************/
            
            monsterImage.sprite = Resources.Load<Sprite>(spritePath);

            RectTransform monsterRect = monsterSpriteHolder.GetComponent<RectTransform>();
            spriteWidth = monsterRect.rect.width;

            SetHealthBar();
            SetTooltip();
            SetMonsterAnimatorClips();
            SetCamera(); 

            yield break;
        }

        /// <summary>
        /// Sets the health bar size of the monster
        /// Currently used due to monster's health bar being inaccurate upon cloning
        /// </summary>
        public void SetHealthBar() {
            string monsterSize = displayedMonster.monsterSize;

            if (monsterSize == "small" || monsterSize == "extraSmall") {
                HPBar.SetMaxAndCurrent(displayedMonster.HP, displayedMonster.CHP, new Vector2(115, 18));
            } 
            else if (monsterSize == "medium") {
                HPBar.SetMaxAndCurrent(displayedMonster.HP, displayedMonster.CHP, new Vector2(150, 18));
            }
             else if (monsterSize == "large") {
                HPBar.SetMaxAndCurrent(displayedMonster.HP, displayedMonster.CHP, new Vector2(230, 18));
            }
        }

        #endregion

        #region [ Section 0 ] Button Interaction

        /// <summary>
        /// Set button's onClick function to the passed in function
        /// </summary>
        /// <param name="smd"> Delegate function only takes in a monster as a parameter </param>
        /// <remark> 
        /// When a monster is instantiated, it does not contain the logic or info that the combatManager
        /// uses to determine if its being attacked.false By passing it an onClick from the combatManager,
        /// its functionality can be simplified.
        /// </remark>
        public void AddSMDListener(SelectMonsterDelegate smd) {
            b.onClick.AddListener(() => smd(displayedMonster));
        }

        /// <summary>
        /// Visually select monster with pressed colour
        /// </summary>
        public void SelectMonsterButton() {
            bts.SetColor("pressed");
        }

        /// <summary>
        /// Visually select monster with the "adjacent hovered" (registered as normalAlternate) colour
        /// </summary>
        public void SelectMonsterButtonAdjacent() {
            bts.SetColor("normalAlternate");
        }

        /// <summary>
        /// Visually deselect monster
        /// </summary>
        public void DeselectMonsterButton() {
            t.SetVisible(false);
            bts.SetColor("normal");
        }

        /// <summary>
        /// Sets the interactivity of the action's button, and handles consequences
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
        public void SetInteractable(bool value) {
            b.gameObject.SetActive(value);
            t.SetVisible(false);

            if (value == false) {
                bts.SetColor("disabled");
            }
            else {
                bts.SetColor("normal");
            }
        }


        public void SetTooltip() {
            t.SetImageDisplayBackgroundWidth(spriteWidth);

            t.SetKey("title", displayedMonster.monsterDisplayName + "_monster");
            t.SetAmountText("subtitle", "LVL_label", displayedMonster.LVL);
            t.SetKey("description", displayedMonster.monsterDisplayName + "_monster_description");   
        }

        #endregion

        #region [ Section 1 ] Button Navigation

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

        #endregion

        #region Animation

        /// <summary>
        /// Sets all monster attack clips in monster animator controller.
        /// </summary>
        /// <remark> 
        /// In Unity, the only way to load in different animations is to create an AnimatorOverrideController, 
        /// set the animations of the AOC, and then set it to be the runtimeAnimatorController of the given animator. 
        /// Overriding a clip in unity overrides all instances of a clip in an animator controller's
        /// state machine. Make sure to use individual placeholders for each clip you want overridden.
        /// </remark>
        public void SetMonsterAnimatorClips() {
            AnimatorOverrideController aoc = new AnimatorOverrideController(monsterAnimator.runtimeAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>>(); // first clip is old clip to override, second is new clip

            for (int i = 0; i < displayedMonster.attackNum; i++) {
                AnimationClip oldClip = aoc.animationClips[i];
                string animationClipPath = "AnimationsAndControllers/Animations/" + displayedMonster.attacks[i].animationClipName;
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
        /// Plays an animation
        /// </summary>
        /// <param name="a"> Name of animator (effectsAnimator, monsterAnimator, etc.) </param>
        /// <param name="trigger"> Animation trigger </param>
        /// <returns> Stops all actions while monster's animation plays </returns>
        /// <remark> 
        /// All animators assumed to have "Idle" as their default state, and
        /// Base Layer as the name of their base animator layer. 
        /// </remark>
        public IEnumerator PlayAnimation(Animator a, string trigger) {
            a.SetTrigger(trigger);
            do {
                yield return null;    
            } while (a.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            a.ResetTrigger(trigger); // Reset the trigger just in case
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
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "attack" + displayedMonster.selectedAttackIndex)));
        }

         /// <summary>
        /// Plays the death animation of a monster
        /// </summary>
        /// <returns> IEnumerator, waiting for the animation to finish </returns>
        public IEnumerator PlayDeathAnimation() {
            yield return (StartCoroutine(PlayTwoAnimations(monsterAnimator, HPBar.barAnimator, "death", "death")));
        }

        /// <summary>
        /// Plays the spawn animation of a monster
        /// </summary>
        /// <returns> IEnumerator, waiting for the animation to finish </returns>
        public IEnumerator PlaySpawnAnimation() {
            yield return (StartCoroutine(PlayTwoAnimations(monsterAnimator, HPBar.barAnimator, "spawn", "spawn")));
        }

        #endregion

        #region Combat Information

        public IEnumerator DisplayLostHP(int amount, string animationClipName) {
            SetEffectsAnimatorClip(animationClipName);
            yield return (StartCoroutine(PlayAnimation(effectsAnimator, "attacked")));
            dt.ShowDamage(amount);
            HPBar.SetCurrent(displayedMonster.CHP);
            yield return (StartCoroutine(PlayTwoAnimations(monsterAnimator, dt.textAnimator, "damaged", "showDamage")));
            dt.HideDamage();
        }

        #endregion

        /// <summary>
        /// Sets the main camera
        /// </summary>
        /// <remark> This might not be needed </remark>
        public void SetCamera() {
            monsterCanvas.worldCamera = GameManager.instance.mainCamera;
        }
    }
}
