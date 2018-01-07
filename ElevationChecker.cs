/*
 * Author: Kurt Noe
 * Desc: Creates a raycast that originates from the attached object that is cast straight downards. Will only report back a hit if the object it collides with is part of the "Ground" layer.
 *       If a hit is found it will record and report back the coordinates of the hit location in the variable "inclineFinder". If no ground is found then no values are retruned. Used to determine if there is 
 *       valid ground underneath the player or any object to allow them to traverse up and down inclines and changing geometry. Attached to a seperate object that is a child of the main player object and 
 *       placed higher than the player character to properly check the elevation in relativity to the player.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Class attached to an object which a ray will originate from. Searches for collisions with objects apart of the Ground layer.
 * If ground is detected then save the value of the Y coordinate of the point of contact from the ray. 
 * This value can then be accessed by other scripts for the sake of elevation detection.
*/
public class ElevationChecker : MonoBehaviour 
{
    public bool detectGround;
    [HideInInspector] public float inclineFinder;

    // Use this for initialization
    void Start()
    {
        detectGround = false;
    }

    // Update is called once per frame
    void Update()
    {

        Ray groundRay = new Ray(transform.position, Vector3.down);

        // Debug.DrawRay(groundRay, Vector3.down, Color.green);

        RaycastHit hitInfo;

        if (Physics.Raycast(groundRay, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            detectGround = true;
            inclineFinder = hitInfo.point.y;

            // print("GROUND HEIGHT " + inclineFinder);
            //print("Ground Z: " + inclineFinder);
            // print(hitInfo.collider.gameObject.name);
        }

        if (!Physics.Raycast(groundRay, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Ground")))
        {
            detectGround = false;
        }
    }
}