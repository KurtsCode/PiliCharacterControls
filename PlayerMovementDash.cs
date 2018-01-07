/*
 * Author: Kurt Noe (Dash Controls and ability interaction), Jeanclyde Cruz and Nathan Nahina (Basic Movement, rotation, and original hotkey setup)
 * Desc: Script to allow the assigned object to move around the game world based on the keys pressed by the player. Currently only allows 8 way movement and prevents movement
 *       when conflicting directions (A and D) are pressed at the same time. Dash controls allow the player to press space to quickly move in the current direction the character is facing, during the dash period
 *       abilities are disabled but some movement is allowed to create a more fluid feeling action. Also references ElevationChecker to readjust the player object to the same elevation as the ground below it.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementDash : MonoBehaviour
{
    //public float speed;
    public float speed = 2.0f;
    public float smooth = 2.0f;
    public float rotationDirection; //added for rotating

    private float startTime;
    private float lerpTime = 0.5f;
    private float currentLerpTime = 0;

    // Camera positions for camera controls.
    public Vector3 camPos1 = new Vector3();
    public Vector3 camPos2 = new Vector3();

    public GameObject EmitterObj;
    public GameObject RayCaster;

   [HideInInspector] public ElevationChecker elevationFunction;
    
    // Player object
    public Rigidbody player;

    // Reference to Playerchar Script for abilityLock boolean.
    public PlayerChar ablLock;

    // Reference to Emitter_Centering Script for canTurn boolean.
    [HideInInspector]public Emitter_Centering rotateLock;

    public Camera cam;

    // Animation controller
    public Animator anim;

    // Check if positional coordinates are negative.
    private bool isNegative;

    //Stop movement when attacking
    public bool canMove;
    public bool canTurn;
    public bool isGrounded;     //Boolean to track if the character sticks to ground geometry or not.

    private bool dashLock;      // Check to make sure that the dash is the only action that can occur when holding down space.
    private bool isCharging;    // Check to ensure charging state of the character.
    private bool inTransit;
    private bool dashBuffer;
    private bool dashActive;
    public bool raycastResult;

    public float nextDash;      // Time until next dash can be performed.
    public float dashCD;        // Dash cooldown time.

    // Stop movement after attacking.
    public float meleeDelay;
    public float rangedDelay;
    public float shieldDelay;

    // Variables for Dashing controls
    public float dashSpeed = 5.0f;   // Speed at which the character should travel to the destination point.

    public float dropSpeed;

    private float incSpeed;          // Incrementing speed while charging.

    private float dashDistance;      // Modifier that determines that starting distance of the dash. Scales up as player holds down space.
    public float dashInc;

    public float maxDashDelay;       // Time after hitting the maximum distance that it will dash by default. Might not need this.
    public float dashWinduptime;     // Time before the dash distance starts to increase affter player continues to hold down space.
    public float dashRecoveryTime;   // Time before the player can act after dashing.
    private float travelRate;        // Resulting speed of the character. Time * dashSpeed.
    private float maxIncrease;       // Maximum incrementation for dash distance.
    private float fixer;             // Fixes dash distance to under the maximmum if the incrementation puts it over the limit.
    private float bufferTime;        // Time before incrementation starts charging up for the dash distance.
    
    public bool dashLocked;
    // public GameObject trackerObj; // Object that is spawned or activated that tracks the position that the player will travel to.
    // public GameObject playerObj;

    public Vector3 destPos;         // Position that the dash will travel to that scales up to the max distance. 
    public Vector3 trackerStartPos; // starting position of the obect that will determine end point of the dash.
    public Vector3 maxTravelPos;    // Maximum distance that the character can dash to.
    public Vector3 travelTarget;    // Coordinates where the object will travel to.

    // private Ray groundRay;

    [HideInInspector] public int faceDir;    // Integer that will update and track which direction the player is facing based off of last movement keys pressed.

    private float dashTimeOut;
    private float dashStartTime;

    public float foundElevation;

    void Start()
    {
        canTurn = true;             // Restriction for player rotation.
        canMove = true;             // Start true as the player is not currently dashing.
        isGrounded = true;
        isNegative = false;
        dashBuffer = false;
        dashActive = false;
        dashLocked = false;
       
        startTime = Time.time;

        //dashDistance = 20f;
        dashDistance = 0f;
        maxIncrease = 20.0f;
        dashSpeed = 100.0f;
        maxDashDelay = 1.0f;
        dashRecoveryTime = 0.3f;
        bufferTime = 0.2f;
        dashInc = 2.2f;
        dashLock = false;
        incSpeed = 25f;
        dashCD = 0.35f;
        dropSpeed = 50f;

        EmitterObj = GameObject.FindWithTag("PlayerEmitter"); // Used to find emitter object to set locks.   

    }

    void Update()
    {

        ablLock = gameObject.GetComponent<PlayerChar>(); // Sets lock state to prevent player form using abilities while dashing or charging for dash.

        rotateLock = EmitterObj.GetComponent<Emitter_Centering>(); // Sets lock state to prevent emitter from rotating within the Emitter_Centering Script.




        travelRate = dashSpeed * Time.deltaTime;    // Travel rate that the object will eventually travel to its destination.

        trackerStartPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z); // starting position for the tracker obj.
        // maxTravelPos = new Vector3(gameObject.transform.position.x + 40, gameObject.transform.position.y, gameObject.transform.position.z); // Sets maximum dash distance.


        //Basic Movement with rotation in same direcction as input key w/ attack delays
        if (canMove == true)
        {
            if ((Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A)))
            {

                //transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", true);
                anim.SetBool("back", false);
                anim.SetBool("left", false);
                anim.SetBool("right", false);


            }
            else if ((Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S)))
            {
                //transform.rotation = Quaternion.Euler(new Vector3 (0,0,0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", false);
                anim.SetBool("back", false);
                anim.SetBool("left", false);
                anim.SetBool("right", true);

            }
            else if ((Input.GetKey(KeyCode.S)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A)))
            {
                //transform.rotation = Quaternion.Euler(new Vector3 (0,90,0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", false);
                anim.SetBool("back", true);
                anim.SetBool("left", false);
                anim.SetBool("right", false);

            }
            else if ((Input.GetKey(KeyCode.A)) && (!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S)))
            {
                //transform.rotation = Quaternion.Euler(new Vector3 (0,180,0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", false);
                anim.SetBool("back", false);
                anim.SetBool("left", true);
                anim.SetBool("right", false);

            }
            else if ((Input.GetKey(KeyCode.W)) && (Input.GetKey(KeyCode.D)))
            {
                //transform.rotation = Quaternion.Euler(new Vector3 (0,315,0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", true);
                anim.SetBool("back", false);
                anim.SetBool("left", false);
                anim.SetBool("right", false);

            }
            else if ((Input.GetKey(KeyCode.W)) && (Input.GetKey(KeyCode.A)))
            {
                //transform.rotation = Quaternion.Euler(new Vector3 (0,225,0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", true);
                anim.SetBool("back", false);
                anim.SetBool("left", false);
                anim.SetBool("right", false);

            }
            else if ((Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.D)))
            {
                //transform.rotation = Quaternion.Euler(new Vector3 (0,45,0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", false);
                anim.SetBool("back", true);
                anim.SetBool("left", false);
                anim.SetBool("right", false);

            }
            else if ((Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.A)))
            {
                //transform.rotation = Quaternion.Euler(new Vector3 (0,135,0));
                transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", false);
                anim.SetBool("back", true);
                anim.SetBool("left", false);
                anim.SetBool("right", false);
            }

            else if ((!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.A)) && (!Input.GetKey(KeyCode.S)) && (!Input.GetKey(KeyCode.D)))
            {
                anim.SetBool("forward", false);
                anim.SetBool("back", false);
                anim.SetBool("left", false);
                anim.SetBool("right", false);
            }
        }

        if (canTurn)
        {
            if ((Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A)))  // Moving upwards.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));

                faceDir = 0;

            }
            else if ((Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S))) // Moving right.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                faceDir = 1;
            }


            else if ((Input.GetKey(KeyCode.S)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A))) // Moving down.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                faceDir = 2;
            }

            else if ((Input.GetKey(KeyCode.A)) && (!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S))) // Moving left.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                faceDir = 3;
            }

            else if ((Input.GetKey(KeyCode.W)) && (Input.GetKey(KeyCode.D))) // Moving up-right.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 315, 0));
                faceDir = 4;
            }

            else if ((Input.GetKey(KeyCode.W)) && (Input.GetKey(KeyCode.A)))  // Moving up-left.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 225, 0));
                faceDir = 5;
            }

            else if ((Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.D))) // Moving down-right.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));
                faceDir = 6;
            }

            else if ((Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.A))) // Moving down-left.
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 135, 0));
                faceDir = 7;
            }
        }

        if (isGrounded)  // Check to determine if there is ground underneath the charcter and to attach them to it if there is. Can be switched off for jumping and dashing over pits.
        {
            // float raiser = 5.2744f;

            float raiser = 5.2744f;

            elevationFunction = RayCaster.GetComponent<ElevationChecker>();

            raycastResult = elevationFunction.detectGround;

            if (raycastResult)
            {
                foundElevation = elevationFunction.inclineFinder;

                Vector3 pos = transform.position;

                pos.y = foundElevation + raiser;

                transform.position = pos;
            }
            
        
            /*  Vector3 inclineFinder;
              inclineFinder = new Vector3(transform.position.x, transform.position.y, transform.position.z);

              Ray groundRay = new Ray(inclineFinder, Vector3.down);


             // groundRay.origin = inclineFinder;
             // groundRay.direction = Vector3.down;

              print("ground check starting");

              Debug.DrawRay(inclineFinder, Vector3.down, Color.green);

               RaycastHit hitInfo;

              LayerMask layer = 1 << LayerMask.NameToLayer("Ground");

              if (Physics.Raycast(groundRay, out hitInfo, layer))
              {
                  float y = hitInfo.point.y;

                  Vector3 pos = transform.position;

                  pos.y = y;

                  transform.position = pos;

                  print("Ground Z: " + pos.y);
              } */

        } 

        //Attack delays to restrict movement during attacking state.
        if (!dashLock)
        {
            if (Input.GetMouseButtonDown(0))
            { //left click
                StartCoroutine(AttackDelay(meleeDelay));
            }

            else if (Input.GetMouseButtonDown(1))
            { //right click
                StartCoroutine(AttackDelay(rangedDelay));
            }

            else if (Input.GetKeyDown(KeyCode.LeftShift))
            { //left shift
                StartCoroutine(AttackDelay(shieldDelay));
            }

        }

        //Code to make sure player is withing a grid
        if (player.IsSleeping())
        {
            //MoveToCenter ();
        }

        if (!dashLocked)
        {
            if (Input.GetKeyDown("space") && Time.time > nextDash)  // If player presses space to start dashing and isn't currently dashing. Need to look into managing the abiity to cancel out of abilities and other states to prevent dashing./*&& !dashLock*/
            {
                //print("space down");

                dashActive = true;
                //destPos = new Vector3(trackerStartPos.x, trackerStartPos.y, trackerStartPos.z);  // Sets starting dash destination

                // Locks
                canMove = false;  // Restricts movement.

                ablLock.abilityLock = true;   // Sets abilityLock to true in PlayerChar script to prevent ability usage while space is held down.
                //print("abilities Locked" + ablLock.abilityLock);

                isCharging = true;            // Starts the charging state.
               // print("isCharging" + isCharging);

                dashBuffer = true;

                // if (destPos.x <= maxTravelPos.x && isCharging)
                // if (destPos.x <= maxTravelPos.x)
            }


            if (isCharging)
            {

                StartCoroutine(DashActionBuffer(bufferTime));

                if (!dashBuffer)
                {
                    dashDistance += incSpeed * Time.deltaTime;

                    //print("dashDistance " + dashDistance);
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
               // print("Final dashDistance " + dashDistance);
            }
       

            if (Input.GetKeyUp("space") && Time.time > nextDash && dashActive /*&& dashLock*/)
            {
                print("space up");

                isCharging = false; // turns off isCharging just in case. Character still locked into dash windup state.
               // print("isCharging" + isCharging);

                canTurn = false;

                rotateLock.canRotate = false;

                //print("canTurn " + canTurn);

                inTransit = true;

                //print("inTransit state " + inTransit);

                dashDistance = dashDistance + 20f;
                //print("Totally Final Distance" + dashDistance);

                SetDashDirection(faceDir);

                // dashStartTime = Time.time;

                dashTimeOut = Time.time + 0.4f;
               // print("Dash Time Out " + dashTimeOut);

                nextDash = Time.time + dashCD;


                //print("nextDash " + nextDash);

            }

            if (inTransit)
            {
                if (gameObject.transform.position != travelTarget)  // translate player character to the dash travel destination.

                //if ((gameObject.transform.position.x != travelTarget.x) && (gameObject.transform.position.z != travelTarget.z) || (gameObject.transform.position.x != travelTarget.x) && (gameObject.transform.position.z == travelTarget.z) || (gameObject.transform.position.x == travelTarget.x) && (gameObject.transform.position.z != travelTarget.z))
                {
                    canTurn = false;
                    // gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, travelTarget, travelRate); // translate player character to the dash travel destination.

                    transform.Translate(Vector3.right * Time.deltaTime * dashSpeed);

                   // print("dashing in process");

                    //gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);
                }


                if (Time.time >= dashTimeOut)
                {
                    //print("dash timed out");
                    travelTarget = gameObject.transform.position;
                    inTransit = false;

                }


            }
            //gameObject.transform.Translate(Vector3.right * Time.deltaTime * speed);


            /* if (destPos.x <= maxTravelPos.x)
             {
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, travelTarget, travelRate); 
             } */

            //if ((gameObject.transform.position.x == travelTarget.x) && (gameObject.transform.position.z == travelTarget.z))
            if (gameObject.transform.position == travelTarget)
            {
               // print("DESTINATION REACHED");
               // print("DASH TIME OUT");
                StartCoroutine(DashRecovery(dashRecoveryTime)); // Starts delay on dash recovery after finishing dash travel. 

                dashLock = false;

                ablLock.abilityLock = false;

                //print("abilities Locked " + ablLock.abilityLock);

                dashDistance = 0;

                inTransit = false;

                dashActive = false;
            }

        }


        // Release locks after action is done. This could probably be changed after animations are incorporated to include the animation durations.

        // Set this to the player for it. TravelObj is a child of the object that this script is attached to so that it doesnt have to worry about rotation. 
        // Need to make sure that it locks player movement but not rotation when charging. 
        // Needs to lock ability usage when charging as well.
     

    }
        

        void MoveToCenter()
    {
        //Calculate the center position for x given the square size is 10x10
        float newX = gameObject.transform.position.x;
        newX = (float)Mathf.Floor(newX);

        if (newX < 0)
        {
            isNegative = true;
            newX = Mathf.Abs(newX);
        }

        if (newX % 10 != 5)
        {
            float check = (Mathf.Round(newX / 10.0f) * 10.0f) - newX;
            if (check <= 5 && check >= 0)
            {
                newX = (Mathf.Round(newX / 10.0f) * 10.0f) - 6.0f;
            }
            newX = (5.0f - newX % 5.0f) + newX;

        }
        else if (newX % 10 == 0)
        {
            newX = newX + 5.0f;
        }

        if (isNegative)
        {
            newX = newX * -1;
            isNegative = false;
        }


        //Calculate the center position for z given the square size is 10x10
        float newZ = gameObject.transform.position.z;
        newZ = (float)Mathf.Floor(newZ);

        if (newZ < 0)
        {
            isNegative = true;
            newZ = Mathf.Abs(newZ);
        }

        if (newZ % 10 != 5)
        {
            float check2 = (Mathf.Round(newZ / 10.0f) * 10.0f) - newZ;
            if (check2 <= 5 && check2 >= 0)
            {
                newZ = (Mathf.Round(newZ / 10.0f) * 10.0f) - 6.0f;
            }
            newZ = (5.0f - newZ % 5.0f) + newZ;

        }
        else if (newZ % 10 == 0)
        {
            newZ = newZ + 5.0f;
        }

        if (isNegative)
        {
            newZ = newZ * -1;
            isNegative = false;
        }

        //Move character to the middle of the square
        Vector3 startPos = gameObject.transform.position;
        Vector3 endPos = new Vector3(newX, 3, newZ);
        gameObject.transform.position = Vector3.MoveTowards(startPos, endPos, Time.deltaTime * 500f);
    }

    public void SetDashDirection(int dashDir)
    {
        float xDashDist;
        float zDashDist;

       // print("dashing method");
       // print("dish Direction " + dashDir);

        if (dashDir == 0)
        {
            zDashDist = dashDistance;


            travelTarget = new Vector3(trackerStartPos.x, trackerStartPos.y, trackerStartPos.z + zDashDist); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }

        if (dashDir == 1)
        {
            xDashDist = dashDistance;


            travelTarget = new Vector3(trackerStartPos.x + xDashDist, trackerStartPos.y, trackerStartPos.z); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }

        if (dashDir == 2)
        {
            zDashDist = dashDistance;


            travelTarget = new Vector3(trackerStartPos.x, trackerStartPos.y, trackerStartPos.z - zDashDist); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }

        if (dashDir == 3)
        {
            xDashDist = dashDistance;


            travelTarget = new Vector3(trackerStartPos.x - xDashDist, trackerStartPos.y, trackerStartPos.z); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }

        if (dashDir == 4)
        {
            zDashDist = dashDistance;
            xDashDist = dashDistance;

            travelTarget = new Vector3(trackerStartPos.x + xDashDist, trackerStartPos.y, trackerStartPos.z + zDashDist); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }

        if (dashDir == 5)
        {
            zDashDist = dashDistance;
            xDashDist = dashDistance;

            travelTarget = new Vector3(trackerStartPos.x - xDashDist, trackerStartPos.y, trackerStartPos.z + zDashDist); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }

        if (dashDir == 6)
        {
            zDashDist = dashDistance;
            xDashDist = dashDistance;

            travelTarget = new Vector3(trackerStartPos.x + xDashDist, trackerStartPos.y, trackerStartPos.z - zDashDist); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }

        if (dashDir == 7)
        {
            zDashDist = dashDistance;
            xDashDist = dashDistance;

            travelTarget = new Vector3(trackerStartPos.x - xDashDist, trackerStartPos.y, trackerStartPos.z - zDashDist); // Store trackerObj position for use.
            print("travelTarget.x " + travelTarget.x);
            print("travelTarget.y " + travelTarget.y);
            print("travelTarget.z " + travelTarget.z);

        }
    }

    //Waits for a couple of seconds after a melee attack
    IEnumerator AttackDelay(float delay)
    {
        canMove = false;
        yield return new WaitForSeconds(delay);         //yield for certain amount of seconds
        //Debug.Log("Waited for " + delay + " seconds.");
        canMove = true;
        yield return null;
    }



    IEnumerator DashDelay(float maxDashDelay)
    {
        yield return new WaitForSeconds(maxDashDelay);        //yield for certain amount of seconds
       // Debug.Log("Waited for " + maxDashDelay + " seconds.");
        yield return null;
    }

    IEnumerator DashWindup(float dashWindupTime)
    {
        yield return new WaitForSeconds(dashWindupTime);        //yield for certain amount of seconds
       // Debug.Log("Waited for " + dashWindupTime + " seconds.");

        yield return null;
    }


    IEnumerator DashRecovery(float dashRecoveryTime)
    {
        yield return new WaitForSeconds(dashRecoveryTime);        //yield for certain amount of seconds
      //  Debug.Log("Waited for " + dashRecoveryTime + " seconds.");
        canMove = true;
        canTurn = true;
        rotateLock.canRotate = true;
        yield return null;
    }

    IEnumerator DashActionBuffer(float chargeBuffer)
    {
        yield return new WaitForSeconds(chargeBuffer);        //yield for certain amount of seconds
       // Debug.Log("Waited for " + chargeBuffer + " seconds.");
        dashBuffer = false;
        yield return null;
    }
}

/**currentLerpTime += Time.deltaTime;
if(currentLerpTime >= lerpTime){
    currentLerpTime = lerpTime;
}
float step = currentLerpTime/lerpTime;
Debug.Log (step);
gameObject.transform.position=Vector3.Lerp(startPos, endPos, step);
*/



/**void Update() {
    
    if (Input.GetKey (KeyCode.W)) {
        transform.Translate (Vector3.forward * Time.deltaTime * speed);
    } else if (Input.GetKey (KeyCode.D)) {
        transform.Translate (Vector3.right * Time.deltaTime * speed);
    } else if (Input.GetKey (KeyCode.S)) {
        transform.Translate (Vector3.back * Time.deltaTime * speed);
    } else if (Input.GetKey (KeyCode.A)) {
        transform.Translate (Vector3.left * Time.deltaTime * speed);
    } else if (Input.GetKey (KeyCode.P)) {
        float step = (Time.time - startTime) / 0.5f;
        cam.transform.position = Vector3.Lerp (camPos1, camPos2, step);
    } else if (Input.GetKey (KeyCode.O)) {
        float step = (Time.time - startTime) / 0.5f;
        cam.transform.position = Vector3.Lerp (camPos2, camPos1, step);
    }
}

/**	void OnTriggerStay (Collider col) 
{
    if ((col.gameObject.tag == "grid")) {
        float newX = col.gameObject.transform.position.x;
        float newZ = col.gameObject.transform.position.z;
        float t = 0.0f;
        Vector3 startPos = gameObject.transform.position;
        Vector3 endPos = new Vector3 (newX, 3, newZ);
        if(player.IsSleeping()){
            if(Vector3.Distance(startPos, endPos)>0.1f) {
            float step = (Time.time - startTime)/0.5f;
            gameObject.transform.position = Vector3.Lerp (startPos, endPos, step);
            }
        }
    }
}
} **/
