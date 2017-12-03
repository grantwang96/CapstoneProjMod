using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistScript : MonoBehaviour {

    public int damage;
    public float force;
    public Transform myBody;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider coll)
    {
        Damageable dam = coll.GetComponent<Damageable>();
        if (dam != null && dam != myBody.GetComponent<Damageable>())
        {
            Vector3 dir = (coll.transform.position - myBody.position).normalized;
            dam.TakeDamage(myBody, damage, dir, force);
            // Debug.Log("Hit");
        }
    }
}
