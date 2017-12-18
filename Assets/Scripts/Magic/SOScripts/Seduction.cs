﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Seduction")]
public class Seduction : SpellPrimary {

    public Transform blushPrefab;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir)
    {
        base.ActivateSpell(user, secondaryEffect, fireDir);
    }

    public override void OnHit(Missile proj, Collider coll)
    {
        Damageable dam = coll.GetComponent<Damageable>();
        if (!proj.friendlyOff && dam.transform == proj.originator) { // if friendly fire is on and the collider is the owner
            return;
        }

        // the part where you seduce that PIMPLE-POPPING, NOSE-PICKING, NIPPLE-TWISTING, DICK OF AN ASS.

        if (dam) {
            dam.Seduce(duration, coll.gameObject, proj.originator.GetComponent<SpellCaster>()); // BECOME SEDUCED!
            proj.Die(); // don't need this anymore
            return;
        }

        if(proj.bounceCount <= 0) {
            proj.Die();
            return;
        } // if the projectile is out of bounces, die.

        proj.bounceCount--;
        // apologies, I thought it was kinda funny. Carry on.
    }

    public override void bounce(Missile proj)
    {
        base.bounce(proj);
    }
}
