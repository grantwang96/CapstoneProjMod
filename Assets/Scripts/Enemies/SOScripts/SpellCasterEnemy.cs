using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyData/SpellCasterEnemy")]
public class SpellCasterEnemy : EnemyData {

    public override void setup(Movement owner)
    {
        startingState = new NPCIdle();
        owner.baseSpeed = baseSpeed;
        owner.maxSpeed = maxSpeed;
        Damageable ownerDam = owner.GetComponent<Damageable>();
        ownerDam.max_health = health;
        owner.damage = damage;
        owner.attackTarget = GameObject.FindGameObjectWithTag("Player").transform;
        // Add spellbook to enemy's inventory
        base.setup(owner);
    }
}
