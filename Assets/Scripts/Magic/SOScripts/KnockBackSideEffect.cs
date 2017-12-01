using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/KnockBack")]
public class KnockBackSideEffect : SpellSecondary {

    public float force;
    public float upwardForce;

    public override void MessUp(Transform user, Missile projectile)
    {
        Damageable dam = user.GetComponent<Damageable>();
        Vector3 dir = user.position - projectile.transform.position;
        dir = dir.normalized;
        dir.y = upwardForce;
        dir = dir.normalized;
        if (dam) { dam.knockBack(dir, force); }
    }

    public override void OnHit(Transform user, Missile projectile)
    {
        base.OnHit(user, projectile);
    }
}
