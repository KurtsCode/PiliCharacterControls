/*
 * Author: Kurt Noe
 * Desc: Receives all of the proper ability values from the ability, summon, and controller scripts and executes the chosen ability in the game space. This version of the triggerable script is specifically for
 *       abilities that are designed to move as part of their actions. Depending on other booleans that are specified within the values of the ability itself it can also allow the abilities to perform other
 *       actions besides simple damage application on hit such as spawning another hitbox, allowing the hitbox to travel back to the original attacker instead of merely vanishing after arriving at it's destination.
 *       Current version needs some cleaning to further refine its functionality.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonTriggerableMoving : MonoBehaviour {
    
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

    private GameObject cloneHitbox;

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

    [HideInInspector]public bool noHitBoxSpawn;


    private bool arrived;
    private bool returning;
    private bool returned;
    public bool isCasting;
    public bool doesReturn;
    public bool twoParterHitboxes;
    public bool onCollisionEffect;



    private bool isLeaving;

    public GameObject triggerableAttacker;
    
    private bool hitDetected;

    [HideInInspector] public Transform hitboxSpawn;
    public Transform hitboxSpawnCardinal;                                // Transform variable to hold the location where we will spawn our projectile
    public Transform hitboxSpawnAngle;
   
    [HideInInspector] public SummonController summonControl;

    public void Launch()
    {
       
        isMoving = false;

        arrived = false;

        returning = false;

        isLeaving = false;

        summonControl = attacker.GetComponent<SummonController>();

        faceDir = summonControl.direction;

        hitDespawnDelay = 0.2f;

        print("TRIGGERABLE ATTACKER " + attacker);


        posRangeCheck = new Vector3(attacker.transform.position.x + range, attacker.transform.position.y + range, attacker.transform.position.z + range);
        negativeRangeCheck = new Vector3(attacker.transform.position.x - range, attacker.transform.position.y - range, attacker.transform.position.z - range);

        // triggerableAttacker = attacker;

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





        /*
        detector = clonedHitbox.GetComponent<HitboxDetection>(); // grabs script that will initialize the object.
        detector.attacker = attacker;
        detector.damage = damage;
        */
        // Destroy(clonedHitbox, meleeDelay);

        /*
         public Transform startMarker;
        public Transform endMarker;
        public float speed = 1.0F;
        private float startTime;
        private float journeyLength;
        void Start() {
        startTime = Time.time;
        journeyLength = Vector3.Distance(startMarker.position, endMarker.position);
        }
        void Update() {
        float distCovered = (Time.time - startTime) * speed;
        float fracJourney = distCovered / journeyLength;
        transform.position = Vector3.Lerp(startMarker.position, endMarker.position, fracJourney);
        }
         */


    }


    // Update is called once per frame
    void Update()
    {
        // Starts with is Moving based off of isMoving bool. Will start a new state and turn off the previous one to perform each action. 
        // All of this is to create the summonVisual instantiation to dictate where hitboxes will spawn as it moves around.


        if (isCasting) // Bool to ensure that the summon cannot be used multiple times due to update.
        {
            print("isCasting Activated");
            isMoving = true;
            isCasting = false;



            summonVisual = Instantiate(sumChar, attacker.transform.position, attacker.transform.rotation);  // Create visual element for the summon.
/*
            if (faceDir == 3 || faceDir == 5 || faceDir == 7)
            {
                summonVisual.transform.rotation = Quaternion.Inverse(summonVisual.transform.rotation);
            }
            */
            print("VISUAL SUMMONED");
            charHitbox = Instantiate(triggerHitbox, attacker.transform.position, hitboxRotation); 
            charDetector = charHitbox.GetComponent<HitboxDetection>();  // Accesses the detector for the hitbox that will follow the character..
            print("CHARHITBOX INSTANTIATED");
            if (doesDamage)  // Determine damage and pass it to the hitbox script
            {
                print("CHARHITBOX DOES DAMAGE");
                charDetector.attacker = attacker;

                print("charHitbox ATTACKER " + charDetector.attacker);
                charDetector.damage = attackVal;

                print("charHitbox DAMAGE " + charDetector.damage);

                charDetector.doesDamage = doesDamage;
            }

            if (doesForce) // Determine force and pass it to the hitbox script
            {

            }

            if (doesHeal) // Determine heal and pass it to the hitbox script
            {
                charDetector.attacker = attacker;
                charDetector.healingDone = healVal;

            }
            if (doesStatus) // Determine status and pass it to the hitbox script
            {

            }

            charHitbox.transform.parent = summonVisual.gameObject.transform;
   

        }



        if (isMoving)  
            {
                print("IS MOVING DETECTED");
                MovePlayer(); // spawn player representation and move it to clicked location.
            }

            //detector = clonedHitbox.GetComponent<HitboxDetection>(); // grabs script that will initialize the object.

            if (arrived && !noHitBoxSpawn)        // Spawn the hitboxes with the effects they have once they arrive to their location or activates.
            {
                print("SPAWN HITBOX DETECTED");
                SpawnHitbox();
            }

            if(arrived && noHitBoxSpawn)
            {
                TwoPartFollow();
            }

            if (returning)     // Returns to the summmoner after completing its actions.
            {
                print("RETURN HOME DETECTED");
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


    void TwoPartFollow()
    {
        if (twoParterHitboxes)
        {

            print("arrived state for two part" + arrived);
            print("noHitBoxSpawn state for two part" + noHitBoxSpawn);
            if (arrived && noHitBoxSpawn)
            {
                Destroy(charHitbox);       // destroy the hitbox to trigger the next part of the animation.

                charHitbox = Instantiate(triggerHitbox, attacker.transform.position, hitboxRotation);
                charDetector = charHitbox.GetComponent<HitboxDetection>();  // Accesses the detector for the hitbox that will follow the character..
                print("CHARHITBOX 2 INSTANTIATED");
                if (doesDamage)  // Determine damage and pass it to the hitbox script
                {
                    print("CHARHITBOX 2 DOES DAMAGE");
                    charDetector.attacker = attacker;

                    print("charHitbox 2 ATTACKER " + charDetector.attacker);
                    charDetector.damage = attackVal;

                    print("charHitbox 2 DAMAGE " + charDetector.damage);

                    charDetector.doesDamage = doesDamage;
                }

                if (doesForce) // Determine force and pass it to the hitbox script
                {

                }

                if (doesHeal) // Determine heal and pass it to the hitbox script
                {
                    charDetector.attacker = attacker;
                    charDetector.healingDone = healVal;

                }
                if (doesStatus) // Determine status and pass it to the hitbox script
                {

                }

                charHitbox.transform.parent = summonVisual.gameObject.transform;

                if (doesReturn)
                {   
                    
                    StartCoroutine("timeToReturn");
                }
                else
                {
                    StartCoroutine("timetoVanish");
                }

                arrived = false;

            }
        }
    }

    void ReturnHome() // Need to update this where it checks the location of the player that is constantly updating for a return. That part might need to happen specifically in update.
    {
        // This might have to happen outside of a function that is in update.
        // transform.LookAt(attacker.transform.position);

        summonVisual.transform.rotation = Quaternion.Inverse(summonVisual.transform.rotation);

        summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, attacker.transform.position, travelSpeed * Time.deltaTime);

        if (summonVisual.transform.position == attacker.transform.position)
        {
            summonVisual.transform.rotation = new Quaternion(0, 0, 0, 0);
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
            else if(summonSlot == 2)
            {
                print("summonSlot 2 selected");
                summonControl.secondNextReadyTime = sumCooldown + Time.time;
                summonControl.summonActive = false;
            }
            print("Next Ready Time: " + summonControl.firstNextReadyTime);    
        }


        Debug.DrawLine(summonVisual.transform.position, attacker.transform.position, Color.red);
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
            print("Summon Final Hitbox Deals Damage");
            detector.attacker = attacker;

            print("detector passed attacker " + attacker);
            print("detector attacker " + detector.attacker);
            detector.damage = attackVal;
            detector.doesDamage = doesDamage;
            detector.textDelay = 0.2f;


            /*
            GameObject clonedHitbox = Instantiate(givenProjectile, hitboxSpawn.position, hitboxRotation); //Instantiates a projectile from the given prefab.

            detector = clonedHitbox.GetComponent<HitboxDetection>(); // grabs script that will initialize the object.
            detector.attacker = attacker;
            detector.damage = damage;
            detector.doesDamage = doesDamage; 
            
              Destroy(clonedHitbox, meleeDelay);
         */


        }

        if (doesForce) // Determine force and pass it to the hitbox script
        {

        }

        if (doesHeal) // Determine heal and pass it to the hitbox script
        {
            print("Summon Final Hitbox Heals");
            detector.attacker = attacker;
            detector.healingDone = healVal;
            detector.doesHeal = doesHeal;
        }
        if (doesStatus) // Determine status and pass it to the hitbox script
        {

        }

      
        Destroy(clonedHitbox, hitDespawnDelay);

        if (doesReturn)
        {
            StartCoroutine("timeToReturn");
        }
        else
        {
            StartCoroutine("timetoVanish");
        }

        arrived = false;
        //StartCoroutine(timeToReturn(summonVisual));

        //summonControl.cooldown = sumCooldown;  // TO REFINE THIS HAVE THE COOLDOWN ACTIVATE ONCE THE ABILITY RETURNS AND JUST HAVE AN ACTIVE STATE THAT KEEPS THE ABILITY UNACTIVATABLE.
    }

    void SpawnFollowHitbox()
    {
        if (onCollisionEffect)
        {
            if (charDetector.hitDetected)  // If the hitbox hits a target.
            {
                castTarget = summonVisual.transform.position;  // Updates the castTarget position for the MovePlayer() script to stop it from moving.
                Destroy(charHitbox);       // destroy the hitbox to trigger the next part of the animation.
                SpawnHitbox();             // Spawn the hitboxes for the new action that was passed by the ability.
            }
        } 
    }


    void MovePlayer()
    {
        summonControl.PlayShortHit();
        if (doesHitboxFollow) // If the attack has a hitbox on the summon.
        {
            print("SPAWN FOLLOW DETECTED");
            SpawnFollowHitbox();
        }


        print("MOVE PLAYER STARTED");
        // Instantiates a projectile from the given prefab. TEMP LOOK AT THIS
        // gameObject.transform.LookAt(target Position); // Rotates the object to face the click location. Disable this to remove that effect if needed.
        // target Position = castTarget

        if (castTarget.x > posRangeCheck.x && castTarget.z > posRangeCheck.z)  // Checks if the cast target determined by mouse click is greater than the cast range.
        {

            limitTargetPos = new Vector3(posRangeCheck.x, castTarget.y, posRangeCheck.z);  // Sets a cast limit.
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime); // Transform the summon to the position specified by the limited range of the cast point specified by.
            MonoBehaviour.print("exceeded both ranges");
            if (summonVisual.transform.position == limitTargetPos)            // Once it hits the limited range.
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;                                // Sets isMoving false to stop it from moving                
                             
                
                 arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + summonVisual.transform.position.x);
                MonoBehaviour.print("SUMMON Y" + summonVisual.transform.position.y);
                MonoBehaviour.print("SUMMON Z" + summonVisual.transform.position.z);


                StartCoroutine("timeDeactivate");  // Start coroutine to despawn the hitboxes
            }

        }

        else if (castTarget.x > posRangeCheck.x && castTarget.z < negativeRangeCheck.z)
        {

            limitTargetPos = new Vector3(posRangeCheck.x, castTarget.y, negativeRangeCheck.z);
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime);
            MonoBehaviour.print("exceeded both ranges");
            if ((summonVisual.transform.position.x == limitTargetPos.x) && (summonVisual.transform.position.z == limitTargetPos.z))
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;
                arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + transform.position.x);
                MonoBehaviour.print("SUMMON Y" + transform.position.y);
                MonoBehaviour.print("SUMMON Z" + transform.position.z);


                StartCoroutine("timeDeactivate");
            }


        }



        else if (castTarget.x < negativeRangeCheck.x && castTarget.z > posRangeCheck.z)
        {

            limitTargetPos = new Vector3(negativeRangeCheck.x, castTarget.y, posRangeCheck.z);
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime);
            MonoBehaviour.print("exceeded both ranges");
            if ((summonVisual.transform.position.x == limitTargetPos.x) && (summonVisual.transform.position.z == limitTargetPos.z))
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;
                arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + summonVisual.transform.position.x);
                MonoBehaviour.print("SUMMON Y" + summonVisual.transform.position.y);
                MonoBehaviour.print("SUMMON Z" + summonVisual.transform.position.z);


                StartCoroutine("timeDeactivate");
            }


        }


        else if (castTarget.x > posRangeCheck.x)
        {
            limitTargetPos = new Vector3(posRangeCheck.x, castTarget.y, castTarget.z);
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime);
            MonoBehaviour.print("exceeded x");
            if ((summonVisual.transform.position.x == limitTargetPos.x) && (summonVisual.transform.position.z == limitTargetPos.z))
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;
                arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + transform.position.x);
                MonoBehaviour.print("SUMMON Y" + transform.position.y);
                MonoBehaviour.print("SUMMON Z" + transform.position.z);

                StartCoroutine("timeDeactivate");
            }


        }

        else if (castTarget.z > posRangeCheck.z)
        {
            limitTargetPos = new Vector3(castTarget.x, castTarget.y, posRangeCheck.z);
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime);
            MonoBehaviour.print("exceeded z");
            if ((summonVisual.transform.position.x == limitTargetPos.x) && (summonVisual.transform.position.z == limitTargetPos.z))
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;
                arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + summonVisual.transform.position.x);
                MonoBehaviour.print("SUMMON Y" + summonVisual.transform.position.y);
                MonoBehaviour.print("SUMMON Z" + summonVisual.transform.position.z);

                StartCoroutine("timeDeactivate");
            }


        }

        else if (castTarget.x < negativeRangeCheck.x && castTarget.z < negativeRangeCheck.z)
        {
            limitTargetPos = new Vector3(negativeRangeCheck.x, castTarget.y, negativeRangeCheck.z);
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime);
            MonoBehaviour.print("exceeded both ranges");
            if ((summonVisual.transform.position.x == limitTargetPos.x) && (summonVisual.transform.position.z == limitTargetPos.z))
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;
                arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + summonVisual.transform.position.x);
                MonoBehaviour.print("SUMMON Y" + summonVisual.transform.position.y);
                MonoBehaviour.print("SUMMON Z" + summonVisual.transform.position.z);

                StartCoroutine("timeDeactivate");
            }


        }

        else if (castTarget.x < negativeRangeCheck.x)
        {
            limitTargetPos = new Vector3(negativeRangeCheck.x, castTarget.y, castTarget.z);
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime);
            MonoBehaviour.print("exceeded -x");
            if ((summonVisual.transform.position.x == limitTargetPos.x) && (summonVisual.transform.position.z == limitTargetPos.z))
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;
                arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + summonVisual.transform.position.x);
                MonoBehaviour.print("SUMMON Y" + summonVisual.transform.position.y);
                MonoBehaviour.print("SUMMON Z" + summonVisual.transform.position.z);

                StartCoroutine("timeDeactivate");
            }


        }

        else if (castTarget.z < negativeRangeCheck.z)
        {
            limitTargetPos = new Vector3(castTarget.x, castTarget.y, negativeRangeCheck.z);
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, limitTargetPos, travelSpeed * Time.deltaTime);
            MonoBehaviour.print("exceeded -z");

            if ((summonVisual.transform.position.x == limitTargetPos.x) && (summonVisual.transform.position.z == limitTargetPos.z))
            {
                MonoBehaviour.print("Arrived at range limit");
                isMoving = false;
                arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
                //SpawnHitbox(summonVisual);

                MonoBehaviour.print("SUMMON X" + summonVisual.transform.position.x);
                MonoBehaviour.print("SUMMON Y" + summonVisual.transform.position.y);
                MonoBehaviour.print("SUMMON Z" + summonVisual.transform.position.z);


                StartCoroutine("timeDeactivate");

                //CoroutineWithMultipleParameters(1.0F, 2.0F, "foo")
            }


        }
        else
        {
            summonVisual.transform.position = Vector3.MoveTowards(summonVisual.transform.position, castTarget, travelSpeed * Time.deltaTime);

            // gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, travelTarget, travelRate); 
        }



        if ((summonVisual.transform.position.x == castTarget.x) && (summonVisual.transform.position.z == castTarget.z))
        {

            MonoBehaviour.print("SUMMON X" + summonVisual.transform.position.x);
            MonoBehaviour.print("SUMMON Y" + summonVisual.transform.position.y);
            MonoBehaviour.print("SUMMON Z" + summonVisual.transform.position.z);

            MonoBehaviour.print("Matches all parameters");
            isMoving = false;
            arrived = true;                                  // Sets arrived to true to run the SpawnHitbox script.
            //SpawnHitbox(summonVisual);

            StartCoroutine("timeDeactivate");
        }

        Debug.DrawLine(summonVisual.transform.position, castTarget, Color.red);
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

    /*
    IEnumerator timeToReturn(GameObject summonVisual)
    {
        yield return new WaitForSeconds(1);
        returning = true;
        ReturnHome(summonVisual);

    }
    */
    IEnumerator timeDestroy()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(summonVisual);

    }

    IEnumerator timetoVanish()
    {
        yield return new WaitForSeconds(0.5f);
        isLeaving = true;
    }

}
