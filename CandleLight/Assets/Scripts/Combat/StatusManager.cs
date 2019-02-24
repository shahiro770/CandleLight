using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public EffectBar effectBar;
    public Bar HPBar;
    public Bar MPBar;

    public void Init(PartyMember pm) {
        HPBar.Init(pm.HP, pm.CHP);
        MPBar.Init(pm.MP, pm.CMP);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
