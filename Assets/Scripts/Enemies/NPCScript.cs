using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : Movement {

    public Collider FistColl;
    bool attacking;

    public override void Start()
    {
        setup();
    }

    public override void Update()
    {
        processMovement();
    }

    public override void setup()
    {
        base.setup();
        FistColl.GetComponent<FistScript>().damage = damage;
    }

    public override void processMovement()
    {
        base.processMovement();
    }

    public override void changeState(NPCState newState)
    {
        base.changeState(newState);
    }

    public override IEnumerator attack(Vector3 target)
    {
        if (attacking) { yield return null; }
        attacking = true;
        hamper++;
        FistColl.enabled = true;
        // play attack animation
        anim.Play("Attack");
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            yield return new WaitForEndOfFrame();
        }
        hamper--;
        FistColl.enabled = false;
        attacking = false;
        // get attack animation length
        // Do attack processing like hitbox, spell spawning, etc.
        // yield return new WaitForSeconds(1f); // set clip length here
    }
}
