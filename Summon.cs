/*
 * Author: Kurt Noe
 * Desc: Scriptable object class that contains all of the relevant information for an ability to operate when passed to the triggerable scripts. pulls values like the attacker from the controller and Summon
 *       scripts that preceded it.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Summon : ScriptableObject {
    public string aName = "New Summon";
    public Sprite aSprite;         // Sprite for the summon.
    public AudioClip aSound;       // Sound that plays for the summon.

    public GameObject attacker;    // Attacker that the summon is sourced form.
    public GameObject sumChar;     // Gameobject that will be played for the summon. 
    public GameObject sumChar2;
    public GameObject sumChar3;
    public GameObject selectedSprite;

    public Ability ability1;       // First ability that the summon can use.
    public Ability ability2;       // second ability that the summon can use.
    public Ability ability3;       // Third ability that the summon can use.
    public Ability ability4;       // Fourth ability that the summon can use.

    public Ability selectedAbility;

    public bool movingAbility;
    public bool doesDamage;         // Check if the summon attacks will deal damage.
    public bool doesForce;          // Check if the summon attacks will push, pull, etc. targets.
    public bool doesStatus;         // Check if the summon attacks will inflict any status effects.
    public bool doesHeal;           // Check if the summon attacks will heal valid targets.

    public float aBaseCoolDown = 1f;
    public float attackVal;         // Base attack value of the summon.
    public float defenseVal;        // Base defense value of the summon.
    public float travelSpeed;       // Travel speed of the summon if it moves.
    public float range;             // Cast range of the summon and its abilities;

    public int summonSlot;

    public Vector3 castTarget;      // Target location for the summon and its abilities. (Passed from SummonController).

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAbility();

}
