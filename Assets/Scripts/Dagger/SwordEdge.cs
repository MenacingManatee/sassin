using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls any effects, joints, rotation, and positioning of the edge of a blade.
public class SwordEdge : MonoBehaviour
{
    // the starting position of the collider
    private Vector3 startPos;
    // The starting rotation of the collider
    private Quaternion startRot;

    // Grab the starting values
    void Awake() {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }
    
    // Reset position and rotation on enable
    void OnEnable() {
        var joint = GetComponent<ConfigurableJoint>();
        var rb = joint.connectedBody;
        joint.connectedBody = null;
        transform.localPosition = startPos;
        transform.localRotation = startRot;
        joint.connectedBody = rb;
    }
}
