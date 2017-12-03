using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/KnockBack")]
public class KnockBackSideEffect : SpellSecondary {

    public float force;
    public float upwardForce;
    public float waitTime;

    public float valueModifier;

    public override void MessUp(Transform user, Missile projectile)
    {
        Damageable dam = user.GetComponent<Damageable>();
        Vector3 dir = user.position - projectile.transform.position;
        dir = dir.normalized;
        dir.y = upwardForce;
        dir = dir.normalized;
        projectile.transform.localScale = new Vector3(projectile.transform.localScale.x * valueModifier, projectile.transform.localScale.y, projectile.transform.localScale.z);
        projectile.trail.widthMultiplier *= valueModifier;
        projectile.power *= Mathf.RoundToInt(valueModifier);
        projectile.duration *= valueModifier;
        projectile.friendlyOff = true;
        if (dam) { dam.knockBack(dir, force); }
    }

    IEnumerator chargeAndFire() {
        yield return new WaitForSeconds(waitTime);
    }

    public override void OnHit(Transform user, Missile projectile)
    {
        base.OnHit(user, projectile);
    }
}
