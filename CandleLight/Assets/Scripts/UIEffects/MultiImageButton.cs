/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 28, 2019
* 
* The MultiImagebutton class is used to make a button and all of its children components perform
* a visual transition at the same time (e.g. make both the button border and interior text change colour)
*
* Source: https://answers.unity.com/questions/820311/ugui-multi-image-button-transition.html by Riposte
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
namespace UIEffects {

    public class MultiImageButton : UnityEngine.UI.Button {
        
        private Graphic[] m_graphics;   /// <value> Backing field for Graphics </value>
        protected Graphic[] Graphics {  /// <value> List of graphic components that need to be affected by attached component's transitions </value>
            get {
                if(m_graphics == null) {
                    m_graphics = targetGraphic.transform.GetComponentsInChildren<Graphic>(); // Only child components will transition, not siblings
                }
                return m_graphics;
            }
        }

        /// <summary>
        /// Applies state transition to all selectable child graphic elements
        /// </summary>
        /// <param name="state"> State of the selectable (normal, highlighted, pressed, disabled) </param>
        /// <param name="instant"> Bool if the state change should be instanteous (no tweening) </param>
        protected override void DoStateTransition (SelectionState state, bool instant) {
            Color color;
            switch (state) {
                case Selectable.SelectionState.Normal:
                    color = this.colors.normalColor;
                    break;
                case Selectable.SelectionState.Highlighted:
                    color = this.colors.highlightedColor;
                    break;
                case Selectable.SelectionState.Pressed:
                    color = this.colors.pressedColor;
                    break;
                case Selectable.SelectionState.Disabled:
                    color = this.colors.disabledColor;
                    break;
                default:
                    color = Color.black;
                    break;
            }
            if (base.gameObject.activeInHierarchy) {    
                switch (this.transition) {
                    case Selectable.Transition.ColorTint:   //only tween if transition is colour tint
                        ColorTween (color * this.colors.colorMultiplier, instant);
                        break;
                    default:
                        throw new System.NotSupportedException();
                }
            }
        }

        /// <summary>
        /// Smoothly transistions between two colours
        /// </summary>
        /// <param name="targetColor"> Colour to change to </param>
        /// <param name="instant"> Flag for if change should be instanteous instead of gradual </param>
        private void ColorTween(Color targetColor, bool instant) {
            if (this.targetGraphic == null) {
                return;
            }

            foreach(Graphic g in this.Graphics) {
                g.CrossFadeColor (targetColor, (!instant) ? this.colors.fadeDuration : 0f, true, true);
            }
        }
    }
}