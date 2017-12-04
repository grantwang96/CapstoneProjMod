using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/Transmute")]
public class Transmute : SpellPrimary {
    
    public GameObject[] possibleReplacements;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir)
    {
        base.ActivateSpell(user, secondaryEffect, fireDir);
    }

    public override void OnHit(Missile proj, Collider coll)
    {
        // if(coll.transform == proj.originator) { return; }
        /*
        if (proj.friendlyOff && coll.transform == proj.originator) {
            return;
        }
        */
        Damageable collDam = coll.GetComponent<Damageable>();
        if (collDam) {
            collDam.InitiateTransmutation(proj.duration, possibleReplacements[Random.Range(0, possibleReplacements.Length)]);
        }
        if(proj.bounceCount <= 0)
        {
            proj.Die();
            return;
        }
        bounce(proj);
        proj.bounceCount--;
    }

    public override void bounce(Missile proj)
    {
        ParticleSystem newBounce = Instantiate(proj.bounceEffect, proj.transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = newBounce.main;
        ParticleSystem.MinMaxGradient startCol = main.startColor;
        startCol.color = baseColor;
        Destroy(newBounce.gameObject, 1f);
    }
}
