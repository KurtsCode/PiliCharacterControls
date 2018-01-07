/*
 * Author: Kurt Noe
 * Desc: Triggerable script to carry out the ranged attack for the player character. Receives the value that tells the script which of the 8 directions the player is currently facing to decide which type of hitbox
 *       needs to be spawned for the attack. Spawns the hitbox and then enacts the given cooldown duration to limit the speed at which the player can perform the action. Attached to the ability emitter object.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackTriggerable : MonoBehaviour
{
    // A script referenced by specific abilities like Melee ability to handle the ascpects of the object when triggered.

    [HideInInspector] public GameObject projectile;                      // Rigidbody variable to hold a reference to our projectile prefab
    [HideInInspector] public GameObject projectileAngled;                // Game object that will be spawned when character faces at an gangle.
    [HideInInspector] public GameObject givenProjectile;                 // To represent the resulting gameobject between the angled and cardinal options.
    [HideInInspector] public GameObject attacker;                        // Attacker quality used to determin the damage values.

    [HideInInspector] public int faceDir;                               // Resulting integer to signify which of eight directions is being faced.

    [HideInInspector] public MeleeCoolDown passedDirect;                // Direction that is passed down to determine which hitbox must spawn.

    private HitboxDetection detector;                                    // To pass the values over to the hitbox for damage and interaction calculations.
    // private Vector3 hitboxCorrection; 

    Quaternion hitboxRotation;                                           // Rotation of the hitbox for correct diagonal placement of angled hitbox.

    public Animator anim;

    public Transform hitboxSpawnCardinal;                                // Transform variable to hold the location where we will spawn our projectile
    public Transform hitboxSpawnAngle;                                   // Transform variable to hold the location of where the angled hitboxes will spawn.
    private Transform hitboxSpawn;

    public float damage;
    public float hitBoxForwardForce;

    public bool doesDamage;

    public GameObject abilityVisual;

    private float rangedDelay;

    //[HideInInspector] public float projectileForce = 250f;                  // Float variable to hold the amount of force which we will apply to launch our projectiles

    public void Launch() //Function thats run on Script call. There is no Start() in this script.
    {
        passedDirect = attacker.GetComponent<MeleeCoolDown>();

        faceDir = passedDirect.direction;

        MonoBehaviour.print("DIRECTION GIVEN " + faceDir);

        rangedDelay = 0.25f;

        // print("ability triggered");

        print("Triggerable attacker is " + attacker);
        print("Triggerable damage is " + damage);
        //Instantiate a copy of our projectile and store it in a new rigidbody variable called clonedBullet

        //projectile.GetComponent("hitbox1").damage = damage;  //up date the damage of the hitbox that will be spawned. Then I need it to spawn the hitboxes in a set direction or pattern.


        if (faceDir == 0)
        {
            givenProjectile = projectile;                       // Passes the id of the hitbox that will be spawned when triggered.


            hitboxRotation = transform.rotation;
            hitboxSpawn = hitboxSpawnCardinal;
            anim.SetBool("attackBack", true);
            anim.SetBool("forward", false);
            anim.SetBool("back", false);
            anim.SetBool("left", false);
            anim.SetBool("right", false);
        }

        else
         if (faceDir == 1)
        {
            givenProjectile = projectile;                       // Passes the id of the hitbox that will be spawned when triggered.


            hitboxRotation = transform.rotation;
            hitboxSpawn = hitboxSpawnCardinal;
            anim.SetBool("attackRight", true);
            anim.SetBool("forward", false);
            anim.SetBool("back", false);
            anim.SetBool("left", false);
            anim.SetBool("right", false);
        }

        else
         if (faceDir == 2)
        {
            givenProjectile = projectile;                       // Passes the id of the hitbox that will be spawned when triggered.
                                                                // print("bar hitbox");

            hitboxRotation = transform.rotation;
            hitboxSpawn = hitboxSpawnCardinal;
            anim.SetBool("attackForward", true);
            anim.SetBool("forward", false);
            anim.SetBool("back", false);
            anim.SetBool("left", false);
            anim.SetBool("right", false);
        }

        else
         if (faceDir == 3)
        {
            givenProjectile = projectile;                       // Passes the id of the hitbox that will be spawned when triggered.


            hitboxRotation = transform.rotation;
            hitboxSpawn = hitboxSpawnCardinal;

            anim.SetBool("attackLeft", true);
            anim.SetBool("forward", false);
            anim.SetBool("back", false);
            anim.SetBool("left", false);
            anim.SetBool("right", false);
        }

        // Checks for the diagonals.
        else
        if (faceDir == 4)
        {
            givenProjectile = projectileAngled;
            // print("angled hitbox");


            hitboxRotation = transform.rotation;
            hitboxRotation *= Quaternion.Euler(0, 45, 0);
            hitboxSpawn = hitboxSpawnAngle;
        }

        else

        if (faceDir == 5)
        {
            givenProjectile = projectileAngled;
            // print("angled hitbox");


            hitboxRotation = transform.rotation;
            hitboxRotation *= Quaternion.Euler(0, 45, 0);
            hitboxSpawn = hitboxSpawnAngle;
        }

        else

        if (faceDir == 6)
        {
            givenProjectile = projectileAngled;
            // print("angled hitbox");


            hitboxRotation = transform.rotation;
            hitboxRotation *= Quaternion.Euler(0, 45, 0);
            hitboxSpawn = hitboxSpawnAngle;
        }

        else

        if (faceDir == 7)
        {
            givenProjectile = projectileAngled;
            // print("angled hitbox");


            hitboxRotation = transform.rotation;
            hitboxRotation *= Quaternion.Euler(0, 45, 0);
            hitboxSpawn = hitboxSpawnAngle;
        }

        else

        {
            givenProjectile = projectile;
        }


        // GameObject clonedHitbox = Instantiate(givenProjectile, hitboxSpawn.position, transform.rotation); //Instantiates a projectile from the given prefab.

        StartCoroutine("AbilityWindup");



        StartCoroutine("AnimationDelay");
        //Add force to the instantiated bullet, pushing it forward away from the bulletSpawn location, using projectile force for how hard to push it away
        //clonedBullet.AddForce(bulletSpawn.transform.forward * projectileForce);
    }


    IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.45f);        //yield for certain amount of seconds
        anim.SetBool("right", false);
        anim.SetBool("forward", false);
        anim.SetBool("back", false);
        anim.SetBool("left", false);
        anim.SetBool("attackRight", false);
        anim.SetBool("attackLeft", false);
        anim.SetBool("attackForward", false);
        anim.SetBool("attackBack", false);
        yield return null;
    }

    IEnumerator AbilityWindup()
    {
        yield return new WaitForSeconds(0.4f);        //yield for certain amount of seconds
        print("Waited for ability windup");

        // GameObject clonedVisual = Instantiate(abilityVisual, hitboxSpawn.position, hitboxRotation);
        GameObject clonedHitbox = Instantiate(givenProjectile, hitboxSpawn.position, hitboxRotation); //Instantiates a projectile from the given prefab.
                                                                                                      // clonedVisual.transform.parent = clonedHitbox.gameObject.transform;

        Rigidbody tempRigidBody;

        tempRigidBody = clonedHitbox.GetComponent<Rigidbody>();
        tempRigidBody.AddForce(transform.right * hitBoxForwardForce);




        detector = clonedHitbox.GetComponent<HitboxDetection>(); // grabs script that will initialize the object.
        detector.attacker = attacker;
        detector.damage = damage;
        detector.doesDamage = doesDamage;

        Destroy(clonedHitbox, rangedDelay);
        yield return null;
    }

}
    /*  private void OnTriggerEnter(Collider other)
     {
         //Debug.Log(other.gameObject);
         sendDamageMessage(other);

     }


     public void sendDamageMessage(Collider other)
     {
         bool condition1 = other.gameObject != attacker;//used to prevent hitbox from hitting owner
         bool condition2 = other.gameObject.GetComponent<EntityVitals>() != null;//check it is attackable
         if (condition1 && condition2)
         {
             other.gameObject.SendMessage("incomingDamage", damage);//calls a method to inflict damage
         }
     } */




    /*
        Debug.Log("Kuʻi Mamao");
            GameObject tempHandler;
        musicSource.clip = rangedSound;
            musicSource.Play();

            // Instantiates object, from emitter position in scene, at emitter's rotation
            tempHandler = Instantiate(hitBoxRanged, hitBoxEmitter.transform.position, hitBoxEmitter.transform.rotation) as GameObject;

            Rigidbody tempRigidBody;
        tempRigidBody = tempHandler.GetComponent<Rigidbody>(); //Need Rigidbody to use AddForce
            tempRigidBody.AddForce(transform.right* hitBoxForwardForce);

            nextRanged = Time.time + rangedCD;

            Destroy(tempHandler, rangedDelay); // Second parameter of Destroy, destroys object after x seconds

        */




    /*
     *
      if (Input.GetKeyDown("space") && Time.time > nextDash)  
            print("space down");

    dashActive = true;
            //destPos = new Vector3(trackerStartPos.x, trackerStartPos.y, trackerStartPos.z);  // Sets starting dash destination

            // Locks
            canMove = false;  // Restricts movement.

            ablLock.abilityLock = true;   // Sets abilityLock to true in PlayerChar script to prevent ability usage while space is held down.
            print("abilities Locked" + ablLock.abilityLock);

    isCharging = true;            // Starts the charging state.
            print("isCharging" + isCharging);

    dashBuffer = true;
        
        }


        if (isCharging)
        {

            StartCoroutine(DashActionBuffer(bufferTime));

            if (!dashBuffer)
            {
                dashDistance += incSpeed* Time.deltaTime;

                print("dashDistance " + dashDistance);
            }
                // print("dashDistance " + dashDistance);

                dashLock = true;
             
        }

        if (dashDistance > maxIncrease)
        {
            isCharging = false;
            fixer = dashDistance - maxIncrease;
            dashDistance = dashDistance - fixer;
        }


        if (dashDistance == maxIncrease && dashLock)
        {
            isCharging = false;
            print("Final dashDistance " + dashDistance);
        }


        if (Input.GetKeyUp("space") && Time.time > nextDash && dashActive  )
        {
            print("space up");

isCharging = false; 
            print("isCharging" + isCharging);

canTurn = false;

            rotateLock.canRotate = false;

            print("canTurn " + canTurn);

inTransit = true;

            dashDistance = dashDistance + 20f;
            print("Totally Final Distance" + dashDistance);

            SetDashDirection(faceDir);

nextDash = Time.time + dashCD;


            print("nextDash " + nextDash);

        }

        if (inTransit)
        {
            if (gameObject.transform.position != travelTarget)  // translate player character to the dash travel destination.
            {
                canTurn = false;
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, travelTarget, travelRate); // translate player character to the dash travel destination.


                //gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);
            }

            

            if (gameObject.transform.position == travelTarget)
            {


                StartCoroutine(DashRecovery(dashRecoveryTime)); // Starts delay on dash recovery after finishing dash travel. 

dashLock = false;

                ablLock.abilityLock = false;

                print("abilities Locked " + ablLock.abilityLock);

dashDistance = 0;

                inTransit = false;

                dashActive = false;
            }

           
     */

