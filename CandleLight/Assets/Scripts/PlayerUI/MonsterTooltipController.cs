/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: June 18, 2019
* 
* The MonsterTooltipController class is used to give a component with a button
* the ability to control when its tooltip displays. This is useful for Monster gameObjects
* where the button is not on the same component with the Monster script (which makes 
* tooltips unresponsive).
*
*/

using Monster = Characters.Monster;
using CombatManager = Combat.CombatManager;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PlayerUI {

    public class MonsterTooltipController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler {

        /* external component references */
        public Button b;
        public Monster m;
        public Tooltip t;

        public void OnPointerEnter(PointerEventData pointerEventData) {
            if (b.interactable) {
                t.SetVisible(true);
                CombatManager.instance.ShowAttackTargets(m);
            }
        }

        public void OnPointerExit(PointerEventData pointerEventData) {
            if (b.interactable) {
                t.SetVisible(false);
                CombatManager.instance.HideAttackTargets(m);
            }
        }

        public void OnSelect(BaseEventData baseEventData) {
            if (b.interactable) {
                t.SetVisible(true);
            }
        }

        public void OnDeselect(BaseEventData baseEventData) {
            if (b.interactable) {
                t.SetVisible(false);
            }
        }
    }
}

