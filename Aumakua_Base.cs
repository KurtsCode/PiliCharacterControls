/*
 * Author: Kurt Noe
 * Desc: Inherits values from the Summon Scriptable object. Allows for the creation for summon asset prefabs that act as containers that the player can call upon to cast different abilities during gameplay. 
 *       Passes the ability and triggerable scripts the necessary information to execute the ability that the player selects. Determines which summon and which spell from that summon was selected and then
 *       who the ability originated from for the purpose of damage and other effect calculation.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Summons/Aumakua_Base")]

public class Aumakua_Base : Summon {
   
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
        // sumController = obj.GetComponent<SummonController>();

        // selectedAbility = sumController.selectedAbility; // Could also just handle this with an int pass.
        MonoBehaviour.print("Summon Initialized");
        MonoBehaviour.print("Summon Attacker " + attacker);
        selectedAbility.attacker = attacker;

        selectedAbility.SummonStandIn = selectedSprite;

        /*
        if (selectedAbility = ability1)
        {
            MonoBehaviour.print("Summon 1 Initialized");
            selectedAbility.SummonStandIn = sumChar;
        }
        else if(selectedAbility = ability2)
        {
            MonoBehaviour.print("Summon 2 Initialized");
            selectedAbility.SummonStandIn = sumChar;
        } */
        

        MonoBehaviour.print("Summon Slot Passed to Summon " + summonSlot);

        selectedAbility.summonSlot = summonSlot;
        MonoBehaviour.print("summon standin object " + sumChar);
        selectedAbility.castTarget = castTarget; // Passes the hit location gained from controller.

        selectedAbility.Initialize(obj);    // Initializes the ability to pass the values.
        // summoner.attacker = attacker;
        // summoner.sumChar = sumChar;
        
       // assignDamageValue();

        // summoner = obj.GetComponent<SummonTriggerable>();
        // summoner.damage = damage;
        // summoner.attacker = attacker;
        // summoner.sumChar = sumChar;

    }

    public override void TriggerAbility()
    {
        MonoBehaviour.print("Summon Triggered");
        selectedAbility.TriggerAbility();  // Triggers the ability to run with all the passed values after both the summon and the ability are initilized.
    }
}
