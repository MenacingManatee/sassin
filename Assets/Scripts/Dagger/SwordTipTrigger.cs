using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls whether the sword is usable for climbing
public class SwordTipTrigger : MonoBehaviour
{
    [System.NonSerialized]
    public GameObject obj;
    public SwordTip tip;

    void OnTriggerExit(Collider col) {
        if (col.gameObject == obj)
            tip.canClimb = true;
    }
}
