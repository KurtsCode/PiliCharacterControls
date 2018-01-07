﻿/*
* Author: Kurt Noe
* Desc: Inherits values from the Ability Scriptable Object class and allows for the creation of different types of ranged attack asset prefabs. Specifically set up to carry out the functions of the 
*       ranged attack for the player character. When triggered it will pass the proper values to the MeleeAttackTriggerable script to perform the attack action with the proper values specified.
*       Calls the created version of this script that is created as an asset and assigned to the RangedCoolDown script. 
* Version: 1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/RangedAbility")]

public class RangedAbility : Ability { // Provides the information for a melee attack. Can be turned into an asset where the numbers can be messed with for different weapons.

    public float damage;        // Value pulled from the attacker gameObject qualities.
    // public GameObject attacker; // Attacker value set by the AbilityCooldown controller script.
    public GameObject hitbox;   // Should be pulling hitbox object as given by the user in the Unity Inspector menu or passed to the script.
    public GameObject hitboxAngle; // Hitbox that will spanwn if the character is facing diagonally.
    public bool isChargeable;  // Boolean to determine if the attack given has a charge time or is instant fire.
    public int ammo;           // Ammo count if the ability needs ammunition. Not going to use this for the demo but planting it here for the future.

    public bool doesDamage;

    public GameObject abilityVisual;

    //public float aBaseCoolDown = 0.3f;

    private RangedAttackTriggerable launcher;

    public override void Initialize(GameObject obj)  //GameObject being passed here is the weaponholder object.
    {

        // attacker = GameObject.FindWithTag("PiliChar");       // gets the attacker object to pull damage values from.
        assignDamageValue();

        MonoBehaviour.print("Initialized RangedAbility");

        MonoBehaviour.print("Ability attacker is " + attacker);
        MonoBehaviour.print("Ability damage is " + damage);

        assignDamageValue();                                    // Sets up potential damage value.

        launcher = obj.GetComponent<RangedAttackTriggerable>();  // Grabs script that will initialize the object.
        launcher.damage = damage;                               // Sets damage value within the triggerable script that activates once this script is initialized.
        launcher.attacker = attacker;                           // Passes attacker id to the triggerable script.
        launcher.projectile = hitbox;                       // Passes the id of the hitbox that will be spawned when triggered.
        launcher.projectileAngled = hitboxAngle;
        launcher.abilityVisual = abilityVisual;
        launcher.doesDamage = doesDamage;
    }

    public void assignDamageValue()
    {
        //Debug.Log(attacker);
        damage = attacker.GetComponent<EntityVitals>().Attack; // Pulls the final attack value from the attacker's EntityVitals for damage. 
       // MonoBehaviour.print("Damage value Assigned");

        //Debug.Log(damage);
    }

    public override void TriggerAbility()
    {
       // MonoBehaviour.print("Triggering Ability");
        launcher.Launch(); // Trigger the ability
    }
}
