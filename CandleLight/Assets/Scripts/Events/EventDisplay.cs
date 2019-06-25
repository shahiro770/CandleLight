using General;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Events {

    public class EventDisplay : MonoBehaviour {

        public Image img;

        public void SetImage(Sprite spr) {
            img.sprite = spr;
        }

        public void SetVisible(bool value) {
            if (value == true) {
                gameObject.SetActive(true);
            }
            else {
                gameObject.SetActive(false);
            }
        }

        public void SetPosition(Vector3 pos) {
            gameObject.transform.localPosition = pos;
        }
    }
}