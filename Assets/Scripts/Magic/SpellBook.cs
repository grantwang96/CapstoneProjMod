using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellBook : MonoBehaviour, Interactable {

    public SpellPrimary primaryEffect;
    public SpellSecondary secondaryEffect;

    public int ammo;
    [SerializeField] bool _dead;
    public bool dead { get { return _dead; } set { _dead = value; } }

    MeshRenderer[] allMeshes;
    Rigidbody rbody;

	// Use this for initialization
	void Awake () {
        allMeshes = GetComponentsInChildren<MeshRenderer>();
        rbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!_dead) // If not dead
        {
            // do fun rotation
            // do floaty effect
            // magic sparks!s
        }
	}

    public void Interact(SpellCaster spellCaster)
    {
        if (_dead) { return; }
        spellCaster.pickUpSpell(this);
        _dead = true;
    }

    public void Deactivate()
    {
        foreach(MeshRenderer mr in allMeshes) {
            mr.enabled = false;
        }
        GetComponent<SphereCollider>().enabled = false;
    }

    public void Activate()
    {
        foreach (MeshRenderer mr in allMeshes) {
            mr.enabled = true;
        }
        GetComponent<SphereCollider>().enabled = true;
    }

    public void Die()
    {
        // Do Die Effect
    }
}
