using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour {

    public List<SpellPrimary> primarySpellEffects = new List<SpellPrimary>();
    public List<SpellSecondary> secondarySpellEffects = new List<SpellSecondary>();

    public static SpellManager Instance;
    public SpellBook spellBookPrefab;

	// Use this for initialization
	void Awake () {
        Instance = this;
	}

    public void GenerateSpell()
    {

    }

    public void GenerateSpell(SpellPrimary primary, SpellSecondary secondary)
    {

    }
	
    public void GenerateSpell(Vector3 position)
    {

    }

    public void GenerateSpell(SpellPrimary primary, SpellSecondary secondary, Vector3 position)
    {

    }
}
