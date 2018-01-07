/*
 * Author: Kurt Noe
 * Desc: Pulls the resulting coordinates of the Raycast created in the ElevationChecker script to determine if there is traversable geometry that is tagged as "Ground" underneath the assigned object.
 *       If traversable ground is discovered then it will transform the Y position of the object the script is attached to match the elevation of the ground under the object. The float variable "raiser" is
 *       used to offset the the object if needed to prevent it from being placed under or halfway into the ground geometry. Will adjust the elevation to whatever game object it is attached to.
 * Version: 1.0
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevationAdjuster : MonoBehaviour
{
    public bool isGrounded;
    [HideInInspector] public float inclineFinder;
    [HideInInspector] public ElevationChecker elevationFunction;
    [HideInInspector] public bool raycastResult;
    public float foundElevation;
    public GameObject RayCaster;
    public float raiser;

    // Use this for initialization
    void Start()
    {
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (isGrounded)
        {

            elevationFunction = RayCaster.GetComponent<ElevationChecker>();

            raycastResult = elevationFunction.detectGround;

            // print("RAY RESULT " + raycastResult);

            if (raycastResult)
            {
                foundElevation = elevationFunction.inclineFinder;

                Vector3 pos = transform.position;

                pos.y = foundElevation + raiser;

                transform.position = pos;
            }
        }
    }
}
