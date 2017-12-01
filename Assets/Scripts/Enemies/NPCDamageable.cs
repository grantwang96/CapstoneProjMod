using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCDamageable : Damageable {

    Coroutine seduction;
    Coroutine transmutation;

    public MeshRenderer[] renderers; // All renderers on this gameobject

    public override void Seduce(float duration, GameObject target, SpellCaster owner)
    {
        base.Seduce(duration, target, owner);
        if(seduction != null) {
            StopCoroutine(seduction);
            myMovement.hamper--;
        }
        seduction = StartCoroutine(processSeduction(duration, target, owner));
    }

    IEnumerator processSeduction(float duration, GameObject target, SpellCaster owner)
    {
        myMovement.hamper++;
        float startTime = Time.time;
        NPCState previousState = myMovement.getCurrentState();
        becomeSeduced(myMovement.myType);
        while(Time.time - startTime < duration)
        {
            yield return new WaitForEndOfFrame();
        }
        myMovement.hamper--;
        seduction = null;
    }

    public override void InitiateTransmutation(float duration, GameObject replacement)
    {
        if (transmutable) {
            StartCoroutine(processTransmutation(duration, replacement));
        }
    }

    public override IEnumerator processTransmutation(float duration, GameObject replacement)
    {
        // Turn off transmutable
        transmutable = false;

        // Remove Collider
        Collider myColl = GetComponent<Collider>();
        myColl.enabled = false;

        // Freeze Rigidbody
        rbody.constraints = RigidbodyConstraints.FreezeAll;

        foreach(MeshRenderer rend in renderers) // Turn off all mesh renderers
        {
            rend.enabled = false;
        }

        GameObject newBody = Instantiate(replacement, transform.position, transform.rotation); // Create a new replacement here
        Damageable newDam = newBody.GetComponent<Damageable>();
        newDam.setTransmutable(false); // Do not allow transmutations on this object either
        newDam.parentHit = this; // redirect damage to enemy
        // Attach some sparkly effect to indicate it is transmuted

        yield return new WaitForSeconds(duration); // Wait for this timeframe

        transform.position = newBody.transform.position; // move original body to newbody's position
        Destroy(newBody); // Get rid of old body

        foreach(MeshRenderer rend in renderers) { rend.enabled = true; } // re-enable mesh renderers
        rbody.constraints = RigidbodyConstraints.FreezeRotation; // re-enable movement
        myColl.enabled = true; // re-enable collisions

        setTransmutable(true);
    } // Transmutation Co-routine

    void becomeSeduced(EnemyData.CombatType combatType)
    {
        switch (combatType)
        {
            case EnemyData.CombatType.Melee:
                myMovement.changeState(new MeleeEnemySeduced());
                break;
            case EnemyData.CombatType.SpellCaster:
                break;
            case EnemyData.CombatType.Mixed:
                break;
            case EnemyData.CombatType.Ranged:
                break;
            case EnemyData.CombatType.Support:
                break;
        }
    }

    public override void Die()
    {
        ScoreKeeper.Instance.incrementScore();
        base.Die();
    }
}
