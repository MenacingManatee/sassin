using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClimbSword : CustomGrab
{
    [SerializeField, Tooltip("Reference to the SwordTip script on the sword tip object")]
    internal SwordTip sword;

    // The interactor (hand), used for setting climbing hand
    private XRBaseInteractor i;
    // Reference to character controller
    private CharacterController controller;
    // Reference to the SwordFlip script
    private SwordFlip flip;

    // Set all private refernces
    void Start() {
        // grabs the character controller for the purpose of climbing over objects
        controller = GameObject.Find("XR Rig").GetComponent<CharacterController>();
        flip = GetComponent<SwordFlip>();
    }

    // Set interactor on grabbing the sword
    protected override void OnSelectEntering(SelectEnterEventArgs args) {
        base.OnSelectEntering(args);
        i = args.interactor;
    }

    // Unset interactor and climbing hand (conditional) on dropping the sword, and ensures tracking is correct
    protected override void OnSelectExiting(SelectExitEventArgs args) {
        base.OnSelectExiting(args);
        if (climbing.climbingHand && climbing.climbingHand.name == args.interactor.name)
            climbing.climbingHand = null;
        i = null;
        trackPosition = true;
        trackRotation = true;
        flip.isFlipped = false;
    }

    // Sets the climbing hand
    public void SetClimbingHand() {
        if (i != null)
            climbing.climbingHand = i;
    }
}
