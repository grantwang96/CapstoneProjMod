using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "EnemyData/SpellCasterEnemy")]
public class SpellCasterEnemy : EnemyData {

    public SpellPrimary[] possibleSpellPrimaries;
    public SpellSecondary[] possibleSpellSecondaries;

    public SpellBook spellBookPrefab;

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
        if(SpellManager.Instance != null) { // if spell manager is running
            SpellPrimary primary = possibleSpellPrimaries[Random.Range(0, possibleSpellPrimaries.Length)];
            SpellSecondary secondary = possibleSpellSecondaries[Random.Range(0, possibleSpellSecondaries.Length)];
            SpellBook newSpellBook = SpellManager.Instance.GenerateSpell(primary, secondary, owner.transform.position);

            SpellCaster spellCaster = owner.GetComponent<SpellCaster>();
            newSpellBook.Interact(spellCaster);
            newSpellBook.SetupSpell();
        }

        /*
        SpellBook newSpellBook = Instantiate(spellBookPrefab, owner.transform.position, owner.transform.rotation);
        newSpellBook.primaryEffect = possibleSpellPrimaries[Random.Range(0, possibleSpellPrimaries.Length)];
        newSpellBook.secondaryEffect = possibleSpellSecondaries[Random.Range(0, possibleSpellSecondaries.Length)];
        
        // Have enemy pick up spellbook
        SpellCaster spellCaster = owner.GetComponent<SpellCaster>();
        newSpellBook.Interact(spellCaster);
        */

        base.setup(owner);
    }
}
