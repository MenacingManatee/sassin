using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class ClimbSwordAutohand : Grabbable
{
    private Climbable climb;
    public AutoHandPlayer player;

    internal void SetClimbingHand() {
        climb = transform.gameObject.AddComponent<Climbable>();
        player.ManualCallClimb(lastHeldBy, this);
    }

    internal void UnsetClimbingHand() {
        player.ManualEndClimb(lastHeldBy, this);
        if (climb)
            Destroy(climb);
    }
}
