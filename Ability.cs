/*
 * Author: Kurt Noe
 * Desc: Scriptable object to create abilities for use by the player and npc's. Base values allow developer to specify the animations and sounds that will be played when the ability activates and what ability slot
 *       it is assigned to for the purposes of allowingg the player to select the ability. 
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject {

    public string aName = "New Ability";
    public Sprite aSprite;
    public AudioClip aSound;
    public GameObject attacker;
    public GameObject SummonStandIn; //  TEMP
    public int summonSlot;
    public Vector3 castTarget;
    public float aBaseCoolDown = 1f;

    public abstract void Initialize(GameObject obj);
    public abstract void TriggerAbility();

}
