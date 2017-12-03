﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Magic Blast")]
public class MagicBlast : SpellPrimary { // Standard damaging magic attack

    public float knockBackForce;
    public float upwardKnockup;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir) {
        base.ActivateSpell(user, secondaryEffect, fireDir);
    }

    public override void OnHit(Missile proj, Collider coll) {
        if(proj.friendlyOff && coll.transform == proj.originator) {
            Debug.Log("Friendly Hit!");
            return;
        }
        Damageable collDam = coll.GetComponent<Damageable>();
        if (collDam) {
            Debug.Log(coll.transform.name + " was hit!");
            Vector3 knockBack = proj.transform.forward;
            knockBack.y = upwardKnockup;
            knockBack = knockBack.normalized;
            collDam.TakeDamage(proj.originator, proj.power, knockBack, knockBackForce);
            // Instantiate special effect
            proj.Die();
            return;
        }
        if (proj.bounceCount <= 0) {
            proj.Die();
        }
        else {
            bounce(proj);
            proj.bounceCount--;
        }
    }

    public override void bounce(Missile proj)
    {
        base.bounce(proj);
    }
}
