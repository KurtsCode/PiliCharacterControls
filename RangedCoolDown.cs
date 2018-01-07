/*
 * Author: Kurt Noe
 * Desc: Controller script for the ranged attack. Check for when the Right Mouse Buttom is pressed to then activate the melee ability and pass down the proper values os that it can be executed in the direction the
 *       character is currently facing. Locks are set in place to ensure that the melee attack cannot be initiated from pressing the RMB during certain actions such as dashing or clicking in menus.
 *       Attached to the player character and calls the RangedAbility script when the attack button is pressed.
 * Version: 1.0
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RangedCoolDown : MonoBehaviour // Used to establish the cooldown time of the ability and trigger it once the button is pressed. Also designates audio and visuals when needed.
{

    public string abilityButtonAxisName = "Fire2";    // Appears in Inspector
    // public Image darkMask;
     // public Text coolDownTextDisplay;

    [SerializeField] private Ability ability;         // Appears in Inspector
    [SerializeField] private GameObject weaponHolder; // Appears in Inspector

    [HideInInspector] public PlayerMovementDash charDirect;

    [HideInInspector] public int direction;
    [HideInInspector] public GameObject attackingChar;

    // private Image myButtonImage;
    //private AudioSource abilitySource;


    // FMOD Sound Call
    [FMODUnity.EventRef]
    public string sh_EventPath;

    private PlayerChar getAudio;
    private float coolDownDuration;
    private float nextReadyTime;
    private float coolDownTimeLeft;

    public bool attackLock;

    public GameObject abilityVisual;

    void Start()
    {
        

        Initialize(ability, weaponHolder);
    }

    public void Initialize(Ability selectedAbility, GameObject weaponHolder) // Initializing script.
    {

        attackingChar = gameObject;  // Detects object this script is attached to and sets it as the attacker to be passed to the ability itself.

        ability = selectedAbility;  // Dictates activated ability, should be referencing a specific script and accessing its values through this..

        // myButtonImage = GetComponent<Image>(); // Component for the ui aspect.
        getAudio = GetComponent<PlayerChar>();
        
        //abilitySource = getAudio.musicSource; //Starts Audio
        // myButtonImage.sprite = ability.aSprite; // Sprite for the GUI element.
                                                // darkMask.sprite = ability.aSprite;

        coolDownDuration = ability.aBaseCoolDown;  // Cooldown pulled from ability basecooldown value;

        ability.attacker = attackingChar;

        ability.Initialize(weaponHolder);           // Sets the spawn point of the ability as the object weaponholder.
        AbilityReady();                            // Sets ability as ready.
    }

    // Update is called once per frame
    void Update()
    {

        charDirect = gameObject.GetComponent<PlayerMovementDash>();
        direction = charDirect.faceDir;

        // print("FIRST DIRECTION " + direction);

        bool coolDownComplete = (Time.time > nextReadyTime);  // Check to see if cooldown is available or not. 
        if (coolDownComplete)
        {
            AbilityReady();  // Ability is ready once cooldown is completed.

            if (!attackLock)
            {
                if (Input.GetButtonDown(abilityButtonAxisName)) // Check for button input to trigger associated ability.
                {
                    ButtonTriggered();    // Once button is triggered perform scripted actions.
                }
            }
        }
        else
        {
            CoolDown();     // If cooldown is not complete then run CoolDown function.
        }
    }

    private void AbilityReady()  // Used mainly for GUI elements more than hitbox procedures.
    {
        // coolDownTextDisplay.enabled = false;
        //  darkMask.enabled = false;
    }

    private void CoolDown()
    {
        coolDownTimeLeft -= Time.deltaTime; // Determines cooldown current time left until cooldown is over.
        float roundedCd = Mathf.Round(coolDownTimeLeft); // Rounds CD to ceaner number.
                                                         // coolDownTextDisplay.text = roundedCd.ToString();
                                                         // darkMask.fillAmount = (coolDownTimeLeft / coolDownDuration);
    }

    private void ButtonTriggered()
    {



        nextReadyTime = coolDownDuration + Time.time; // Determines next time the cooldown will be completed.
        coolDownTimeLeft = coolDownDuration;      // Sets coolDownTimeLeft starting time as the coolDownduration current value.

        print("scriptable start");
        // darkMask.enabled = true;
        // coolDownTextDisplay.enabled = true;

        //abilitySource.clip = ability.aSound;  //Finds loaded sound for the ability.
        //abilitySource.Play();           // Play audio from AudioSource
        PlayShortHit();


        ability.TriggerAbility();       //Activates the TriggerAbility function of the ability when the corrresponding button is pressed.
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