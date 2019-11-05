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
using Localization;
using PanelConstants = Constants.PanelConstants;
using System.Collections;
using System.Collections.Generic;
using UIEffects;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class PartyMemberDisplay : MonoBehaviour {
        
        /* external component references */
        public Animator pmDisplayAnimator;  /// <value> Animator for display motion </value>
        public Animator pmEffectsAnimator;  /// <value> Animator for effects that display over the pmd </value>
        public Image classIcon;         /// <value> Icon of class </value>
        public Image LVLBackground;     /// <value> Background to where level text is displayed </value>
        public Bar HPBar;               /// <value> Visual for health points </value>
        public Bar MPBar;               /// <value> Visual for mana points </value>
        public EXPBar EXPBar;           /// <value> Visual for experience points, should rename this </value>
        public Button b;                /// <value> Button to make display clickable for more info </value>
        public ButtonTransitionState bts;
        public LocalizedText LVLText;   /// <value> Text displaying current level </value>
        public StatsPanel statsPanel;   /// <value> Panel for stats </value>

        private PartyMemberVisualController pmvc;       /// <value> PartyMember the display is referring to <value>

        /// <summary>
        /// Displays the class, health, and mana of a partyMember
        /// </summary>
        /// <param name="pm"></param>
        public void Init(PartyMemberVisualController pmvc) {
            this.pmvc = pmvc;
            classIcon.sprite = pmvc.partyMemberSprite;
            pmvc.SetPartyMemberDisplay(this, PanelConstants.PARTYPANEL, HPBar, MPBar);

            ColorBlock normalBlock = b.colors; 
            ColorBlock activeBlock = b.colors;
            
            activeBlock.normalColor = new Color32(255, 255, 255, 255);
            activeBlock.highlightedColor = new Color32(255, 255, 255, 200);
            activeBlock.pressedColor = new Color32(255, 255, 255, 255);
            activeBlock.disabledColor = new Color32(255, 255, 255, 255);
            
            bts.SetColorBlock("normalAlternate", activeBlock);
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
            SetInteractable(false);
        }

        /// <summary>
        /// Toggles the stats panel open or close
        /// Also sets the active partyMember to whoever was clicked/selected
        /// </summary>
        public void ToggleStatsPanel() {
            if (statsPanel.pmvc == this.pmvc || statsPanel.pmvc == null) {
                statsPanel.gameObject.SetActive(!statsPanel.isOpen);
            }
            pmvc.SetActivePartyMember();
            
            if (statsPanel.isOpen == true) {
                UpdateStatsPanel();
            }
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
        }

        /// <summary>
        /// Sets the bts to show the partyMember that is active
        /// </summary>
        public void ShowActive() {
            bts.SetColor("normalAlternate");
        }
        
        /// <summary>
        /// Sets navigation from PartyMemberDisplay's button to another button
        /// </summary>
        /// <param name="direction"> Direction button pressed to navigate </param>
        /// <param name="b2"> Other button </param>
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
            pmDisplayAnimator.SetTrigger("dodged");
            do {
                yield return null;    
            } while (pmDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmDisplayAnimator.ResetTrigger("dodged");
        }

        /// <summary>
        /// Plays the animation for when the partyMember dodges an attack
        /// </summary>
        /// <returns> Waits for animation to finish playing </returns>
        public IEnumerator PlayStatusEffectAnimations(List<string> animationClipNames) {
            // for (int i = 0; i < animationClipNames.Count; i++) {
            //      SetEffectsAnimatorClip(animationClipName, i);
               
            // }
             SetEffectsAnimatorClip(animationClipNames[0]);
            pmEffectsAnimator.SetTrigger("statusEffected");
            do {
                yield return null;    
            } while (pmDisplayAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Idle") == false);
            pmDisplayAnimator.ResetTrigger("statusEffected");
        }

        /// <summary>
        /// Sets the interactivity of the action's button, and handles consequences
        /// </summary>
        /// <param name="value"> Enable interactivity on true and disable on false </param>
         public void SetInteractable(bool value) {
            b.interactable = value;
            classIcon.raycastTarget = value;
        }
    }
}
