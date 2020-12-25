/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 9, 2019
* 
* The PartyMemberdisplay class is used to display some relevant information about an individual partyMember.
* A PartyMemberDisplay is shown in the PartyPanel for each PartyMember.
*
*/

using Characters;
using GameManager = General.GameManager;
using Localization;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyMemberDisplay : MonoBehaviour {
        
        /* external component references */
        public Animator pmDisplayAnimator;  /// <value> Animator for display motion </value>
        public Animator pmEffectsAnimator;  /// <value> Animator for effects that display over the pmd </value>
        public Animator pmBurnAnimator;     /// <value> Animator for burns </value>
        public Animator pmBleedAnimator;    /// <value> Animator for bleeds </value>
        public Animator pmPoisonAnimator;   /// <value> Animator for poison </value>
        public Animator pmRegenerateAnimator;   /// <value> Animator for poison </value>
        public Animator pmFocusAnimator;        /// <value> Animator for focus </value>
        public Animator pmFrostBiteAnimator;    /// <value> Animator for frostbite </value>
        public Image LVLBackground;     /// <value> Background to where level text is displayed </value>
        public Image background;        /// <value> Background for entire pmd </value>
        public Bar HPBar;               /// <value> Visual for health points </value>
        public Bar MPBar;               /// <value> Visual for mana points </value>
        public GameObject SEDisplayPrefab;  /// <value> Status Effect display prefab </value>    
        public GameObject SEHolder;     /// <value> Holds all of the statusEffects on the partyMember </value>
        public EXPBar EXPBar;           /// <value> Visual for experience points, should rename this </value>
        public Button b;                /// <value> Button to make display clickable for more info </value>
        public ButtonTransitionState bts;
        public LocalizedText LVLText;           /// <value> Text displaying current level </value>
        public LocalizedText skillPointsText;   /// <value> Text displaying current skillPoints </value>
        public SpriteRenderer classIcon;        /// <value> Icon of class </value>
        public StatsPanel statsPanel;   /// <value> Panel for stats </value>

        private PartyMemberVisualController pmvc;       /// <value> PartyMember the display is referring to <value>
        private Color32 backgroundNormalColor = new Color32(0, 0, 0, 0);
        private Color32 backgroundSelectedColor;
        // private Coroutine lerper;                       /// <value> Store the coroutine responsible for changing background colour </value>
        // private float lerpSpeed = 25f;
        private bool selectForAttack = false;           /// <value> Flag for if partyMember can be selected for an attack target on the next click</value>

        public void Awake() {
            if (bts != null) {
                ColorBlock activeBlock = b.colors;
                
                activeBlock.normalColor = new Color32(255, 255, 255, 255);
                activeBlock.highlightedColor = new Color32(255, 255, 255, 200);
                activeBlock.pressedColor = new Color32(255, 255, 255, 255);
                activeBlock.disabledColor = new Color32(255, 255, 255, 255);
                
                bts.SetColorBlock("na0", activeBlock);
            }
        }

        /// <summary>
        /// Displays the class, health, and mana of a partyMember
        /// </summary>
        /// <param name="pm"></param>
        public void Init(PartyMemberVisualController pmvc) {
            if (pmvc.pmdPartyPanel == null) {
                this.pmvc = pmvc;
                classIcon.sprite = pmvc.partyMemberSprite;
                backgroundSelectedColor = new Color32(pmvc.partyMemberColour.r, pmvc.partyMemberColour.g, pmvc.partyMemberColour.b, 100);
                pmvc.SetPartyMemberDisplay(this, PanelConstants.PARTYPANEL, HPBar, MPBar);
            }
            else {
                pmvc.UpdateHPAndMPBar(PanelConstants.PARTYPANEL);
            }
        }

        /// <summary>
        /// Displays the class, level, and EXP of a partyMember
        /// </summary>
        /// <param name="pm"></param>
        public void InitRewardsDisplay(PartyMemberVisualController pmvc) {
            this.pmvc = pmvc;
     
            classIcon.sprite = pmvc.partyMemberSprite;
            LVLBackground.color = pmvc.partyMemberColour;
            
            pmvc.SetPartyMemberDisplayRewardsPanel(this, PanelConstants.REWARDSPANEL, EXPBar, LVLText);
            SetInteractable(false, true);
        }
        
        /// <summary>
        /// Displays the amount of skill pointsa partyMember has
        /// </summary>
        /// <param name="pmvc"></param>
        /// <param name="skillPoints"></param>
        public void InitSkillsDisplay(PartyMemberVisualController pmvc, int skillPoints) {
            if (pmvc.pmdSkillsPanel == null) {
                this.pmvc = pmvc;
                classIcon.sprite = pmvc.partyMemberSprite;
                backgroundSelectedColor = new Color32(pmvc.partyMemberColour.r, pmvc.partyMemberColour.g, pmvc.partyMemberColour.b, 100);
                pmvc.SetPartyMemberDisplaySkillsPanel(this);
            }

            UpdateSkillPointsText(skillPoints);
        }

        /// <summary>
        /// Toggles the stats panel open or close
        /// Also sets the active partyMember to whoever was clicked/selected (side effect shows active skills)
        /// </summary>
        public void ToggleStatsPanel() {
            if (statsPanel != null) {
                if (statsPanel.pmvc == this.pmvc || statsPanel.pmvc == null) {
                    statsPanel.gameObject.SetActive(!statsPanel.isOpen);
                }
                pmvc.SetActivePartyMember();
                if (statsPanel.isOpen == true) {
                    UpdateStatsPanel();
                }
            }
            else {
                pmvc.SetActivePartyMember();
            }
        }

        /// <summary>
        /// Handles logic that happens when the PMD is clicked
        /// </summary>
        public void OnClick() {
            if (selectForAttack == true) {
                pmvc.SelectPartyMember();
            }
            else {
                ToggleStatsPanel();
            }
        }

        /// <summary>
        /// Sets the active partyMember to the one this pmd shows
        /// </summary>
        public void DisplayPMSkills() {
            pmvc.SetActivePartyMember();
        }

        /// <summary>
        /// Updates the skill points text to show the right amount and colour for visual feedback
        /// </summary>
        /// <param name="amount"></param>
        public void UpdateSkillPointsText(int amount) {
            skillPointsText.SetText(amount.ToString());
            pmvc.UpdateSkillsTab();
        }

        public void AddStatusEffectDisplay(StatusEffect se) {
            GameObject newSED = Instantiate(SEDisplayPrefab);
            StatusEffectDisplay SEDComponent = newSED.GetComponent<StatusEffectDisplay>();
            SEDComponent.Init(se);
            SEDComponent.b.interactable = true;

            newSED.transform.SetParent(SEHolder.transform, false);
        }

        /// <summary>
        /// Updates the stats panel to show this partyMember's stats
        /// </summary>
        public void UpdateStatsPanel() {
            statsPanel.Init(pmvc);
        }

        /// <summary>
        /// Sets the bts to show the normal colour block
        /// </summary>
        public void ShowNormal() {
            bts.SetColor("normal");
            // if (gameObject.activeSelf == true && gameObject.activeInHierarchy) {
            //     if (lerper != null) {
            //         StopCoroutine(lerper);
            //     }
            //     lerper = StartCoroutine(LerpBackgroundColour(backgroundNormalColor));
            // }
            // else {
            background.color = backgroundNormalColor;
            // }
        }

        /// <summary>
        /// Sets the bts to show the partyMember that is active
        /// </summary>
        public void ShowActive() {
            bts.SetColor("na0");
            background.color = backgroundNormalColor;
        }

        /// <summary>
        /// Sets the bts and background to show the partyMember that is active in combat
        /// </summary>
        public void ShowActiveCombat() {
            bts.SetColor("na0");
            // if (gameObject.activeSelf == true && gameObject.activeInHierarchy) {
            //     if (lerper != null) {
            //         StopCoroutine(lerper);
            //     }
            //     lerper = StartCoroutine(LerpBackgroundColour(backgroundNormalColor));
            // }
            // else {
            background.color = backgroundSelectedColor;
            // }
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
            int SEAnimationStateIndex = 0;    // constant index of the state in the animator that is triggered by the "attacked" trigger
            // TODO find a way to change animationClips for higher layers.
            AnimatorOverrideController aoc = new AnimatorOverrideController(pmEffectsAnimator.runtimeAnimatorController);
            List<KeyValuePair<AnimationClip, AnimationClip>> anims = new List<KeyValuePair<AnimationClip, AnimationClip>>(); // first clip is old clip to override, second is new clip

            AnimationClip newClip = Resources.Load<AnimationClip>("AnimationsAndControllers/Animations/" + animationClipName);

            for (int i = 0; i < aoc.animationClips.Length; i++) {
                AnimationClip oldClip = aoc.animationClips[i];

                if (i == SEAnimationStateIndex) {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip> (oldClip, newClip)); 
                } else {
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip> (oldClip, oldClip));
                }   
            }
            
            aoc.ApplyOverrides(anims);
            pmEffectsAnimator.runtimeAnimatorController = aoc;   
        }

        /// <summary>
        /// Plays the animation for when the partyMember is damaged
        /// </summary>
        /// <returns> Waits for animation to finish playing </returns>
        public IEnumerator PlayDamagedAnimation() {
            pmDisplayAnimator.speed = GameManager.instance.gsDataCurrent.animationSpeed;
            pmDisplayAnimator.SetTrigger("damaged");
            do {
                yield return null;    
            } while (pmDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmDisplayAnimator.ResetTrigger("damaged");
        }

        /// <summary>
        /// Plays the animation for when the partyMember is damaged by a critical hit attack
        /// </summary>
        /// <returns> Waits for animation to finish playing </returns>
        public IEnumerator PlayCritDamagedAnimation() {
            pmDisplayAnimator.speed = GameManager.instance.gsDataCurrent.animationSpeed;
            pmDisplayAnimator.SetTrigger("damagedCrit");
            do {
                yield return null;    
            } while (pmDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmDisplayAnimator.ResetTrigger("damagedCrit");
        }

        /// <summary>
        /// Plays the animation for when the partyMember dodges an attack
        /// </summary>
        /// <returns> Waits for animation to finish playing </returns>
        public IEnumerator PlayDodgedAnimation() {
            pmDisplayAnimator.speed = GameManager.instance.gsDataCurrent.animationSpeed;
            pmDisplayAnimator.SetTrigger("dodged");
            do {
                yield return null;    
            } while (pmDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmDisplayAnimator.ResetTrigger("dodged");
        }

        /// <summary>
        /// Plays the animation for when a partyMember "spawns" (i.e. only for summons)
        /// </summary>
        /// <returns> Waits for animation to finish playing </returns>
        public IEnumerator PlaySpawnAnimation() {
            pmDisplayAnimator.speed = GameManager.instance.gsDataCurrent.animationSpeed;
            pmDisplayAnimator.SetTrigger("spawn");
            do {
                yield return null;    
            } while (pmDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmDisplayAnimator.ResetTrigger("spawn");
        }

        /// <summary>
        /// Plays the blinking animation indicating s
        /// </summary>
        /// <param name="value"> True to start blinking, false to stop </param>
        public void PlaySelectMeAnimation(bool value) {
            if (value == true) {
                pmDisplayAnimator.SetTrigger("blinking");
                pmDisplayAnimator.ResetTrigger("blinkingEnd");
                selectForAttack = true;
            }
            else if (value == false) {
                pmDisplayAnimator.SetTrigger("blinkingEnd");
                pmDisplayAnimator.ResetTrigger("blinking");
                selectForAttack = false;
            }
        }

        /// <summary>
        /// Plays the animation for when the partyMember has some effect occur on them
        /// </summary>
        /// <returns> Waits for animation to finish playing </returns>
        public IEnumerator PlayEffectAnimation(string animationClipName) {
            pmEffectsAnimator.speed = GameManager.instance.gsDataCurrent.animationSpeed;
            SetEffectsAnimatorClip(animationClipName);
            pmEffectsAnimator.SetTrigger("statusEffected");
            do {
                yield return null;    
            } while (pmEffectsAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmEffectsAnimator.ResetTrigger("statusEffected");
        }

        public IEnumerator PlaySEAnimation(Animator animator) {
            animator.speed = GameManager.instance.gsDataCurrent.animationSpeed;
            animator.SetTrigger("statusEffected");
            do {
                yield return null;    
            } while (animator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            animator.ResetTrigger("statusEffected");
        }

        /// <summary>
        /// Plays status effect animations depending on the partyMember's current status effects
        /// </summary>
        /// <param name="animationsToPlay"></param>
        public void PlayCleanUpStatusEffectAnimations(int[] animationsToPlay) {
            if (animationsToPlay[0] == 1) {
                StartCoroutine(PlaySEAnimation(pmBurnAnimator));
            }
            if (animationsToPlay[1] == 1) {
                StartCoroutine(PlaySEAnimation(pmPoisonAnimator));
            }
            if (animationsToPlay[2] == 1) {
                StartCoroutine(PlaySEAnimation(pmBleedAnimator));
            }
            if (animationsToPlay[3] == 1) {
                StartCoroutine(PlaySEAnimation(pmRegenerateAnimator));
            }
            if (animationsToPlay[4] == 1) {
                StartCoroutine(PlaySEAnimation(pmFocusAnimator));
            }
            if (animationsToPlay[5] == 1) {
                StartCoroutine(PlaySEAnimation(pmFrostBiteAnimator));
            }
        }

        /// <summary>
        /// Sets the interactivity of the partyMemberDisplay's button and statusEffectsDisplays
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
        public void SetInteractable(bool value, bool SEDvalue) {
            b.interactable = value;

            if (SEHolder != null) {     // partyMemberDisplay in rewards panel will have no SEHolder
                foreach (StatusEffectDisplay SED in SEHolder.GetComponentsInChildren<StatusEffectDisplay>()) {
                    SED.b.interactable = SEDvalue;
                    if (SED.t.gameObject.activeSelf == true) {
                        SED.t.SetVisible(SEDvalue);
                    }
                    else {
                        SED.t.SetVisible(false);
                    }
                }
            }
        }

        // public IEnumerator LerpBackgroundColour(Color targetColour) {
        //     float timeStartedLerping = Time.time;
        //     float timeSinceStarted = Time.time - timeStartedLerping;
        //     float percentageComplete = timeSinceStarted * lerpSpeed;
        //     Color prevColour = background.color;
            
        //     while (background.color != targetColour) {
        //         timeSinceStarted = Time.time - timeStartedLerping;
        //         percentageComplete = timeSinceStarted * lerpSpeed;

        //         background.color = Color.Lerp(prevColour, targetColour, percentageComplete);
        //         yield return new WaitForEndOfFrame();
        //     }
        // }
    }
}
