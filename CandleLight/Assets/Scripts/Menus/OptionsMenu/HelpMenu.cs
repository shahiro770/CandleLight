/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: January 23, 2019
* 
* The OptionsMenu class is used to modify aspects of the game.
*/

using PlayerUI;
using UIEffects;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.HelpMenu {

    public class HelpMenu : MonoBehaviour {

        /* external component references */
        public Tooltip burntt;
        public Tooltip poisontt;
        public Tooltip taunttt;
        public Tooltip freezett;
        public Tooltip ragett;
        public Tooltip bleedtt;
        public Tooltip weaknesstt;
        public Tooltip advantagett;
        public Tooltip roottt;
        public Tooltip shocktt;
        public Tooltip regeneratett;
        public Tooltip guardtt;
        public Tooltip curett;
        public Tooltip traptt;
        public Tooltip miraclett;
        public GridLayoutGroup glgStatusEffects;

        void Start() {
            burntt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            poisontt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            taunttt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            freezett.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            ragett.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            bleedtt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            weaknesstt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            advantagett.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            roottt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            shocktt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            regeneratett.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            guardtt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            curett.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            traptt.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x);
            miraclett.SetImageDisplayBackgroundWidth(glgStatusEffects.cellSize.x); 
        }
    }
}