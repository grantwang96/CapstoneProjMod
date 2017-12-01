using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "Secondary Spell Effect/Surround EFfect")]
public class SurroundSideEffect : SpellSecondary {

    public float forwardTime;
    public float speed;

    public override void MessUp(Transform user, Missile projectile)
    {
        base.MessUp(user, projectile);
        projectile.messUpEffect = projectile.StartCoroutine(processSurround(user, projectile));
    }

    public override void OnHit(Transform user, Missile projectile)
    {
        base.OnHit(user, projectile);
    }

    IEnumerator processSurround(Transform user, Missile projectile)
    {
        projectile.bounceCount = 0;
        yield return new WaitForSeconds(forwardTime);
        Rigidbody projRbody = projectile.GetComponent<Rigidbody>();
        projRbody.velocity = Vector3.zero;
        while (true)
        {
            if(projectile == null) { break; }
            Debug.Log("woo!");
            // fly around the user
            projectile.transform.RotateAround(user.position, user.up, speed * Time.deltaTime);
            Debug.Log(projectile.transform.eulerAngles);
            yield return new WaitForEndOfFrame();
        }
    }
}
