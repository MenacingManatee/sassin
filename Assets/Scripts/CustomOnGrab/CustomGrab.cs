using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Custom script overriding XR grab. Mainly used to call a script on interactible when grabbed or released
// and fixing collision with grabbed objects to avoid breaking simulation
public class CustomGrab : XRGrabInteractable
{
    [SerializeField]
    private bool disableCollisionOnGrab = false;
    int startLayer;

    // TODO: check layer on grab and save it to a variable here, so non-default layers can be used for grabbables. Non-issue until non-default layers are needed
    protected override void OnSelectEntering(SelectEnterEventArgs args) {
        //transform.position = args.interactor.attachTransform.position;
        CustomOnGrab c = args.interactable.GetComponent<CustomOnGrab>(); // Grabs the OnGrab script from interactable if it exists
        if (c)
            c.OnGrabEnter();
        startLayer = args.interactable.gameObject.layer;
        SetLayer s = args.interactable.GetComponent<SetLayer>();
        if (s)
            s.Set();
        else {
            if (disableCollisionOnGrab) {
                //StartCoroutine(FlickerCollision());
            }
            else
                args.interactable.gameObject.layer = 10; // layer 10 collides with very few things. May be a better way, but this minimizes ways to break the simulation/immersion for now
        }
        base.OnSelectEntering(args);
    }

    protected override void OnSelectExiting(SelectExitEventArgs args) {
        CustomOnGrab c = args.interactable.GetComponent<CustomOnGrab>();
        if (c)
            c.OnGrabExit();
        base.OnSelectExiting(args);
        SetLayer s = args.interactable.GetComponent<SetLayer>();
        if (s)
            s.Set(startLayer);
        else
            args.interactable.gameObject.layer = startLayer; // layer 6 is grabbable default layer
    }

    private IEnumerator FlickerCollision() {
        gameObject.layer = 12; // noCollision layer
        yield return new WaitForSeconds(0.2f);
        gameObject.layer = 10;
    }
}
