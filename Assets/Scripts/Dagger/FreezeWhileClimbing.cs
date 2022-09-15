using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class FreezeWhileClimbing : MonoBehaviour
{
    public Stabber stab;
    public Rigidbody rb;

    bool gripPressed = false;

    // Update is called once per frame
    void Update()
    {
        if (gripPressed && stab.stabbed.Count > 0) {
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
        }
    }

    public void OnSqueeze() {
        gripPressed = true;
    }

    public void OnUnsqueeze() {
        rb.constraints = RigidbodyConstraints.None;
        gripPressed = false;
    }
}
