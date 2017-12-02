using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Magic Blast")]
public class MagicBlast : SpellPrimary { // Standard damaging magic attack

    public float knockBackForce;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect) {
        base.ActivateSpell(user, secondaryEffect);
    }

    public override void OnHit(Missile proj, Collider coll) {
        if(!proj.friendlyOff || coll.transform != proj.originator) {
            if (proj.bounceCount <= 0)
            {
                Damageable collDam = coll.GetComponent<Damageable>();
                if (collDam) {
                    collDam.TakeDamage(proj.originator, power, proj.transform.forward, knockBackForce);
                }
                // Instantiate special effect
                proj.Die();
                return;
            }
        }
        bounce(proj);
        proj.bounceCount--;
    }

    public override void bounce(Missile proj)
    {
        base.bounce(proj);
    }
}
