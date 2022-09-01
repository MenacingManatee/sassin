using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

// Controls the climbing functionality for the player
// Placed on the topmost object for the XR Rig
public class climbing : MonoBehaviour
{
    // Global Vars
    private static XRBaseInteractor _climbingHand = null;
    public static XRBaseInteractor climbingHand // Global variable used for keeping track of which hand is anchored
    {
        get
        {
            return _climbingHand;
        }
        set
        {
            if (_climbingHand != null) {
                ClimbSword s = null;
                if (climbingHand.selectTarget) {
                    s = _climbingHand.selectTarget.GetComponent<ClimbSword>();
                }
                if (s)
                    s.sword.DetachJoint();
            }
            _climbingHand = value;
        }
    }
    // **NOTE** Above section is causing the error where EITHER
    // When disabled - sword must be grabbed before moving on
    // When enabled - sword follows as expected, but once you grab a regular node it can't be released


    [Header("References")]
    [SerializeField, Tooltip("The move provider, to be disabled while climbing")]
    private ActionBasedContinuousMoveProvider move;
    private CharacterController character; // The character controller on the XR Rig
    
    
    // private vars
    private int startLayer; // Layer the player is on before climbing
    private bool movePrevEnabled = true; // Was the move var enabled before climbing
    private bool isFinishedClimbing = true; // Is the player finished climbing

    // Set charactercontroller reference and grab starting layer
    void Start()
    {
        character = GetComponent<CharacterController>();
        startLayer = gameObject.layer;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!climbingHand && isFinishedClimbing) {
            // Debug.Log(string.Format("enabled? {0}", movePrevEnabled));
            movePrevEnabled = move.enabled;
        }
        if (climbingHand) {
            // disable gravity and player stick movement while climbing
            isFinishedClimbing = false;
            move.enabled = false;
            Climb();
            gameObject.layer = 12; // No collision layer
        } else {
            // "move.enabled = movePrevEnabled" in case movement was already disabled
            if (!isFinishedClimbing) {
                isFinishedClimbing = true;
                gameObject.layer = startLayer;
                move.enabled = movePrevEnabled;
            }
        }
    }

    public void Climb() {
        // Way less intimidating than it looks
        // InputDevices.GetDeviceAtXRNode(                                              = Get the InputDevice object using an XRNode
        // climbingHand.gameObject.GetComponent<PhysicsHandController>().controllerNode = Get the XRNode stored in the PhysicsHandController script
        // .TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);      = Get the velocity associated with the InputDevice and save it to variable velocity
        HandInfo info = climbingHand.gameObject.GetComponent<HandInfo>();
        if (!info)
            info = climbingHand.transform.parent.gameObject.GetComponent<HandInfo>();
        InputDevices.GetDeviceAtXRNode(info.controllerNode).TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 velocity);

        // Inverted velocity to anchor player body to hand holding climb point
        // rotation corrects for rotation, time corrects for time
        character.Move(transform.rotation * -velocity * Time.deltaTime);
    }
}
