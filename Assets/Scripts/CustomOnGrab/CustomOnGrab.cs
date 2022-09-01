using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomOnGrab : MonoBehaviour
{
    // isGrabbed is used for scripts inheriting from this to determine if the gameobject is currently grabbed
    internal bool isGrabbed = false;

    [Header("Events")]
    public UnityEvent Grabbed; // Called once when first grabbed
    public UnityEvent Released;
    public void OnGrabEnter() {
        Grabbed.Invoke();
        isGrabbed = true;
    }

    public void OnGrabExit() { // Called once when grab exits
        Released.Invoke();
        isGrabbed = false;
    }
}
