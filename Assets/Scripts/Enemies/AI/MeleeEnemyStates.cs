using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyIdle : NPCState
{
    public override void Enter(Movement owner, float newDuration) {

        myOwner = owner; // save the owner
        duration = newDuration; // set the duration
        stateStartTime = Time.time;

        anim = myOwner.anim;
        anim.SetInteger("Status", 0);

        // stop the agent from moving
        myOwner.agent.isStopped = true;

        Debug.Log("Entering Idle...");
    }

    public override void Execute() {

        if(myOwner.checkView()) { // look for the player
            Debug.Log("I see you!");
            myOwner.changeState(new MeleeEnemyAggro());
            return;
        }

        if(Time.time - stateStartTime >= duration) { // connect to wander
            myOwner.changeState(new MeleeEnemyWander());
        }
    }

    public override void Exit() {

    }

    public override void becomeAggro(EnemyData.CombatType combatType)
    {
        
    }
}

public class MeleeEnemyWander : NPCState
{
    Vector3 target;

    public override void Enter(Movement owner)
    {
        myOwner = owner;
        stateStartTime = Time.time;
        duration = Random.Range(4f, 6f);
        anim = myOwner.anim;
        anim.SetInteger("Status", 1);
        myOwner.agent.speed = myOwner.baseSpeed;

        // Set myowner agent's destination(ONLY HAPPENS ONCE)
        Vector3 target = myOwner.getRandomLocation(myOwner.transform.position, myOwner.sightRange);
        myOwner.agent.SetDestination(target);

        myOwner.agent.isStopped = false;
        if(target == null) { Debug.Log("No Target!"); }
        Debug.Log("Entering wander...");
    }

    public override void Execute()
    {
        float dist = Vector3.Distance(myOwner.transform.position, target);
        if(dist <= 1.5f) {
            myOwner.changeState(new MeleeEnemyIdle());
        }

        if(myOwner.checkView()) { myOwner.changeState(new MeleeEnemyAggro()); Debug.Log("I see you!"); }

        if(Time.time - stateStartTime >= duration) {
            myOwner.changeState(new MeleeEnemyIdle());
        }
    }

    public override void Exit() {

    }
}

public class MeleeEnemyAggro : NPCState
{
    Transform attackTarget;
    bool targetInView;
    float lostTargetViewTime = 0f;

    public override void Enter(Movement owner)
    {
        // Initialize information from movement object(owner)
        myOwner = owner;
        attackTarget = myOwner.attackTarget;
        targetInView = false;
        myOwner.agent.speed = myOwner.maxSpeed;

        // Grab and set animation state
        anim = myOwner.anim;
        anim.SetInteger("Status", 2);
        duration = myOwner.blueprint.attentionSpan;

        myOwner.agent.isStopped = false;
        Debug.Log("Entering Chase...");
    }

    public override void Execute()
    {
        // if you have nothing to chase, stop chasing
        if(myOwner.attackTarget == null) { myOwner.changeState(new MeleeEnemyIdle()); }

        // Check to see if the target is still in view
        targetInView = myOwner.checkView();

        // Enter idle if target has been out of view too long
        if(!targetInView) {
            lostTargetViewTime += Time.deltaTime;
            if(lostTargetViewTime >= duration) { myOwner.changeState(new MeleeEnemyIdle()); }
        }
        else {
            lostTargetViewTime = 0f;
        }

        // Enter attack state if in range to attack
        float dist = Vector3.Distance(myOwner.transform.position, attackTarget.position);
        if(dist <= myOwner.blueprint.attackRange) {
            myOwner.changeState(new MeleeEnemyAttack());
        }

        // set the movement object's navmesh agent destination to their attack target
        myOwner.agent.SetDestination(attackTarget.position);
    }

    public override void Exit()
    {
        
    }
}

public class MeleeEnemyAttack : NPCState
{
    Transform attackTarget;
    public override void Enter(Movement owner)
    {
        myOwner = owner;
        attackTarget = myOwner.attackTarget;

        anim = myOwner.anim;
        anim.Play("Attack");
        myOwner.attack(attackTarget.position);
        Debug.Log(myOwner.transform.name + " attacks!");
    }

    public override void Execute()
    {
        // check if attack animation is finished
        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            myOwner.changeState(new MeleeEnemyAggro());
        }
    }

    public override void Exit()
    {
        
    }
}