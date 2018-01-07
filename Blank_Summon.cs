/*
 * Author: Kurt Noe
 * Desc: A placeholder summon to fill one of the two summons slots in the case that the player does not yet have access to two summons or only has one or none equipped. Simply feeds dead end values to ensure
 *       that the casting function begins and ends successfully without any actual result if the empty summon slot is chosen.
 * Version: 1.0
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Summons/Blank_Summon")]

// Creates a summon for the base aumakua. passes the ability and triggerable scripts summon specific information, 
// primarily the attacker that summoned it and a game object as it's ingame representation.
// Need to ensure sprite and sounds work correctly afterwards. Also base values for each summon can be implemented later.
// Values such as the damage and actions of the ability chosen will be handled else where in the ability and executer based scripts.

public class Blank_Summon : Summon {
   
    public float damage;
   // public Ability selectedAbility;
   // public Ability ability1;
   // public Ability ability2;
   // public Ability ability3;
   // public Ability ability4;

   //  public GameObject sumChar;

   // private SummonTriggerable summoner;
    private SummonController sumController;


    // Use this for initialization
    public override void Initialize(GameObject obj)
    {

        sumController = attacker.GetComponent<SummonController>();
        sumController.castActive = false;
        sumController.sumChosen = 0;

        MonoBehaviour.print("No summon equipped in this slot");
        

    }

    public override void TriggerAbility()
    {
        
    }
}
