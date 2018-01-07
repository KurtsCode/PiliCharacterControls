/*
 * Author: Kurt Noe 
 * Desc: Attached to the emitter object that acts as the source for the players attacks based on a grid system. Determines which space in a grid consisting of 10x10 sized spaces the player character is currently 
 *       in and places the emitter into that square. Also rotates the emitter object based on the last button pressed by the player to determine which direction the attack will travel to.  
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Emitter_Centering : MonoBehaviour
{
    // public float speed;
    // public float speed = 2.0f;
    // public float smooth = 2.0f;
    public float rotationDirection; //added for rotating
    public float emitterElevation;
    public Rigidbody player;
    // private float startTime;
    // public Camera cam;
    // public Vector3 camPos1 = new Vector3();
    // public Vector3 camPos2 = new Vector3();
    // public Animator anim;
    private bool isNegative;
    // private float lerpTime = 0.5f;
    // private float currentLerpTime = 0;
    //for stop movement when attacking
    public bool canRotate;
    public bool turnCheck;

    public float meleeDelay;
    public float rangedDelay;
    public float shieldDelay;


    void Start()
    {
        canRotate = true;
        //startTime = Time.time;
        isNegative = false;
        turnCheck = false;
    }

    void Update()
    {

        // Find and update the Y value based off the player position so that the emitter is on the ground.
        emitterElevation = player.transform.position.y - 3.7644f;

        //Basic Movement with rotation in same direcction as input key w/ attack delays
        if (canRotate)
        {
            if ((Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 270, 0));
                /* transform.Translate(Vector3.right * Time.deltaTime * speed);
                 anim.SetBool("forward", true);
                 anim.SetBool("back", false);
                 anim.SetBool("left", false);
                 anim.SetBool("right", false); */

            }
            else if ((Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                /* transform.Translate(Vector3.right * Time.deltaTime * speed);
                 anim.SetBool("forward", false);
                 anim.SetBool("back", false);
                 anim.SetBool("left", false);
                 anim.SetBool("right", true); */

            }
            else if ((Input.GetKey(KeyCode.S)) && (!Input.GetKey(KeyCode.D)) && (!Input.GetKey(KeyCode.A)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                /* transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", false);
                anim.SetBool("back", true);
                anim.SetBool("left", false);
                anim.SetBool("right", false); */

            }
            else if ((Input.GetKey(KeyCode.A)) && (!Input.GetKey(KeyCode.W)) && (!Input.GetKey(KeyCode.S)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                /*  transform.Translate(Vector3.right * Time.deltaTime * speed);
                  anim.SetBool("forward", false);
                  anim.SetBool("back", false);
                  anim.SetBool("left", true);
                  anim.SetBool("right", false); */

            }
            else if ((Input.GetKey(KeyCode.W)) && (Input.GetKey(KeyCode.D)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 315, 0));
                /* transform.Translate(Vector3.right * Time.deltaTime * speed);
                 anim.SetBool("forward", false);
                 anim.SetBool("back", true);
                 anim.SetBool("left", false);
                 anim.SetBool("right", false); */

            }
            else if ((Input.GetKey(KeyCode.W)) && (Input.GetKey(KeyCode.A)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 225, 0));
                /* transform.Translate(Vector3.right * Time.deltaTime * speed);
                anim.SetBool("forward", false);
                anim.SetBool("back", true);
                anim.SetBool("left", false);
                anim.SetBool("right", false); */

            }
            else if ((Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.D)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 45, 0));
                /*  transform.Translate(Vector3.right * Time.deltaTime * speed);
                  anim.SetBool("forward", true);
                  anim.SetBool("back", false);
                  anim.SetBool("left", false);
                  anim.SetBool("right", false); */

            }
            else if ((Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.A)))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 135, 0));
                /* transform.Translate(Vector3.right * Time.deltaTime * speed);
                 anim.SetBool("forward", true);
                 anim.SetBool("back", false);
                 anim.SetBool("left", false);
                 anim.SetBool("right", false); */
            }
        }

        PositionCheck();

        
        //Attack delays
        if (Input.GetMouseButtonDown(0) && turnCheck)
        { //left click
            StartCoroutine(AttackDelay(meleeDelay));
        }

        else if (Input.GetMouseButtonDown(1) && turnCheck)
        { //right click
            StartCoroutine(AttackDelay(rangedDelay));
        }

        else if (Input.GetKeyDown(KeyCode.LeftShift) && turnCheck)
        { //left shift
            StartCoroutine(AttackDelay(shieldDelay));
        }

    
    }

    void MoveToCenter()
    {
        //Calculate the center position for x given the square size is 10x10
        float newX = player.transform.position.x;
        newX = (float)Mathf.Floor(newX);

        if (newX < 0)
        {
            isNegative = true;
            newX = Mathf.Abs(newX);
        }

        if (newX % 10 != 5)
        {
            float check = (Mathf.Round(newX / 10.0f) * 10.0f) - newX;
            if (check <= 5 && check > 0)
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
        float newZ = player.transform.position.z;
        newZ = (float)Mathf.Floor(newZ);

        if (newZ < 0)
        {
            isNegative = true;
            newZ = Mathf.Abs(newZ);
        }

        if (newZ % 10 != 5)
        {
            float check2 = (Mathf.Round(newZ / 10.0f) * 10.0f) - newZ;
            if (check2 <= 5 && check2 > 0)
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

        Vector3 endPos = new Vector3(newX, emitterElevation, newZ);
        // gameObject.transform.position = Vector3.MoveTowards(startPos, endPos, Time.deltaTime * 500f);
        gameObject.transform.position = new Vector3(endPos.x, endPos.y, endPos.z);
    }

    void PositionCheck()
    {
        float startX = gameObject.transform.position.x;
        float startZ = gameObject.transform.position.z;

        float charX = player.transform.position.x;
        float charZ = player.transform.position.z;

        if ((startX - charX) >= 5 || (startX - charX) <= -5)
        {
            MoveToCenter();
        }

        if ((startZ - charZ) >= 5 || (startZ - charZ) <= -5)
        {
            MoveToCenter();
        }
    }

  

    //Waits for a couple of seconds after a melee attack
    IEnumerator AttackDelay(float delay)
    {
        canRotate = false;
        yield return new WaitForSeconds(delay);         //yield for certain amount of seconds
        Debug.Log("Waited for " + delay + " seconds.");
        canRotate = true;
        yield return null;
    }
}


