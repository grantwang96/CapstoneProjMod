using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface SpellCaster
{
    void addToSeductionList(Damageable loser);
    void fireSpell();
    void pickUpSpell(SpellBook newSpell);
    void dropSpell(SpellBook dropSpell, Vector3 originPos);
    Transform returnGun();
    Transform returnBody();
    Transform returnHead();
}

public interface Interactable
{
    void Interact(SpellCaster spellCaster);
}