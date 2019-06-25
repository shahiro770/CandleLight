using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayerUI;

namespace Events {

    public class EventManager : MonoBehaviour {

        public static EventManager instance;

        private Area currentArea;
        private SubArea currentSubArea;
        private Event currentEvent;
        private string currentAreaName;
        public float areaMultiplier { get; private set; }

        public EventDisplay[] eventDisplays = new EventDisplay[3];

        public EventDescription eventDescription;
        public Image eventBackground;
        public ActionsPanel actionsPanel;

        private Vector3 pos1d1 = new Vector3(0, 0, 0);
        private Vector3 pos2d1 = new Vector3(-150, 0, 0);
        private Vector3 pos2d2 = new Vector3(150, 0, 0);
        private Vector3 pos3d1 = new Vector3(-275, 0, 0);
        private Vector3 pos3d2 = new Vector3(0, 0, 0);
        private Vector3 pos3d3 = new Vector3(275, 0, 0);

        private bool isReady = false;

        /// <summary>
        /// Awake to instantiate singleton
        /// </summary>
        void Awake() {
            if (instance == null) {
                instance = this;
            }
            else if (instance != this) {
                DestroyImmediate (gameObject);
                instance = this;
            }
        }

        void Start() {
            //LoadArea("GreyWastes");
        
            //StartCoroutine(StartArea("GreyWastes"));
        }

        public void LoadArea(string areaName) {
            this.currentAreaName = areaName;
             Debug.Log("loading");
            currentArea = GameManager.instance.DB.GetAreaByName(areaName);
            Debug.Log("loaded");
            SetAreaMultiplier();

            isReady = true;
        }

        public IEnumerator StartArea(string areaName) {
            LoadArea(areaName);
            while (isReady == false) {
                yield return null;
            }
            Debug.Log("we good");
            GetStartEvent();
        }

        public void GetStartEvent() {
            currentSubArea = currentArea.GetSubArea(1);
            currentEvent = currentSubArea.GetEvent(0);

            DisplayCurrentEvent();
        }

        public void DisplayCurrentEvent() {
            eventBackground.sprite = currentEvent.GetBackground();
            eventDescription.SetText(currentEvent.promptKey);
            DisplayEventSprites(currentEvent);
        }

        private void SetAreaMultiplier() {
            switch(currentAreaName) {
                case "GreyWastes":
                    areaMultiplier = 1.0f;
                    break;
            }
        }

        public void DisplayEventSprites(Event e) {
            if (e.spriteNum != 0) {
                if (e.spriteNum == 1) {
                    eventDisplays[0].SetImage(e.eventSprites[0]);

                    eventDisplays[0].SetPosition(pos1d1);

                    eventDisplays[0].SetVisible(true);
                }
                else if (e.spriteNum == 2) {
                    eventDisplays[0].SetImage(e.eventSprites[0]);
                    eventDisplays[1].SetImage(e.eventSprites[1]);

                    eventDisplays[0].SetPosition(pos2d1);
                    eventDisplays[1].SetPosition(pos2d2);

                    eventDisplays[0].SetVisible(true);
                    eventDisplays[1].SetVisible(true);
                }
                else {
                    eventDisplays[0].SetImage(e.eventSprites[0]);
                    eventDisplays[1].SetImage(e.eventSprites[1]);
                    eventDisplays[2].SetImage(e.eventSprites[2]);

                    eventDisplays[0].SetPosition(pos3d1);
                    eventDisplays[1].SetPosition(pos3d2);
                    eventDisplays[2].SetPosition(pos3d3);

                    eventDisplays[0].SetVisible(true);
                    eventDisplays[1].SetVisible(true);
                    eventDisplays[2].SetVisible(true);
                }
            }
        }

        public void HideEventSprites() {
            foreach (EventDisplay ed in eventDisplays) {
                ed.SetVisible(false);
            }
        }
    }
}
