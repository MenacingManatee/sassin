using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Allows the player to flip a blade into a reverse grip
public class SwordFlip : MonoBehaviour
{
    [SerializeField, Tooltip("The animator controlling the flip animation")]
    private Animator anim;

    [Tooltip("Is the animator currently in the flipped state")]
    public bool isFlipped = false;

    // blend value to control the point in the animation
    private float blend = 0f;

    // Set the blend value, and update the animation state
    void Update()
    {
        if (isFlipped) {
            blend += Time.deltaTime;
        } else {
            blend -= Time.deltaTime;
        }
        if (blend >= 1)
            blend = 1;
        else if (blend <= 0)
            blend = 0;
        anim.SetFloat("flipBlend", blend);
    }

    // Called to toggle the isFlipped variable. Purpose is to be used with unityevents for on activate
    public void ToggleFlip() {
        isFlipped = !isFlipped;
    }
}
