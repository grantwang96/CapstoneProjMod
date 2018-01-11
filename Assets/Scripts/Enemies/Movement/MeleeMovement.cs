using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeMovement : Movement {
    
    public override void setup() {
        agent = GetComponent<NavMeshAgent>(); // set the agent
        base.setup();
    }
}
