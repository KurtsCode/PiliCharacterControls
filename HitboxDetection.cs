/*
 * Author: Kurt Noe & Jeanclyde Cruz
 * Desc: Attached to hitbox objects to carry out the desired effects when a valid target comes into contact with ability hitboxes. Checks for flags assigned to the abilities that create them to determine
 *       what effects should happen when triggered such as damage, heal, status effects, and pushing or pulling forces.
 * Version: 0.6
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxDetection : MonoBehaviour {


    public float damage;//should try to grab appropriate value from attacking character
    public float healingDone;
    public GameObject attacker;//character that spawn this hitbox. used to find appropriate attack value. assign this when instantiating a hitbox

    public bool doesDamage;         // Check if the summon attacks will deal damage.
    public bool doesForce;          // Check if the summon attacks will push, pull, etc. targets.
    public bool doesStatus;         // Check if the summon attacks will inflict any status effects.
    public bool doesHeal;           // Check if the summon attacks will heal valid targets.

    public bool isBlocking;

    public bool hitDetected;

    public float textDelay;

    private EntityVitals healthCheck;

    // Use this for initialization
    void Start()
    {
        //  assignDamageValue();.
         print("Hitbox damage is " + damage);
         print("Hitbox attacker is " + attacker);
         print("doesDamage " + doesDamage);

         isBlocking = false;
         hitDetected = false;
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject);

        bool condition1 = other.gameObject != attacker; //used to prevent hitbox from hitting owner
        bool condition2 = other.gameObject.GetComponent<EntityVitals>() != null;//check it is attackable
        if (condition1 && condition2)
        {
            hitDetected = true;
            print("Hit target " + other);
        }

        if (doesDamage)
        {
            print("damage check worked");
            sendDamageMessage(other);
            //collisionCheck(other);
            print("Hit detected");
          
            print("Hitbox damage " + damage);
        }

        if (doesHeal)
        {
            sendHealingMessage(other);
            print("Hitbox healing " + healingDone);
        }

        if (doesStatus)
        {
            sendStatusMessage(other);
            print("Hitbox Status applied ");
        }

        if (doesForce)
        {
            sendForceMessage(other);
            print("Hitbox Force Applied");
        }
    }

    //tries to find how much to deal damage
    public void assignDamageValue()
    {
        //Debug.Log(attacker);
        damage = attacker.GetComponent<EntityVitals>().Attack;//should be able to find a script that inherits from entityvitals

        print("attacker is " + attacker);
        //Debug.Log(damage);
        print("damage assigned");
    }

    public void sendDamageMessage(Collider other)
    {
        print("damage message initialized");
        bool condition1 = other.gameObject != attacker; //used to prevent hitbox from hitting owner
        bool condition2 = other.gameObject.GetComponent<EntityVitals>() != null;//check it is attackable
        bool condition3 = other.gameObject.CompareTag("Ehuehu");
        if ((condition1 && condition2) || condition3)
        {
            print("Entity Receiving Damage " + other);

            // healthCheck = other.gameObject.GetComponent<EntityVitals>();

            // print("Hit Entity Health Remaining " + healthCheck.hitPoints);

            if (isBlocking)
            {
                damage = 0f;
            }

            other.gameObject.SendMessage("incomingDamage", damage);//calls a method to inflict damage
            print("Message Sent");

            isBlocking = false;
        }
    }


    public void sendHealingMessage(Collider other) // VERY BROKEN, Neeeds fixing. 
    {
       /* bool condition1 = other.gameObject == attacker; //used to only allow hitbox to hit owner
        bool condition2 = other.gameObject.GetComponent<EntityVitals>() != null; //check it is attackable
        
        if (condition1 && condition2)
        {
            print("Entity Receiving Healing" + other);

            other.gameObject.SendMessage("incomingHealing", healingDone);//calls a method to inflict damage
            print("Healing Message Sent");
        } */
    }

    public void sendStatusMessage(Collider other)
    {

    }

    public void sendForceMessage(Collider other)
    {

    }
}


