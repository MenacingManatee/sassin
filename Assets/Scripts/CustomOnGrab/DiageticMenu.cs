using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiageticMenu : CustomOnGrab
{
    [Header("Boolean Toggles")]
    [SerializeField, Tooltip("Should the gameobject be highlighted using the following colors")]
    private bool highlight = false;


    [Header("Variables")]
    [SerializeField, Tooltip("Starting highlight color")]
    private Color hColor1;

    [SerializeField, Tooltip("Highlight color after condition triggered")]
    private Color hColor2;


    [Header("References")]
    [SerializeField, Tooltip("The menu to be enabled upon being grabbed")]
    private GameObject menu;
    [SerializeField, Tooltip("A list of other menus that should always be disabled if this one is enabled. Sanity check")]
    private GameObject[] disableMenus;
    

    // private vars
    private Vector3 startPos; // start position of the object, used to move back after released
    private Quaternion startRot; // Same as above but rotation
    private Material startMaterial; // Starting material, to have color shifted
    private Material shiftMat; // material to be shifted based on starting material
    private Color hColor; // copy of either hColor1 or hColor2 depending on isDone bool
    private float lerpVal = 0f, lerpDur = 1.5f, timer = 0f; // values used for pulsing highlight color on material
    private bool toFlash = false, toStart = false, isDone = false; // values used to determine which direction to lerp, and which hColor to use
    private MeshRenderer mesh; // Reference to mesh of object
    
    // Set all private references
    void Start() {
        mesh = GetComponent<MeshRenderer>();
        startMaterial = mesh.material;
        shiftMat = new Material(startMaterial);
        mesh.material = shiftMat;
        startPos = gameObject.transform.position;
        startRot = gameObject.transform.localRotation;
        hColor1 = new Color(hColor1.r * 2, hColor1.g * 2, hColor.b * 2); // multiplying times 2 makes color brighter and less subtle
        hColor2 = new Color(hColor2.r * 2, hColor2.g * 2, hColor2.b * 2);
    }

    // Update is called once per frame
    void Update() {
        // Temporary check, pending survey completion message being added
        if (isGrabbed)
            isDone = true;
        // highlight is set in the inspector
        if (highlight) {
            // toggle for which highlight color to use
            hColor = isDone ? hColor2 : hColor1;
            // Delay between finishing pulse and starting pulse
            timer += Time.deltaTime;
            if (timer >= .5f && !toFlash && !toStart)
                toFlash = true;
            if (toFlash) {
                if (lerpVal < .8) {
                    lerpVal += Time.deltaTime / lerpDur;
                    shiftMat.color = Color.Lerp(startMaterial.color, hColor, lerpVal);
                } else {
                    toFlash = false;
                    toStart = true;
                    lerpVal = 0f;
                }
            } else if (toStart) {
                if (lerpVal < 1) {
                    lerpVal += Time.deltaTime / lerpDur;
                    shiftMat.color = Color.Lerp(shiftMat.color, startMaterial.color, lerpVal);
                } else {
                    toStart = false;
                    lerpVal = 0f;
                    timer = 0f;
                }
            }
        }
    }

    // Unused for now, shows the menu when called
    public void ShowMenu() {
        foreach (var dMenu in disableMenus) { // sanity check
            dMenu.SetActive(false);
        }
        menu.SetActive(true);
    }

    // Called on grab released unityevent
    public void JumpToStartPos() {
        StartCoroutine(DoJump());
    }

    // Jumps to start position, rotation, with 0 velocity after a short delay.
    private IEnumerator DoJump() {
        yield return new WaitForSeconds(1.5f);
        gameObject.transform.position = startPos;
        gameObject.transform.localRotation = startRot;
        var v = GetComponent<Rigidbody>();
        v.velocity = Vector3.zero;
        v.angularVelocity = Vector3.zero;
    }
}
