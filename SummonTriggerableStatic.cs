/*
* Author: Kurt Noe
* Desc: Receives all of the proper ability values from the ability, summon, and controller scripts and executes the chosen ability in the game space. This version of the triggerable script is specifically for
*       abilities that are designed not to move as part of their actions. Depending on other booleans that are specified within the values of the ability itself it can also allow the abilities to perform other
*       actions besides simple damage application on hit such as spawning another hitbox, allowing the hitbox to travel back to the original attacker instead of merely vanishing after completing its actions.
*       Current version needs some cleaning to further refine its functionality.
* Version: 1.0
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonTriggerableStatic : MonoBehaviour {

    
    /*
     * Passing through the other functions.
     * 
     * Controller:
     * attacker to summon
     * castTarget to summon
     * Needs to pass it the cooldown back on activation.
     * 
     * Aumakua_Base:
     * attacker to ability
     * summChar to ability
     * castTarget to ability
     * 
     * Summon_Ability:
     * PASSES TO EXECUTER
     * attacker 
     * sumchar
     * hitbox
     * hitboxangle
     * movingability
     * doesDamage
     * doesForce
     * doesStatus
     * doesHeal
     * sumcooldown
     * healval
     * attackval
     * travelspeed
     * range
     */

    //  private Vector3 startPosition;          // Starting position for the summon.
    private Vector3 posRangeCheck;            // Check to ensure that the summon does not exceed the positive casting range of the ability.
    private Vector3 negativeRangeCheck;       // Check to ensure that the summon does not exceed the negative casting range of the ability.
    private Vector3 limitTargetPos;           // Resulting position of the cast target and the actual casting range that the object will now travel to.
    private Vector3 startingPositon;
    // private Vector3 spawnPoint; 

    private HitboxDetection detector;       // Detector to represent the hitbox for the ability that is spawned disconnected from the summon.
    private HitboxDetection charDetector;   // Detector to pass values and access the hitbox that follows the summon.

    [HideInInspector] public Sprite aSprite;                  // Sprite for the summon.
    [HideInInspector] public AudioClip aSound;                // Sound that plays for the summon.

    [HideInInspector] public GameObject attacker;             // Attacker that the summon is sourced form. this has been sourced fomr the gameobject the contrller is atached to.
    [HideInInspector] public GameObject sumChar;              // Gameobject that will be played for the summon to represent it.
    [HideInInspector] public GameObject hitboxCardinal;       // Hitbox for the regular attacks.
    [HideInInspector] public GameObject hitboxAngle;          // Hitbox for the diagonal attacks.
    [HideInInspector] public GameObject triggerHitbox;        // Hitbox passed in to spawn to follow on the summon during moving abilities.
    [HideInInspector] public GameObject charHitbox;           // Hitbox representing the instantiated clone of triggerHitBox.
    [HideInInspector] private GameObject summonVisual;        // Object for the sprite or object that represents the summon.
    [HideInInspector] private GameObject givenHitbox;         // Hitbox selected for the ability to be spawned for the attack portion of the summon.

    [HideInInspector] Quaternion hitboxRotation;

    [HideInInspector] public bool movingAbility;              // Checks if the summon itself moves during the ability.
    [HideInInspector] public bool isMoving;                   // Switched on if is Moving Ability. Triggers moving actions until turned off at the end of actions.
    [HideInInspector] public bool doesDamage;                 // Check if the summon attacks will deal damage.
    [HideInInspector] public bool doesForce;                  // Check if the summon attacks will push, pull, etc. targets.
    [HideInInspector] public bool doesStatus;                 // Check if the summon attacks will inflict any status effects.
    [HideInInspector] public bool doesHeal;                   // Check if the summon attacks will heal valid targets.
    [HideInInspector] public bool doesHitboxFollow;

    [HideInInspector] public float sumCooldown;               // To be passed back to the Summon Controller to manage the particular summon's current cooldown.
    [HideInInspector] public float attackVal;                 // Base attack value of the summon.
    [HideInInspector] public float scndAttackVal;
    [HideInInspector] public float healVal;

    [HideInInspector] public float travelSpeed;               // Travel speed of the summon if it moves.
    [HideInInspector] public float range;                     // Cast range of the summon and its abilities;
    [HideInInspector] public float hitDespawnDelay;           // Delay before the hitbox despawns.

    [HideInInspector] public int faceDir;
    [HideInInspector] public int summonSlot;

    [HideInInspector] public Vector3 castTarget;              // Target location for the summon and its abilities. (Passed from SummonController)

    


    private bool arrived;
    private bool returning;
    private bool returned;
    private bool isStatic;

    public bool doesReturn;

    private bool isLeaving;

    public GameObject triggerableAttacker;
    
    private bool hitDetected;

    [HideInInspector] public Transform hitboxSpawn;
    public Transform hitboxSpawnCardinal;                                // Transform variable to hold the location where we will spawn our projectile
    public Transform hitboxSpawnAngle;
   
    [HideInInspector] public SummonController summonControl;

    public void Launch()
    {
        summonControl = attacker.GetComponent<SummonController>();

        arrived = false;

        faceDir = summonControl.direction;

        hitDespawnDelay = 0.2f;

        print("TRIGGERABLE ATTACKER " + attacker);

        //triggerableAttacker = attacker;

        isStatic = true;

        // Determines the direction and type of hitbox that should be used based off of the direction passed down pulled from the palyer movements.

        if (faceDir == 0 || faceDir == 1 || faceDir == 2 || faceDir == 3)
        {
            givenHitbox = hitboxCardinal;                       // Passes the id of the hitbox that will be spawned when triggered.
            // print("bar hitbox");

            hitboxRotation = transform.rotation;                // Sets the rotation of the spawning hitbox based off of the parent object.
            hitboxSpawn = hitboxSpawnCardinal;                  // Determines which hitbox object that spawns.

        }

        else
        if (faceDir == 4 || faceDir == 5 || faceDir == 6 || faceDir == 7)
        {
            givenHitbox = hitboxAngle;
            // print("angled hitbox");

            hitboxRotation = transform.rotation;
            hitboxSpawn = hitboxSpawnAngle;
            hitboxRotation *= Quaternion.Euler(0, 45, 0);
        }

        else
        {
            givenHitbox = hitboxCardinal;
        }


    

        // Has all passed values from summonability;


        // Need to create a new script that contains all of the values that triggerable has that actually runs things with the process.




        //throw new System.NotImplementedException();


    }


    // Update is called once per frame
    void Update()
    {
        if (isStatic)
        {
            isStatic = false;
            print("TRIGGERABLE ATTACKER IN ELSE" + attacker);
            summonVisual = Instantiate(sumChar, attacker.transform.position, attacker.transform.rotation); //Instantiates a projectile from the given prefab. TEMP LOOK AT THIS
                                                                                                           // Could use an ability delay for the wind up.
            summonControl.PlayShortHit();
            ActivateObject();
            // Need to set a delay function before it spawns the hitbox.

          
        }

        if (arrived)
        {
            print("SPAWNING HITBOX FOR STATIC");
            SpawnHitbox();
        }


        if (returning)     // Returns to the summmoner after completing its actions.
        {
            print("RETURN HOME DETECTED FOR STATIC");
            ReturnHome();
        }

        if (isLeaving)
        {
            SummonLeave();
        }
        

      

        // Need to set a delay function before it spawns the hitbox.
        //GameObject clonedHitbox = Instantiate(givenHitbox, hitboxSpawn.position, hitboxRotation); // CHECK THIS for clarity of hitboxrotation;
        /*
                   summonVisual.transform.position = Vector3.Lerp(startingPosition.position, castTarget, fracJourney);

                   if (fracJourney == 1)
                   {
                   GameObject hitboxClone = Intantiate(givenHitbox, hitboxSpawn.position, hitboxSpawn.rotation);
                   }
       */

    }

    void ActivateObject()
    {
        arrived = true;
        StartCoroutine("timeDeactivate");
    }

    void ReturnHome() // Need to update this where it checks the location of the player that is constantly updating for a return. That part might need to happen specifically in update.
    {
        // transform.LookAt(attacker.transform.position);
        print("RETURNING HOME");
        summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, attacker.transform.position, travelSpeed * Time.deltaTime);

        if (summonVisual.transform.position == attacker.transform.position)
        {
            returned = true;
            StartCoroutine("timeDestroy");
            returning = false;

            print("summon cooldown starts");
            if (summonSlot == 1)
            {
                print("summonSlot 1 selected");
                summonControl.firstNextReadyTime = sumCooldown + Time.time;
                summonControl.summonActive = false;
            }
            else if (summonSlot == 2)
            {
                print("summonSlot 2 selected");
                summonControl.secondNextReadyTime = sumCooldown + Time.time;
                summonControl.summonActive = false;
            }
            print("Next Ready Time: " + summonControl.firstNextReadyTime);
        }

        Debug.DrawLine(transform.position, attacker.transform.position, Color.red);
    }


    void SummonLeave()
    {
        isLeaving = false;
        StartCoroutine("timeDestroy");


        print("summon cooldown starts");
        if (summonSlot == 1)
        {
            print("summonSlot 1 selected");
            summonControl.firstNextReadyTime = sumCooldown + Time.time;
            summonControl.summonActive = false;
        }
        else if (summonSlot == 2)
        {
            print("summonSlot 2 selected");
            summonControl.secondNextReadyTime = sumCooldown + Time.time;
            summonControl.summonActive = false;
        }
        print("Next Ready Time: " + summonControl.firstNextReadyTime);

    }


    void SpawnHitbox()
    {

        GameObject clonedHitbox = Instantiate(givenHitbox, summonVisual.transform.position, hitboxRotation); // CHECK THIS for clarity of hitboxrotation;


        detector = clonedHitbox.GetComponent<HitboxDetection>(); // grabs script that will initialize the object.

        if (doesDamage)  // Determine damage and pass it to the hitbox script
        {
            print("Summon ability 2 Final Hitbox Deals Damage");
            detector.attacker = attacker;

            print("detector passed attacker " + attacker);
            print("detector attacker " + detector.attacker);

            detector.damage = attackVal;
            detector.doesDamage = doesDamage;
            detector.textDelay = 0.2f;
        }

        if (doesForce) // Determine force and pass it to the hitbox script
        {

        }

        if (doesHeal) // Determine heal and pass it to the hitbox script
        {
            detector.attacker = attacker;
            detector.healingDone = healVal;
            detector.doesHeal = doesHeal;
        }
        if (doesStatus) // Determine status and pass it to the hitbox script
        {

        }

       

        Destroy(clonedHitbox, hitDespawnDelay);

        print("STATIC DOES RETURN RESULT" + doesReturn);

        if (doesReturn)
        {
            StartCoroutine("timeToReturn");
        }
        else
        {
            StartCoroutine("timetoVanish");
        }
       

        //isStatic = false;
        //summonControl.cooldown = sumCooldown;  // TO REFINE THIS HAVE THE COOLDOWN ACTIVATE ONCE THE ABILITY RETURNS AND JUST HAVE AN ACTIVE STATE THAT KEEPS THE ABILITY UNACTIVATABLE.

        arrived = false;

    }

    void SpawnFollowHitbox()
    {
        charHitbox = Instantiate(triggerHitbox, attacker.transform.position, hitboxRotation); // CHECK THIS for clarity of hitboxrotation AND HITBOX;  Creates hitbox on top of the summon.
        // Need to set the spawned hitbox as the child of the object. Alternatively just slap a hitbox as the child of it and activate it.
        // Currently saying its an empty reference because the 
        charDetector = charHitbox.GetComponent<HitboxDetection>();  // Accesses the detector for the hitbox.

        if (charDetector.hitDetected)  // If the hitbox hits a target.
        {
            castTarget = attacker.transform.position;  // Updates the castTarget position for the MovePlayer() script to stop it from moving.
            Destroy(charHitbox);       // destroy the hitbox to trigger the next part of the animation.
            SpawnHitbox();             // Spawn the hitboxes for the new action that was passed by the ability.
        }
    }


    IEnumerator timeDeactivate()
    {
        yield return new WaitForSeconds(1);
        //gameObject.active = false;
    }

    IEnumerator timeToReturn()
    {
        yield return new WaitForSeconds(1);
        returning = true;
    }

    IEnumerator timeDestroy()
    {
        yield return new WaitForSeconds(1f);
        Destroy(summonVisual);

    }

    IEnumerator timetoVanish()
    {
        yield return new WaitForSeconds(0.5f);
        isLeaving = true;
    }

}
