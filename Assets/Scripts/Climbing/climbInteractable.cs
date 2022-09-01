using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

// Class is applied in place of the standard grab interactable script
// allows player to grab object and anchor their movement to the object's location, moving their body around the object
// instead of the object around their body
public class climbInteractable : XRBaseInteractable
{
    [Header("Boolean Toggles")]
    [SerializeField, Tooltip("Allow the player to slide down object using local rotation of object")]
    private bool canSlide = false;

    [SerializeField, Tooltip("Allow the player to climb over object by bringing grab hand to waist level")]
    private bool canClimbOver = false;

    [SerializeField, Tooltip("Highlight the interactable on hovering with the controller")]
    private bool highlightOnHover = false;

    [SerializeField, Tooltip("If highlighOnHover == true, highlight children with parents?")]
    public bool includeChildren = false;


    [Header("Variables")]
    [SerializeField, Tooltip("Layermask used when detecting viable surfaces for climbing over/onto")]
    private LayerMask mask =  9; // 9 is the default layer and the ignore grab raycast layer (1001)

    [SerializeField, Tooltip("If left unset program will automatically try to find it on first hover.")]
    private Material startMat;

    [SerializeField, Tooltip("Material to be used on highlight, only needed if highlighOnHover set to true")]
    private Material highlightMat;


    // Reference to character controller
    private CharacterController controller;
    
    // is the player currently sliding down this object
    private bool isSliding = false;

    // is this object currently being grabbed. Warning about unused set var disabled because variable is still useful even though its not currently used
    #pragma warning disable 0414
    private bool grabbing = false;
    #pragma warning restore 0414

    void Start() {
        // grabs the character controller for the purpose of climbing over objects
        controller = GameObject.Find("XR Rig").GetComponent<CharacterController>();
    }
    void Update() {
        // If you are touching the ground, prevents infinite sliding. Character controller handles grounded detection
        if (controller.isGrounded) {
            isSliding = false;
        }
        // Slide math if you are currently sliding
        if (isSliding) {
            // rotation of grab interactable
            // --imperfect due to visual angle not matching inspector angle--, but an invisible collider with correct orientation
            // can be placed over/under the visible object, assuming it doesn't move
            var ang = transform.localRotation;
            var rot = ang * Physics.gravity;
            controller.Move(rot * 0.15f * Time.deltaTime);
        }
    }

    // On grab
    protected override void OnSelectEntered(SelectEnterEventArgs args) {
        base.OnSelectEntered(args);
        // Sets static variable for hand currently anchored for climbing
        grabbing = true;
        climbing.climbingHand = args.interactor;
    }

    // On grab release
    protected override void OnSelectExited(SelectExitEventArgs args) {
        base.OnSelectExited(args);
        // Unsets static variable for hand currently anchored for climbing
        if (climbing.climbingHand && climbing.climbingHand.name == args.interactor.name) {
            climbing.climbingHand = null;
        }
        if (climbing.climbingHand == null) {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, -Vector3.up, out hit, controller.height * 3, mask)) {
                climbInteractable c = hit.collider.gameObject.GetComponent<climbInteractable>();
                if ((c && c.canClimbOver == true) || hit.distance < controller.height * 2) { // Player is above platform
                    Vector3 newPos = hit.point;
                    newPos.y += controller.height;
                    controller.gameObject.transform.position = newPos;
                } else {
                    gameObject.GetComponent<MeshRenderer>().material.color = Color.blue;
                }
            }
        }
        grabbing = false;
        isSliding = false;
    }

    // If the player presses trigger while holding grip on climbable
    protected override void OnActivated(ActivateEventArgs args) {
        base.OnActivated(args);
        // check if object can be slid down
        if (canSlide && args.interactor.name == climbing.climbingHand.name) {
            isSliding = true;
        } else {
            isSliding = false;
        }
    }

    // If the player releases trigger while holding grip on climbable
    protected override void OnDeactivated(DeactivateEventArgs args) {
        base.OnDeactivated(args);
        isSliding = false;
    }

    protected override void OnHoverEntered(HoverEnterEventArgs args) {
        base.OnHoverEntered(args);
        if (highlightOnHover && args.interactor != climbing.climbingHand) {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            MeshRenderer[] childRenderer = null;
            if (!renderer || includeChildren)
                childRenderer = GetComponentsInChildren<MeshRenderer>();
            if (startMat == null) {
                if (renderer) {
                    startMat = renderer.material;
                }
                if (childRenderer != null) {
                    startMat = childRenderer[0].material;
                }
            }
            if (renderer)
                renderer.material = highlightMat;
            if (childRenderer != null) {
                foreach (var render in childRenderer) {
                    render.material = highlightMat;
                }
            }
        }
    }

    protected override void OnHoverExited(HoverExitEventArgs args) {
        base.OnHoverExited(args);
        if (highlightOnHover && !isHovered) {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            MeshRenderer[] childRenderer = null;
            if (!renderer || includeChildren)
                childRenderer = GetComponentsInChildren<MeshRenderer>();
            if (renderer) {
                renderer.material = startMat;
            }
            if (childRenderer != null) {
                foreach (var render in childRenderer) {
                    render.material = startMat;
                }
            }
        }
    }
}
