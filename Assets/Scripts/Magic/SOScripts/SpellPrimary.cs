using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellPrimary : ScriptableObject {

    public float force; // for travel
    public int power; // For damage or healing

    public float powerLevelModifier;
    [Range(1, 10)] public int powerLevel;

    public Color baseColor;

    public Missile projectilePrefab; // projectile used

    public virtual void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect) // When the spell is fired
    {
        Transform firingPoint = user.returnGun();
        if (firingPoint)
        {
            // Create a new missile object
            Missile newProjectile = Instantiate(projectilePrefab, firingPoint.position, user.returnHead().rotation);
            newProjectile.bounceCount = 0;
            newProjectile.primaryEffect = this;
            newProjectile.secondaryEffect = secondaryEffect;
            newProjectile.originator = user.returnBody();

            // Modify rigidbody settings for takeoff
            Rigidbody projRbody = newProjectile.GetComponent<Rigidbody>();
            projRbody.useGravity = false;
            projRbody.AddForce(newProjectile.transform.forward * force, ForceMode.Impulse);

            // Apply visual effects
            ParticleSystem.MainModule main = newProjectile.sparkles.main;
            main.startColor = baseColor;
            TrailRenderer trail = newProjectile.trail;
            trail.startColor = baseColor;
            trail.endColor = baseColor;

            // Apply secondary effects
            secondaryEffect.MessUp(user.returnBody(), newProjectile);
        }
    }

    public virtual void OnHit(Missile proj, Collider coll) // When the spell hits something
    {
        
    }

    public virtual void bounce(Missile proj)
    {
        ParticleSystem newBounce = Instantiate(proj.bounceEffect, proj.transform.position, Quaternion.identity);
        ParticleSystem.MainModule main = newBounce.main;
        ParticleSystem.MinMaxGradient startCol = main.startColor;
        startCol.color = baseColor;
        Destroy(newBounce.gameObject, 1f);
    }
}
