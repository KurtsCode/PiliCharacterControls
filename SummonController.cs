/*
 * Author: Kurt Noe
 * Desc: Detects the player input to activate the function that allows the player to cast abilities through the pressing of the assigned hotkeys. Once an ability is selected and cast it will 
 *       decide which summon asset has been assigned to the player and which ability of that summon was selected. Once all of that has been found it will pass the proper values down to be executed 
 *       into the game space. Detects if the player presses Q or E to select one of the two equipped summons and on LMB or RMB click casts a ray from the main camera to the location of the mouse click and saves
 *       the hit location to determine the destination that ability will be cast to. Attached to the player character object.
 * Version: 1.0
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonController : MonoBehaviour {

    // Values related to Summon handling
    private AudioSource summonSource;                   // Audio to play when summon occurs.
    private PlayerChar getAudio;                        // Audio object from Player.

    public string sumAbilityButton1 = "Fire1";          // String for first ability summon button. Used for possible in game key remapping. Unused currently.
    public string sumAbilityButton2 = "Fire2";          // String for second ability summon button.

    [SerializeField] private Summon firstSummon;        // Object that stores the first summon asset with all its given values
    [SerializeField] private Summon secondSummon;       // Stores second summon asset.
    [SerializeField] private GameObject summonHolder;   // Defines object to act as the origin point that summons will appear from.

    [HideInInspector] public Summon selectedSummon;                      // Holds the summon that was selected by the user.


    // FMOD Sound Call
    [FMODUnity.EventRef]
    public string sh_EventPath;

    private float unlockBuffer;

    // Values passed to the Summon
    [HideInInspector] public int direction;             // Direction that the player is facing.
    [HideInInspector] public GameObject attackingChar;  // Character that the ability is sourced from, used for damage and hit calculations.
     public PlayerMovementDash charDirect; 
    [HideInInspector] public Ability selectedAbility;   // Represents the ability selected by the user after clicking the left or right mouse buttons.
    [HideInInspector] public Vector3 targetPosition;    // Used to store the resulting location of camera sourced raycast hit.
    public MeleeCoolDown meleeLocker;
     public RangedCoolDown rangedLocker;
    // public PlayerMovementDash dashLocker;

    // Values related to Summon Ability Cooldowns.
    private float firstSumCooldown;                     // Duration of the cooldown for the first summon that is determined by the cooldown value of the ability used by the character. 
    private float secondSumCooldown;                    // Duration of the second summon cooldown. 
    [HideInInspector] public float firstNextReadyTime;                   // Next time the ability can be used.
    [HideInInspector] public float secondNextReadyTime;                  // Next time the ability can be used.
    private float firstCoolDownTimeLeft;                // Time remaining beforet he ability is off cooldown.
    private float secondCoolDownTimeLeft;               // Time remaining beforet he ability is off cooldown.

    public bool castActive;                             // States whether the player is in the casting state or not.
    public bool summonActive;

    public int summonSlot;

    [HideInInspector] public int sumChosen;                              // Integer to define which summon was chosen by the player. 0 = none, 1 = First Summon, 2 = Second Summon.

   

    // Values related to handling Raycasting and movement. Needs pruning. Check MovingHitboxSpawn() for further reference.

    // NEED CATCHES FOR WHEN SUMMONS AREN"T EQUIPPED.

    // Use this for initialization
    void Start () {
        

        firstNextReadyTime = 0.0f;
        secondNextReadyTime = 0.0f;
        unlockBuffer = 0.05f;
      

        Initialize(firstSummon, secondSummon, summonHolder);
    }



    public void Initialize(Summon firstSummon, Summon secondSummon, GameObject summonHolder)
    {
       

     

        print("CONTROLLER ATTACKER " + attackingChar);


        castActive = false;
        sumChosen = 0; 

        attackingChar = gameObject;  // Detects object this script is attached to and sets it as the attacker to be passed to the ability itself.

        // myButtonImage = GetComponent<Image>(); // Component for the ui aspect.

        getAudio = GetComponent<PlayerChar>();

        summonSource = getAudio.musicSource;  //Starts Audio
                                              // myButtonImage.sprite = ability.aSprite; // Sprite for the GUI element.
                                              // darkMask.sprite = ability.aSprite;

        firstSumCooldown = firstSummon.aBaseCoolDown;  // Cooldown pulled from ability basecooldown value;
        secondSumCooldown = secondSummon.aBaseCoolDown;

     
        // NEED TO TEST FOR PROPER SOUNDS TO PLAY WHEN THE REST OF THE ACTIONS WORK.

        // AbilityReady(); // Used for a ui element for the abilities to show cooldowns, keep inactive for now.
    }

    // Update is called once per frame
    void Update()
    {
        meleeLocker = gameObject.GetComponent<MeleeCoolDown>();
        rangedLocker = gameObject.GetComponent<RangedCoolDown>();
        charDirect = gameObject.GetComponent<PlayerMovementDash>();

        print("Start state of Melee Lock " + meleeLocker.attackLock);
        print("Start state of Ranged Lock " + rangedLocker.attackLock);
        print("Dash Locker State " + charDirect.dashLocked);

       
        direction = charDirect.faceDir;


        bool firstCoolDownComplete = (Time.time > firstNextReadyTime);  // Check to see if cooldown is available or not. 
        bool secondCoolDownComplete = (Time.time > secondNextReadyTime);  // Check to see if cooldown is available or not. 

        if (firstCoolDownComplete && !summonActive)
        {
            // print("First Summon Cooldown Complete");
            if (Input.GetKeyDown("q"))          // Checks for pressing the Q key to start casting for first summon. Look into possibly changing this so it pulls the string from a variable for more flexbility.
            {
                castActive = true;

                sumChosen = 1;
                
                print("First summon ready");

                meleeLocker.attackLock = true;
                rangedLocker.attackLock = true;
                //charDirect.dashLocked = true;
                print("Dash Locker State " + charDirect.dashLocked);


            }

            if (Input.GetKeyDown(KeyCode.Tab))      // Checks for pressing the Space key to deactivate the casting state. NOTE: This and the left/right clicks still need locks on abilities in other functions.
            {
                castActive = false;
                sumChosen = 0;
                
                print("Summon Deactivated");

                meleeLocker.attackLock = false;
                rangedLocker.attackLock = false;
                
            }
            /*
            if (Input.GetKeyUp("space"))
            {
                //charDirect.dashLocked = false;
               StartCoroutine("DashUnlockDelay");
            }
            */
            
        }
        else
        {
            print("FIRST SUMMON COOLING DOWN");
            FirstCoolDown();                         // If cooldown is not complete then run CoolDown function to update the display for the ui. CURRENTLY NOT USED FULLY.
        }



        if (secondCoolDownComplete && !summonActive)
        {
            // print("Second Summon Cooldown Complete");
            if (Input.GetKeyDown("e"))          // Checks for pressing the E key to start casting for second summon.
            {
                castActive = true;
                sumChosen = 2;
                print("Second summon ready");

                meleeLocker.attackLock = true;
                rangedLocker.attackLock = true;
                //charDirect.dashLocked = true;

            }

            if (Input.GetKeyDown(KeyCode.Tab))      // Checks for pressing the Space key to deactivate the casting state. NOTE: This and the left/right clicks still need locks on abilities in other functions.
            {
                castActive = false;
                sumChosen = 0;
                print("Summon Deactivated");

                meleeLocker.attackLock = false;
                rangedLocker.attackLock = false;
            }

           /* if (Input.GetKeyUp(KeyCode.Tab))
            {
                StartCoroutine("DashUnlockDelay");
               
            } */
        }
        else
        {
            print("SECOND SUMMON COOLING DOWN");
            SecondCoolDown();                         // If cooldown is not complete then run CoolDown function to update the display for the ui. CURRENTLY NOT USED FULLY.
        }


        if (castActive && sumChosen == 1)   // If the player has pressed the Q key to activate casting, pull from the two abilities assigned to the given first summon.
        {
           
            if (Input.GetButtonDown("Fire1"))  // If assigned button is pressed then perform proper actions for the given ability. NOTE: Take another look at this, the process seems wrong, might not need clone yet.
            {
                selectedSummon = firstSummon;

                selectedSummon.attacker = attackingChar;

                selectedSummon.selectedSprite = selectedSummon.sumChar;

                selectedSummon.summonSlot = sumChosen;

                selectedAbility = selectedSummon.ability1;

                selectedSummon.selectedAbility = selectedAbility;

                SetTargetPosition();

                selectedSummon.Initialize(summonHolder);           // Sets the spawn point of the ability as the object weaponholder.

                castActive = false;

                summonActive = true;

               
                print("cast deactivated");

                // firstNextReadyTime = firstSumCooldown + Time.time; // Sets the ready time for the summon based off cooldown. This value is what handles the checking for the cooldown.
                firstCoolDownTimeLeft = firstSumCooldown;  // Updated to update the display for the player.



                

                selectedSummon.TriggerAbility();           // Runs TriggerAbility of the summon to then run the given ability.

                StartCoroutine(AbilityUnlockerBuffer(unlockBuffer));

                //meleeLocker.attackLock = false;
                //rangedLocker.attackLock = false;
            }

            if (Input.GetButtonDown("Fire2"))
            {
                selectedSummon = firstSummon;

                selectedSummon.attacker = attackingChar;

                selectedSummon.summonSlot = sumChosen;

                selectedSummon.selectedSprite = selectedSummon.sumChar2;

                selectedAbility = selectedSummon.ability2;

                selectedSummon.selectedAbility = selectedAbility;

                SetTargetPosition();

                selectedSummon.Initialize(summonHolder);           // Sets the spawn point of the ability as the object weaponholder.

                castActive = false;
                print("cast deactivated");

                firstNextReadyTime = firstSumCooldown + Time.time; // Sets the ready time for the summon based off cooldown.
                firstCoolDownTimeLeft = firstSumCooldown;

                

                selectedSummon.TriggerAbility();

                meleeLocker.attackLock = false;
                rangedLocker.attackLock = false;
               // charDirect.dashLocked = false;

            }
        }


        if (castActive && sumChosen == 2)   // If the player has pressed the E key to activate casting, pull from the two abilities assigned to the given second summon.
        {

           

            if (Input.GetButtonDown("Fire1"))
            {

                selectedSummon = secondSummon;

                selectedSummon.attacker = attackingChar;

                selectedSummon.summonSlot = sumChosen;

                selectedSummon.selectedSprite = selectedSummon.sumChar;

                selectedAbility = selectedSummon.ability1;  // This might be a thing that leads nowhere GIVE IT ANOTHER LOOK

                selectedSummon.selectedAbility = selectedAbility;

                SetTargetPosition();

                selectedSummon.Initialize(summonHolder);           // Sets the spawn point of the ability as the object weaponholder.

                castActive = false;


                summonActive = true;

                print("cast deactivated");

                //secondNextReadyTime = secondSumCooldown + Time.time; // Sets the ready time for the summon based off cooldown.
                secondCoolDownTimeLeft = secondSumCooldown;

                selectedSummon.TriggerAbility();

                StartCoroutine(AbilityUnlockerBuffer(unlockBuffer));

                
            }

            if (Input.GetButtonDown("Fire2"))
            {
                selectedSummon = secondSummon;

                selectedSummon.attacker = attackingChar;

                selectedSummon.summonSlot = sumChosen;

                selectedSummon.selectedSprite = selectedSummon.sumChar2;

                selectedAbility = selectedSummon.ability2;  // This might be a thing that leads nowhere GIVE IT ANOTHER LOOK

                selectedSummon.selectedAbility = selectedAbility;

                SetTargetPosition();

                selectedSummon.Initialize(summonHolder);           // Sets the spawn point of the ability as the object weaponholder.

                castActive = false;


                summonActive = true;

                print("cast deactivated");

                //secondNextReadyTime = secondSumCooldown + Time.time; // Sets the ready time for the summon based off cooldown.
                secondCoolDownTimeLeft = secondSumCooldown;

                selectedSummon.TriggerAbility();

                StartCoroutine(AbilityUnlockerBuffer(unlockBuffer));
            }
        }
    }

    private void FirstCoolDown()
    {
        firstCoolDownTimeLeft -= Time.deltaTime; // Determines cooldown current time left until cooldown is over.
        float firstRoundedCd = Mathf.Round(firstCoolDownTimeLeft); // Rounds CD to ceaner number for displaying in the ui.
        // coolDownTextDisplay.text = roundedCd.ToString();
        // darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);                                                     

    }

    private void SecondCoolDown()
    {
        secondCoolDownTimeLeft -= Time.deltaTime; // Determines cooldown current time left until cooldown is over.
        float secondRoundedCd = Mathf.Round(secondCoolDownTimeLeft); // Rounds CD to ceaner number to display in the ui.
        // coolDownTextDisplay.text = roundedCd.ToString();
        // darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
    }

    /* 
    * Creates a ray that looks for collision with the ground or plane, 
    * saves the coordinates once it hits valid geometry to use for summon aiming and calculation.
    */
    void SetTargetPosition() 
        {
            Plane plane = new Plane(Vector3.up, transform.position);
            Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            float point = 0f;

            if (plane.Raycast(camRay, out point))
            {
             targetPosition = camRay.GetPoint(point);

             selectedSummon.castTarget = targetPosition; // Passes the found hit position Vector3 to the selected Summon.

            print("targetPosition result " + targetPosition);
            }


            // isMoving = true;
            // casting = false;
        }

    IEnumerator AbilityUnlockerBuffer(float unlockBuffer)
    {
        yield return new WaitForSeconds(unlockBuffer);        //yield for certain amount of seconds
        Debug.Log("Waited for " + unlockBuffer + " seconds.");
        meleeLocker.attackLock = false;
        rangedLocker.attackLock = false;
        yield return null;
    }


    IEnumerator DashUnlockDelay()
    {
        yield return new WaitForSeconds(0.3f);
       charDirect.dashLocked = false;
        print("undone dash lock" + charDirect.dashLocked);
        yield return null;
    }

    public void PlayShortHit()
    {
        if (sh_EventPath != null)
        {
            FMOD.Studio.EventInstance e = FMODUnity.RuntimeManager.CreateInstance(sh_EventPath);

            e.start();
            e.release();//Release each event instance immediately, there are fire and forget, one-shot instances. 
        }
    }

}
