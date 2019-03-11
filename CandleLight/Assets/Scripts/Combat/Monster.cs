/*
* Project: CandleLight 
* Author: Shahir Chowdhury
* Date: February 11, 2019
* 
* The Monster class is used to store information about the Monster. 
*
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Combat {
    public class Monster : Character {
        
        public Canvas monsterCanvas;
        public Image monsterSprite;
        public Animator effectsAnimator;
        public Animator monsterAnimator;
        public Button b;
        public Bar HPBar;

        public delegate void ButtonListener(Monster monsterToSelect);
        public string monsterName { get; set; }

        private ButtonTransitionState bts;
        private bool selected = false;
        private int attackNum = 0;
        private Vector2 size;

        public void Init(string monsterName, string monsterArea, string monsterSize, int LVL, int HP, int MP, int[] stats, Attack[] attacks) {
            base.Init(LVL, HP, MP, stats, attacks);
            this.monsterName = monsterName;
            string spritePath = "Sprites/Enemies/" + monsterArea + "/" +  monsterName;
            monsterSprite.sprite = Resources.Load<Sprite>(spritePath);
            SetSize(monsterSize);   
            HPBar.Init(HP, CHP, size);
            bts = b.GetComponent<ButtonTransitionState>();

            foreach (Attack a in attacks) {
                if (a.name != "None") {
                    attackNum++;
                }
            }
        }

        private void SetSize(string monsterSize) {
            RectTransform monsterRect = monsterSprite.GetComponent<RectTransform>();
            RectTransform canvasRect = monsterCanvas.GetComponent<RectTransform>();
            if (monsterSize == "Small") {
                size = new Vector2(150, 150);
                monsterRect.sizeDelta = size;
                monsterRect.position = new Vector3(0, -25, 0); //(150 - 200) / 2
                canvasRect.sizeDelta = new Vector2(150, 200);
            } else if (monsterSize == "Medium") {
                size = new Vector2(200, 200);
                monsterRect.sizeDelta = size;
                canvasRect.sizeDelta = size;
                monsterRect.position = new Vector3(0, 0, 0);  // (200 - 200) / 2
            } else if (monsterSize == "Large") {
                size = new Vector2(250, 250);
                monsterRect.sizeDelta = size;
                monsterRect.position = new Vector3(0, 25, 0);  // (250 - 200) / 2
                canvasRect.sizeDelta = new Vector2(250, 200);
            }
        }

        public void AddSMDListener(SelectMonsterDelegate smd) {
            b.onClick.AddListener(() => smd(this));
        }

        public void SelectMonster() {
            bts.SetColor("pressed");
            selected = true;
        }

        public void DeselectMonster() {
            bts.SetColor("normal");
            selected = false;
        }

        public void EnableInteraction() {
            b.interactable = true;
            monsterSprite.raycastTarget = true;
        }

        public void DisableInteraction() {
            b.interactable = false;
            monsterSprite.raycastTarget = false;
        }

        public void SetNavigation(Button b2) {
            Navigation n = b.navigation;
            n.selectOnDown = b2;
            b.navigation = n;
        }

        public override IEnumerator LoseHP(int amount) {
            CHP -= amount;
            if (CHP < 0) {
                CHP = 0;
            }

            yield return StartCoroutine(PlayAnimation(effectsAnimator, "attacked"));
            HPBar.SetCurrent(CHP);
        }

        // need to put yielding logic higher up, combat manager should select attack targets
        public IEnumerator Attack(List<PartyMember> partyMembers) {
            int atkChoice = Random.Range(0, attackNum);
            int targetChoice = Random.Range(0, partyMembers.Count);
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "attack")));
            yield return (StartCoroutine(partyMembers[targetChoice].LoseHP(attacks[atkChoice].damage)));
        }

        public IEnumerator Die() {
            yield return (StartCoroutine(PlayAnimation(monsterAnimator, "death")));
            Destroy(gameObject);
        }

        public IEnumerator PlayAnimation(Animator a, string trigger) {
            a.SetTrigger(trigger);
            yield return null;      // wait a frame because animation transition takes a frame
            float waitTime = a.GetCurrentAnimatorClipInfo(0)[0].clip.length;
            yield return new WaitForSeconds(waitTime);
        }

        public bool CheckDeath() {
            return CHP == 0;
        }

        public override void LogStats() {
            Debug.Log(monsterName);
            base.LogStats();
        }

        public override void LogName() {
            Debug.Log("Name " + monsterName);
        }




        /* /// use this for initialization
        void Start() {
            monsterSprite.sprite = SO.monsterSprite;
            
            health = SO.health;
        }*/
    }
}
