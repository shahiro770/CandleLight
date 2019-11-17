/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The MonsterDisplay class is used to display information held inside
* a Monster class and handle visual interactions.
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
        public Animator burnAnimator;
        public Animator poisonAnimator;
        public Animator monsterAnimator;    /// <value> Animator for monster's sprite </value>
        public Animator SEAnimator;
        public Bar HPBar;                   /// <value> Monster's health points display </value>
        public Button b;                    /// <value> Button to make monster selectable </value>
        public ButtonTransitionState bts;   /// <value> Button's visual state controller </value>
        public Canvas monsterCanvas;        /// <value> Monster's personal canvas to display UI elements and minimize repainting </value>
        public DamageText dt;               /// <value> Text display to show how much damage taken by an attack </value>we
        public GameObject SEDisplayPrefab;
        public GameObject SEHolder;
        public RectTransform monsterSpriteHolder;   /// <value> Holds monster's sprite and button, resized to prevent animations from repositioning </value>
        public SpriteRenderer monsterSprite;        /// <value> Sprite of monster </value>
        public Tooltip t;                   /// <value> Tooltip holding monster information </value>
        
        [field: SerializeField] public Vector2 vectorSize { get; private set; }         /// <value> Size of monster's sprite </value>
        [field: SerializeField] public float spriteWidth { get; private set; }          /// <value> Width of sprite rect transform </value>
        
        [field: SerializeField] private Vector2 buttonVectorSize;       /// <value> Size of monster's sprite </value>
        [field: SerializeField] private Monster displayedMonster;       /// <value> </value>
        [field: SerializeField] private float healthBarHeight = 18;     /// <value> Default width of a monster's health bar </value>
        [field: SerializeField] private float healthBarWidthSmall = 115;/// <value> Default width of a monster's health bar </value>
        [field: SerializeField] private float healthBarWidthMed = 150;  /// <value> Default width of a monster's health bar </value>
        [field: SerializeField] private float healthBarWidthLarge = 230;/// <value> Default width of a monster's health bar </value>
        [field: SerializeField] private bool isCrit = false;            /// <value> Flag for if damage text should show a critical hit </value>

        #region [ Initialization ] Initialization

        /// <summary>
        /// Initializes monster display with the monster's information
        /// </summary>
        /// <param name="displayedMonster"> Monster object </param>
        /// <returns> Ienumerator so combat doesn't start until all monsters have spawned </returns>
        public IEnumerator Init(Monster displayedMonster) {          
            this.displayedMonster = displayedMonster;
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
            
            monsterSprite.sprite = Resources.Load<Sprite>(spritePath);

            SetSize();
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
                HPBar.SetMaxAndCurrent(displayedMonster.HP, displayedMonster.CHP, new Vector2(healthBarWidthSmall, healthBarHeight));
            } 
            else if (monsterSize == "medium") {
                HPBar.SetMaxAndCurrent(displayedMonster.HP, displayedMonster.CHP, new Vector2(healthBarWidthMed, healthBarHeight));
            }
             else if (monsterSize == "large") {
                HPBar.SetMaxAndCurrent(displayedMonster.HP, displayedMonster.CHP, new Vector2(healthBarWidthLarge, healthBarHeight));
            }
        }

        public void SetSEHolderSize() {
            RectTransform rt = SEHolder.GetComponent<RectTransform>();
            string monsterSize = displayedMonster.monsterSize;

            if (monsterSize == "small" || monsterSize == "extraSmall") {
                rt.sizeDelta = new Vector2(healthBarWidthSmall, rt.sizeDelta.y);
            } 
            else if (monsterSize == "medium") {
                rt.sizeDelta = new Vector2(healthBarWidthMed, rt.sizeDelta.y);
            }
             else if (monsterSize == "large") {
                rt.sizeDelta = new Vector2(healthBarWidthLarge, rt.sizeDelta.y);
            }
        }

        /// <summary>
        /// Sets the size of monster's rect 
        /// </summary>
        private void SetSize() {
            RectTransform monsterRect = monsterSpriteHolder.GetComponent<RectTransform>();
            RectTransform buttonRect = b.GetComponent<RectTransform>();
            string monsterSize = displayedMonster.monsterSize;
            
            if (monsterSize == "small") {
                vectorSize = new Vector2(128, 128);
                buttonVectorSize = new Vector2(160, 160);
            } 
            else if (monsterSize == "medium") {
                vectorSize = new Vector2(192, 192);
                buttonVectorSize = new Vector2(192, 192);
            }
             else if (monsterSize == "large") {
                vectorSize = new Vector2(192, 192);
                buttonVectorSize = new Vector2(234, 192);
            }

            monsterRect.sizeDelta = vectorSize;
            buttonRect.sizeDelta = buttonVectorSize;
            monsterRect.anchoredPosition = new Vector3(0, (vectorSize.y - 192) / 2 + 8);
            buttonRect.anchoredPosition = new Vector3(0, buttonVectorSize.y / 2);
            spriteWidth = vectorSize.x;
            SetSEHolderSize();
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

        /// <summary>
        /// Sets the tooltip to display the monster's information
        /// </summary>
        public void SetTooltip() {
            RectTransform buttonRect = b.GetComponent<RectTransform>();
            t.SetImageDisplayBackgroundWidth(buttonRect.sizeDelta.x);

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

        #region [ Section 2 ] Animation

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
        /// Sets the isCrit flag to true
        /// </summary>
        public void SetCrit() {
            isCrit = true;
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
         /// <param name="selectedMonsterAttackIndex"> Index of animation trigger </param>
        /// <returns> IEnumerator, waiting for the animation to finish </returns>
        public IEnumerator PlayAttackAnimation(int selectedMonsterAttackIndex) {
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "attack" + selectedMonsterAttackIndex)));
        }

        /// <summary>
        /// Plays the burn animation of a monster
        /// </summary>
        public void PlayBurnAnimation() {
            StartCoroutine(PlayAnimation(burnAnimator, "statusEffected"));
        }

        /// <summary>
        /// Plays the poisonn animation of a monster
        /// </summary>
        public void PlayPoisonAnimation() {
            StartCoroutine(PlayAnimation(poisonAnimator, "statusEffected"));
        }

         /// <summary>
        /// Plays the death animation of a monster
        /// </summary>
        /// <returns> IEnumerator, waiting for the animation to finish </returns>
        public IEnumerator PlayDeathAnimation() {
            StartCoroutine(PlayAnimation(SEAnimator, "death"));
            StartCoroutine(PlayAnimation(HPBar.barAnimator, "death"));
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "death")));
        }

        /// <summary>
        /// Plays the spawn animation of a monster
        /// </summary>
        /// <returns> IEnumerator, waiting for the animation to finish </returns>
        public IEnumerator PlaySpawnAnimation() {
            StartCoroutine(PlayAnimation(HPBar.barAnimator, "spawn"));
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "spawn")));
        }

        #endregion

        #region [ Section 3] Combat Information

        /// <summary>
        /// Displays a change in HP
        /// </summary>
        /// <param name="amount"> Positive int amount changed </param>
        /// <param name="isLoss"> True if health is lost, positive if gained </param>
        /// <param name="animationClipName"> Name of animation to play overtop of monster </param>
        /// <returns> IEnumerator for animations </returns>
        public IEnumerator DisplayHPChange(int amount, bool isLoss, string animationClipName) {
            SetEffectsAnimatorClip(animationClipName);
            if (isLoss) {
                yield return (StartCoroutine(PlayAnimation(effectsAnimator, "attacked")));
                dt.ShowDamage(amount);
                HPBar.SetCurrent(displayedMonster.CHP);
                if (isCrit) {
                    yield return (StartCoroutine(PlayTwoAnimations(monsterAnimator, dt.textAnimator, "damagedCrit", "showCritDamage")));
                    isCrit = false;
                }
                else {
                    yield return (StartCoroutine(PlayTwoAnimations(monsterAnimator, dt.textAnimator, "damaged", "showDamage")));
                }
                dt.HideDamage();
            }
        }

        /// <summary>
        /// Plays animation of when monster is attacked with no damage numbers or hp bar change.
        /// Used for buffs and debuffs.
        /// </summary>
        /// <param name="animationClipName"></param>
        /// <returns></returns>
        public IEnumerator DisplayAttackEffect(string animationClipName) {
            SetEffectsAnimatorClip(animationClipName);
            yield return (StartCoroutine(PlayAnimation(effectsAnimator, "attacked")));
        }

        /// <summary>
        /// Plays animation for when attack is dodged
        /// </summary>
        /// <param name="animationClipName"> Name of animation to play overtop of monster </param>
        /// <returns> IEnumerator for animations</returns>
        public IEnumerator DisplayAttackDodged(string animationClipName) {
            SetEffectsAnimatorClip(animationClipName);
            yield return (StartCoroutine(PlayAnimation(effectsAnimator, "attacked")));
            dt.ShowDodged();
            yield return (StartCoroutine(PlayAnimation(dt.textAnimator, "showDamage")));
            dt.HideDamage();
        }

        /// <summary>
        /// Adds a status effect display to the status effect holder
        /// </summary>
        /// <param name="se"></param>
        public void AddStatusEffectDisplay(StatusEffect se) {
            GameObject newSED = Instantiate(SEDisplayPrefab);
            newSED.GetComponent<StatusEffectDisplay>().Init(se);

            newSED.transform.SetParent(SEHolder.transform, false);
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
