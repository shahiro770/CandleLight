using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTabManager : MonoBehaviour
{
    public GameObject InventoryP;
    public GameObject EquipmentP;
    public GameObject CandleP;

    void Start ()
    {
        InventoryP = GameObject.Find ("InventoryPanel");
        EquipmentP = GameObject.Find ("EquipmentPanel");
        CandleP = GameObject.Find ("CandlePanel");
        
        InventoryP.gameObject.SetActive (true);
        EquipmentP.gameObject.SetActive (false);
        CandleP.gameObject.SetActive (false);
    }

    public void InventoryTabSelect(){
        InventoryP.gameObject.SetActive (true);
        EquipmentP.gameObject.SetActive (false);
        CandleP.gameObject.SetActive (false);
    }

    public void EquipmentTabSelect(){
        InventoryP.gameObject.SetActive (false);
        EquipmentP.gameObject.SetActive (true);
        CandleP.gameObject.SetActive (false);
    }

    public void CandleTabSelect(){
        InventoryP.gameObject.SetActive (false);
        EquipmentP.gameObject.SetActive (false);
        CandleP.gameObject.SetActive (true);
    }
}
