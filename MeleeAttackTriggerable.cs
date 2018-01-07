/*
 * Author: Kurt Noe
 * Desc: Triggerable script to carry out the melee attack for the player character. Receives the value that tells the script which of the 8 directions the player is currently facing to decide which type of hitbox
 *       needs to be spawned for the attack. Spawns the hitbox and then enacts the given cooldown duration to limit the speed at which the player can perform the action. Attached to the ability emitter object.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackTriggerable : MonoBehaviour { 
    // A script referenced by specific abilities like Melee ability to handle the ascpects of the object when triggered.

    [HideInInspector] public GameObject projectile;                      // Rigidbody variable to hold a reference to our projectile prefab
    [HideInInspector] public GameObject projectileAngled;
    [HideInInspector] public GameObject givenProjectile;
    public Transform hitboxSpawnCardinal;                                // Transform variable to hold the location where we will spawn our projectile
    public Transform hitboxSpawnAngle;
    private Transform hitboxSpawn;
   // private Vector3 hitboxCorrection; 
    [HideInInspector] public GameObject attacker;                        // Attacker quality used to determine the damage values.
    [HideInInspector] public PlayerMovementDash dashLocker;
    Quaternion hitboxRotation;
    public Animator anim;
    private HitboxDetection detector;
    public float damage;
    public bool doesDamage;
    private float meleeDelay;

    public GameObject abilityVisual;


    [HideInInspector] public MeleeCoolDown passedDirect;
    [HideInInspector] public int faceDir;

    //[HideInInspector] public float projectileForce = 250f;                  // Float variable to hold the amount of force which we will apply to launch our projectiles

    public void Launch() //Function thats run on Script call. There is no Start() in this script.
    {

        print("Triggerable Start");
        passedDirect = attacker.GetComponent<MeleeCoolDown>();

        faceDir = passedDirect.direction;

        doesDamage = true;

        // print("DIRECTION GIVEN " + faceDir);

        meleeDelay = 0.3f;

       // print("ability triggered");

        // print("Triggerable attacker is " + attacker);
        // print("Triggerable damage is " + damage);
        

        //projectile.GetComponent("hitbox1").damage = damage;  //up date the damage of the hitbox that will be spawned. Then I need it to spawn the hitboxes in a set direction or pattern.



        if (faceDir == 0)
        {
            givenProjectile = projectile;                       // Passes the id of the hitbox that will be spawned when triggered.
           // print("bar hitbox");

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
                                                                // print("bar hitbox");

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
                                                                // print("bar hitbox");

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
        if (faceDir == 4 )
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


        StartCoroutine("AbilityWindup");



        StartCoroutine("AnimationDelay");


        // GameObject clonedHitbox = Instantiate(givenProjectile, hitboxSpawn.position, transform.rotation); //Instantiates a projectile from the given prefab.

     
        //Add force to the instantiated bullet, pushing it forward away from the bulletSpawn location, using projectile force for how hard to push it away
        //clonedBullet.AddForce(bulletSpawn.transform.forward * projectileForce);
    }



    IEnumerator AnimationDelay()
    {
        yield return new WaitForSeconds(0.3f);        //yield for certain amount of seconds
        anim.SetBool("attackRight", false);
        anim.SetBool("attackForward", false);
        anim.SetBool("attackBack", false);
        anim.SetBool("attackLeft", false);
        anim.SetBool("forward", false);
        anim.SetBool("back", false);
        anim.SetBool("left", false);
        anim.SetBool("right", false);
        yield return null;
    }


    IEnumerator AbilityWindup()
    {
        yield return new WaitForSeconds(0.3f);        //yield for certain amount of seconds
        print("Waited for ability windup");

        // GameObject clonedVisual = Instantiate(abilityVisual, hitboxSpawn.position, hitboxRotation);
        print("Melee finished");
        GameObject clonedHitbox = Instantiate(givenProjectile, hitboxSpawn.position, hitboxRotation); //Instantiates a projectile from the given prefab.

        detector = clonedHitbox.GetComponent<HitboxDetection>(); // grabs script that will initialize the object.
        detector.attacker = attacker;
        detector.damage = damage;
        detector.doesDamage = doesDamage;

        Destroy(clonedHitbox, meleeDelay);

        yield return null;
    }
}
