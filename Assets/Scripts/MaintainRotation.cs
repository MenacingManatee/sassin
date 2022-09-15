using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainRotation : MonoBehaviour
{
    public bool x;
    public bool y;
    public bool z;
    private Quaternion startRot;
    // Start is called before the first frame update
    void Start()
    {
        startRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion newRot;
        if (x && transform.localRotation.x != startRot.x) {
            newRot = transform.localRotation;
            newRot.x = startRot.x;
            transform.localRotation = newRot;
        }
        if (y && transform.localRotation.y != startRot.y) {
            newRot = transform.localRotation;
            newRot.y = startRot.y;
            transform.localRotation = newRot;
        }
        if (z && transform.localRotation.z != startRot.z) {
            newRot = transform.localRotation;
            newRot.z = startRot.z;
            transform.localRotation = newRot;
        }
    }
}
