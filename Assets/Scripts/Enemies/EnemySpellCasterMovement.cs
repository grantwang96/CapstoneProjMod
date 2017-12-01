using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellCasterMovement : Movement, SpellCaster
{
    #region Public Variables
    SpellBook heldSpell;

    [SerializeField] Transform gun;
    [SerializeField] Transform body;
    #endregion

    #region Movement Implementations

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override IEnumerator attack(Vector3 target)
    {
        hamper++;
        anim.Play("Attack");
        bool fired = false;
        while (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            if(anim.GetCurrentAnimatorStateInfo(0).length >= 0.5f && !fired) {
                // Debug.Log("Pew");
                fired = true;
                // heldSpell.primaryCast();
            }
            yield return new WaitForEndOfFrame();
        }
        hamper--;
    }

    #endregion

    #region SpellCaster Implementations
    public void pickUpSpell(SpellBook newSpell)
    {
        
    }

    public void addToSeductionList(Damageable loser)
    {
        
    }

    public void dropSpell(SpellBook dropSpell, Vector3 originPos)
    {
        if(heldSpell == dropSpell)
        {
            heldSpell.transform.parent = null;
            heldSpell = null;
        }
    }

    public Transform returnGun() { return gun; }

    public Transform returnBody() { return body; }

    public Transform returnHead() { return transform; }

    public void getHitList(List<Damageable> hitList, SpellCaster owner)
    {
        
    }

    public void initiateSeduction(float duration)
    {
        
    }

    public void removeFromSeductionList(Damageable loser)
    {
        
    }

    public void fireSpell()
    {
        
    }
    
    #endregion
}
