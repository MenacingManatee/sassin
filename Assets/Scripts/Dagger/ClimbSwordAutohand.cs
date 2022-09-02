using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ClimbSwordAutohand : Grabbable
{
    private Climbable climb;

    internal void SetClimbingHand() {
        climb = transform.gameObject.AddComponent<Climbable>();
    }

    internal void UnsetClimbingHand() {
        if (climb)
            Destroy(climb);
    }
}
