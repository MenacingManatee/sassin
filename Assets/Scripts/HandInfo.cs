using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Contains info for reference on each hand. May be updated further in the future as needs increase, may be moved to other scripts if needs stay consistent
public class HandInfo : MonoBehaviour
{
    // Copied directly from XRController script with minor edit to docs due to not existing in actionbasedcontroller script
    // Only used to tell climbing script which XRNode to use to check controller velocity
    [AddComponentMenu("XR/XR Controller (Device-based)")]
    [SerializeField, Tooltip("The XR node associated with this hand")]
    XRNode m_ControllerNode = XRNode.RightHand;
    public XRNode controllerNode
    {
        get => m_ControllerNode;
        set => m_ControllerNode = value;
    }
    public GameObject hand;
}
