using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Primary Spell Effect/FireBlast")]
public class FireBlast : SpellPrimary {

    public ParticleSystem tinyFlamePrefab;
    public Transform firePillarPrefab;

    public int shrapnelCountLowerBound;
    public int shrapnelCountUpperBound;
    public int subBlastPowerMod;
    public float mass;
    public float radius;

    public override void ActivateSpell(SpellCaster user, SpellSecondary secondaryEffect, Vector3 fireDir)
    {
        base.ActivateSpell(user, secondaryEffect, fireDir);
    }

    public override void OnHit(Missile proj, Collider coll)
    {
        if(proj.bounceCount <= 0) {
            proj.StartCoroutine(firePillar(coll.transform, proj));
            // proj.Die();
        }
        else {
            bounce(proj);
            proj.bounceCount--;
        }
    }

    IEnumerator firePillar(Transform coll, Missile proj)
    {
        proj.Deactivate();
        Rigidbody rbody = proj.GetComponent<Rigidbody>();
        rbody.useGravity = false;
        rbody.isKinematic = true;
        proj.GetComponent<Collider>().enabled = false;
        ParticleSystem flame = Instantiate(tinyFlamePrefab, coll.position, Quaternion.identity);
        ParticleSystem.MainModule flameMain = flame.main;
        while(flameMain.startLifetime.constant > 0f) {
            ParticleSystem.MinMaxCurve lifeTime = flameMain.startLifetime;
            lifeTime.constant -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        // Transform newPillar = Instantiate(firePillarPrefab, flame.transform.position, Quaternion.identity);
        mainBlast(proj);
        Destroy(flame.gameObject);
    }

    void mainBlast(Missile projFired)
    {
        Transform newPillarOfDoom = Instantiate(firePillarPrefab, projFired.transform.position, Quaternion.identity);
        newPillarOfDoom.GetComponent<PillarOfDoom>().damage = projFired.power;
        newPillarOfDoom.GetComponent<PillarOfDoom>().myCaster = projFired.originator;
        int shrapCount = UnityEngine.Random.Range(shrapnelCountLowerBound, shrapnelCountUpperBound);
        float angInterval = 360 / shrapCount;
        for (int i = 0; i < shrapCount; i++)
        {
            float ang = angInterval * i;
            Vector3 offset;
            offset.x = projFired.transform.position.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            offset.y = projFired.transform.position.y + 1f;
            offset.z = projFired.transform.position.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
            subBlast(offset, newPillarOfDoom.position, projFired.originator);
        }
    }

    void subBlast(Vector3 position, Vector3 startingPos, Transform caster)
    {
        Missile newSubBlast = Instantiate(projectilePrefab, position, Quaternion.identity);
        newSubBlast.transform.forward = position - startingPos;
        Missile newproj = newSubBlast.GetComponent<Missile>();
        newproj.originator = caster;
        newproj.primaryEffect = this;
        newproj.power = power;
        Rigidbody rbody = newSubBlast.GetComponent<Rigidbody>();
        rbody.useGravity = true;
        rbody.mass = mass;
        rbody.AddForce(newSubBlast.transform.forward * UnityEngine.Random.Range(6, 12), ForceMode.Impulse);
        ParticleSystem.MainModule main = newproj.GetComponent<ParticleSystem>().main;
        main.startColor = baseColor;
        newproj.mainShot = false;
    }

}
