using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WizardEnemyIdle : NPCState{

    public override void Enter(Movement owner, float newDuration)
    {
        base.Enter(owner, newDuration);
    }

    public override void Execute()
    {
        if (myOwner.checkView()) { // look for the player
            Debug.Log("I see you!");
            myOwner.changeState(new WizardEnemyAttack());
            return;
        }

        if (Time.time - stateStartTime >= duration) { // connect to wander
            myOwner.changeState(new WizardEnemyWander());
        }
    }
}

public class WizardEnemyWander : NPCState
{
    public override void Enter(Movement owner)
    {
        myOwner = owner;
        stateStartTime = Time.time;
        duration = Random.Range(4f, 6f);
        anim = myOwner.anim;
        anim.SetInteger("Status", 1);
        myOwner.agent.speed = myOwner.baseSpeed;

        // Set myowner agent's destination(ONLY HAPPENS ONCE)
        Vector3 target = myOwner.getRandomLocation(myOwner.transform.position, myOwner.maxWanderDistance);
        myOwner.agent.SetDestination(target);

        if (target == null) { Debug.Log("No Target!"); }
        Debug.Log("Entering wander...");
        Debug.Log("Target " + myOwner.agent.destination);
    }

    public override void Execute()
    {
        if (myOwner.checkView()) { myOwner.changeState(new WizardEnemyAttack()); }

        float distToDest = Vector3.Distance(myOwner.transform.position, myOwner.agent.pathEndPosition);
        if (distToDest < 0.2f + myOwner.agent.stoppingDistance)
        {
            Debug.Log("Reached destination!");
            myOwner.changeState(new WizardEnemyIdle(), Random.Range(4f, 6f));
        }
        /*
        if(myOwner.hamper <= 0) {
            myOwner.rbody.MovePosition(myOwner.transform.position + myOwner.agent.desiredVelocity * Time.deltaTime);
        }
        */
        if (myOwner.friction != 1f) { myOwner.rbody.AddForce(myOwner.agent.desiredVelocity * (1f - myOwner.friction)); }
    }

    public override void Exit()
    {
        
    }
}

public class WizardEnemyAggro : NPCState
{
    bool targetInView;
    bool targetWasInView;
    float lostTargetViewTime = 0f;

    public override void Enter(Movement owner)
    {
        myOwner = owner;

        // always look at the target
        myOwner.agent.updateRotation = false;

        targetInView = false;
        myOwner.agent.speed = myOwner.maxSpeed;
        anim = myOwner.anim;
        anim.SetInteger("Status", 2);
        duration = myOwner.blueprint.attentionSpan;

        Debug.Log("Entering aggro...");
        FindCover();
    }

    public override void Execute()
    {
        Vector3 targetDir = myOwner.attackTarget.position - myOwner.transform.position;
        targetDir.y = 0;
        Quaternion forward = Quaternion.LookRotation(targetDir);
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, forward, 0.8f);

        // if you have nothing to chase, stop chasing
        if (myOwner.attackTarget == null) { myOwner.changeState(new WizardEnemyIdle(), Random.Range(4f, 6f)); }

        // Check to see if the target is still in view
        targetInView = myOwner.checkView();

        // Enter idle if target has been out of view too long
        if (!targetInView) {
            lostTargetViewTime += Time.deltaTime;
            // myOwner.agent.SetDestination(myOwner.attackTarget.position);
            if (lostTargetViewTime >= duration) {
                Debug.Log("Where'd you go?");
                myOwner.changeState(new WizardEnemyIdle(), Random.Range(4f, 6f));
            }
        }
        else { lostTargetViewTime = 0f; }

        // if(targetInView && !targetWasInView) { FindCover(); }

        if(myOwner.agent.desiredVelocity.magnitude < 0.5f) { myOwner.changeState(new WizardEnemyAttack()); }
        targetWasInView = targetInView;
    }

    public override void Exit()
    {
        myOwner.agent.updateRotation = true;
    }

    void FindCover()
    {
        List<NavMeshHit> potentialPositions = new List<NavMeshHit>();
        for (int i = 0; i < 10; i++) {
            Vector3 randPos = myOwner.getRandomLocation(myOwner.attackTarget.position, myOwner.maxWanderDistance * 2);
            NavMeshHit hit;
            if (NavMesh.FindClosestEdge(randPos, out hit, myOwner.agent.areaMask)) {
                Vector3 dir = myOwner.attackTarget.position - hit.position;
                dir.y = 0;

                float angle = Vector3.Angle(hit.normal, dir.normalized);
                if (angle >= 90f) {
                    potentialPositions.Add(hit);
                    Debug.Log("Position: " + hit.position + ". Angle: " + angle);
                }
            }
        }

        if(potentialPositions.Count > 0) {
            NavMeshHit[] arr = SplitNMerge(potentialPositions.ToArray());
            myOwner.agent.SetDestination(arr[0].position);
        }
    }

    NavMeshHit[] SplitNMerge(NavMeshHit[] array)
    {
        if (array.Length < 2) { return array; } // if the resulting array is length 1
        int middle = array.Length / 2; // split the array down the middle

        // recursively handle each array

        NavMeshHit[] leftArray = new NavMeshHit[middle];
        CopyArray(array, leftArray, 0, middle);
        leftArray = SplitNMerge(leftArray);

        NavMeshHit[] rightArray = new NavMeshHit[array.Length - middle];
        CopyArray(array, rightArray, middle, array.Length);
        rightArray = SplitNMerge(rightArray);

        // merge back the left and right array
        NavMeshHit[] newArray = MergeBack(leftArray, rightArray); 
        return newArray;
    }

    NavMeshHit[] MergeBack(NavMeshHit[] a1, NavMeshHit[] a2)
    {
        int a = 0, b = 0;
        int length = a1.Length + a2.Length;
        NavMeshHit[] finishedArray = new NavMeshHit[length];
        for (int i = 0; i < length; i++) {
            float distA = -1;
            float distB = -1;

            // only compare a1 and a2 if a and be are respectively in range
            if (a < a1.Length) { distA = Vector3.Distance(a1[a].position, myOwner.attackTarget.position); }
            if (b < a2.Length) { distB = Vector3.Distance(a2[b].position, myOwner.attackTarget.position); }

            // if one of them is out of range
            if (distB == -1) { finishedArray[i] = a1[a]; a++; }
            else if (distA == -1) { finishedArray[i] = a2[b]; b++; }

            // otherwise compare
            else {
                if (distA <= distB) { finishedArray[i] = a1[a]; a++; }
                else { finishedArray[i] = a2[b]; b++; }
            }
        }
        return finishedArray;
    }

    void CopyArray(NavMeshHit[] A, NavMeshHit[] B, int start, int end)
    {
        int idx = start;
        for (int i = 0; i < B.Length; i++) {
            B[i] = A[idx];
            idx++;
        }
    }
}

public class WizardEnemyAttack : NPCState
{
    public override void Enter(Movement owner) {
        myOwner = owner;
        anim = myOwner.anim;

        myOwner.agent.updateRotation = false;
        Debug.Log("Entering Attack...");
        myOwner.StartCoroutine(attackProcessing());
    }

    public override void Execute()
    {
        Vector3 targetDir = myOwner.attackTarget.position - myOwner.transform.position;
        targetDir.y = 0;
        Quaternion forward = Quaternion.LookRotation(targetDir);
        myOwner.transform.rotation = Quaternion.Lerp(myOwner.transform.rotation, forward, 5f * Time.deltaTime);
    }

    IEnumerator attackProcessing()
    {
        int fireCount = Random.Range(2, 5);
        SpellBook spellbook = myOwner.GetComponent<SpellCaster>().returnSpell();
        float cooldown = spellbook.primaryEffect.coolDown + spellbook.secondaryEffect.coolDown;
        for (int i = 0; i < fireCount; i++)
        {
            myOwner.StartCoroutine(myOwner.attack(myOwner.attackTarget.position));
            yield return new WaitForEndOfFrame();
            while(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("Fired spell");
            yield return new WaitForSeconds(cooldown);
        }
        myOwner.changeState(new WizardEnemyAggro());
    }

    public override void Exit()
    {
        myOwner.agent.updateRotation = true;
    }
}

public class WizardEnemySeduced : NPCState
{

}
