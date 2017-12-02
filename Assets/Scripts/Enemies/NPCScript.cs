using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : Movement {

    public Collider FistColl;
    bool attacking;
    bool falling = true;

    float yMove = Physics.gravity.y;
    Coroutine movementTakeOver;

    public CharacterController charCon;

    public override void Start()
    {
        setup();
        charCon = GetComponent<CharacterController>();
    }

    public override void Update()
    {
        processMovement();
    }

    void FixedUpdate()
    {
        if (falling) { yMove -= Time.deltaTime; }
        Move(Vector3.up * Physics.gravity.y * Time.deltaTime);
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

    public override void Move(Vector3 movement) {
        if (!charCon.enabled) { return; }
        charCon.Move(movement);
    }

    public override void knockBack(Vector3 dir, float force)
    {
        if (movementTakeOver != null) { StopCoroutine(movementTakeOver); }
        movementTakeOver = StartCoroutine(processKnockBack(dir, force));
    }

    IEnumerator processKnockBack(Vector3 dir, float force)
    {
        hamper++;
        yMove = dir.y;
        charCon.Move(dir * Time.deltaTime);
        falling = true;
        Vector3 flatForce = dir;
        flatForce.y = 0;
        while (!charCon.isGrounded)
        {
            charCon.Move(flatForce * Time.deltaTime);
            charCon.Move(Vector3.up * yMove * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        Vector3 start = flatForce;
        float prog = 0f;
        Debug.Log(flatForce);
        while (flatForce != Vector3.zero)
        {
            charCon.Move(flatForce * Time.deltaTime);
            prog += Time.deltaTime;
            flatForce = Vector3.Lerp(start, Vector3.zero, prog);
            yield return new WaitForEndOfFrame();
        }
        hamper--;
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

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float dist = Vector3.Distance(hit.point, transform.position + Vector3.up * charCon.height / 2);
        if(dist < 0.1f) {
            yMove = 0f;
        }
        if (charCon.isGrounded) { falling = false; }
    }
}
