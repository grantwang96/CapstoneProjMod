﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementV2 : Movement {

    public int drunkMod;
    public float slownessSeverity;

    public float jumpForce;
    Vector3 moveDir;

    [SerializeField] bool falling;
    [SerializeField] float yMove;

    public CharacterController charCon;
    Coroutine movementTakeover;

	// Use this for initialization
	public override void Start () {
        // setup();
        charCon = GetComponent<CharacterController>();
        charCon.detectCollisions = true;
        yMove = -3f;
        falling = true;
	}

    // Update is called once per frame
    public override void Update () {
        processMovement();
	}

    void FixedUpdate()
    {
        if (falling) { yMove += Time.deltaTime * Physics.gravity.y; }
        // Debug.Log(charCon.isGrounded);
        Vector3 move = moveDir * Time.deltaTime * slownessSeverity * drunkMod; // Get the total movement
        charCon.Move(move);
        charCon.Move(Vector3.up * yMove * Time.deltaTime);
    }

    void calculateMove()
    {
        /*
            Vector3 dirNorm = move.normalized; // get direction of movement
            float distCheck = dirNorm.magnitude * Time.deltaTime; // get total distance moved
            RaycastHit[] hits = rbody.SweepTestAll(dirNorm, distCheck, QueryTriggerInteraction.Ignore);
            bool okayToMove = true;
            if(hits.Length != 0) { // check if collision will occur
                foreach(RaycastHit hit in hits) {
                    Debug.Log(hit.collider.name);
                    if(!hit.collider.tag.Contains("Furniture") || !hit.collider.tag.Contains("Ground")) {
                        okayToMove = false;
                    }
                }
            }
            if (okayToMove) { rbody.MovePosition(rbody.position + move); } // if it's okay to move, then move
            */
    }

    public override void setup()
    {
        // Do NOT give this a state machine!
        // Do NOT run base.setup() on this!
        rbody = GetComponent<Rigidbody>();
        currSpeed = maxSpeed;
    }

    public override void processMovement()
    {
        if (hamper > 0) { return; }
        float horizontal = Input.GetAxis("Horizontal"); // Get player inputs
        float vertical = Input.GetAxis("Vertical"); // Get player inputs

        if (Input.GetKeyDown(KeyCode.Space)) { Jump(); }
        moveDir = ((transform.forward * vertical * currSpeed) + (transform.right * horizontal * currSpeed));
    }

    public override void knockBack(Vector3 dir, float force)
    {
        if(movementTakeover != null) { StopCoroutine(movementTakeover); hamper--; }
        Vector3 knock = dir * force;
        movementTakeover = StartCoroutine(knockingBack(knock));
    }

    IEnumerator knockingBack(Vector3 force)
    {
        hamper++;
        yMove = force.y;
        charCon.Move(force * Time.deltaTime);
        falling = true;
        Vector3 flatForce = force;
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
        movementTakeover = null;
    }

    void Jump()
    {
        if (!charCon.isGrounded) { return; }
        yMove = jumpForce;
        falling = true;
    }

    void OnControllerColliderHit(ControllerColliderHit coll)
    {
        string tag = coll.collider.tag;
        if (tag.Contains("Ground"))
        {
            if (Vector3.Distance(coll.point, Head.position) < 0.1f) // If collided with head
            {
                yMove = 0f;
                return;
            }
            Vector3 feet = transform.position + Vector3.down * charCon.bounds.extents.y;
            if (Vector3.Distance(coll.point, feet) < 0.2f && !charCon.isGrounded) // If collided with feet
            {
                falling = false;
                yMove = Physics.gravity.y;
                return;
            }
        }
        if(tag.Contains("Wall") || tag.Contains("Furniture"))
        {
            if(movementTakeover != null) {
                Debug.Log("Hit a wall!");
                StopCoroutine(movementTakeover);
                movementTakeover = null;
                hamper--;
            }
            if(coll.collider.attachedRigidbody != null) {
                Vector3 velocity = coll.collider.transform.position - transform.position;
                coll.collider.attachedRigidbody.AddForce(velocity.normalized * currSpeed);
            }
        }
    }
}
