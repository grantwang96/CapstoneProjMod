
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagic : MonoBehaviour, SpellCaster {

    List<SpellBook> spellsInventory = new List<SpellBook>();
    public int maxSpells;
    int currentHeld;

    public float spellPickUpSpeed;

    public Transform body;
    [SerializeField] Transform gun;

    // Use this for initialization
    void Start () {
        currentHeld = 0;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("Fire1") && spellsInventory.Count != 0) {
            fireSpell();
        }
    }

    void processScrolling()
    {
        float mouse = Input.GetAxis("Mouse ScrollWheel");
        if(mouse > 0) { currentHeld++; if(currentHeld >= spellsInventory.Count) { currentHeld = 0; } }
        else if(mouse < 0) { currentHeld--; if(currentHeld < 0) { currentHeld = spellsInventory.Count - 1; } }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.tag.Contains("Book"))
        {
            SpellBook touchedSpell = coll.GetComponent<SpellBook>();
            if (Input.GetButtonDown("Fire2")) { touchedSpell.Interact(this); }
        }
    }

    public void addToSeductionList(Damageable loser)
    {
        
    }

    public void fireSpell()
    {
        spellsInventory[currentHeld].primaryEffect.ActivateSpell(this, spellsInventory[currentHeld].secondaryEffect);
    }

    public Transform returnGun() { return gun; }

    public Transform returnBody() { return body; }

    public Transform returnHead() { return transform; }

    public void pickUpSpell(SpellBook newSpell)
    {
        Debug.Log("Grabbed Spell");
        if(spellsInventory.Count == maxSpells) {
            SpellBook lostSpell = spellsInventory[currentHeld];
            spellsInventory[currentHeld] = newSpell;
            dropSpell(lostSpell, newSpell.transform.position);
        }
        else {
            spellsInventory.Add(newSpell);
            currentHeld = spellsInventory.Count - 1;
        }
        StartCoroutine(pickUpProcess(newSpell));
    }

    IEnumerator pickUpProcess(SpellBook newSpell)
    {
        float startTime = Time.time;
        while(Time.time - startTime < spellPickUpSpeed)
        {
            newSpell.transform.position = Vector3.Lerp(newSpell.transform.position, transform.position, (Time.deltaTime / spellPickUpSpeed) * 2);
            yield return new WaitForEndOfFrame();
        }
        newSpell.transform.localPosition = Vector3.zero;
        newSpell.transform.parent = transform;
        newSpell.Deactivate();
    }

    public void dropSpell(SpellBook dropSpell, Vector3 originPos)
    {
        Debug.Log("Dropped Spell");
        dropSpell.Activate();
        StartCoroutine(dropSpellProcess(dropSpell, originPos));
    }

    IEnumerator dropSpellProcess(SpellBook dropSpell, Vector3 originPos)
    {
        dropSpell.dead = true;
        Vector3 startPos = dropSpell.transform.position;
        while (Vector3.Distance(dropSpell.transform.position, originPos) > 0.2f)
        {
            dropSpell.transform.position = Vector3.Lerp(dropSpell.transform.position, originPos, Time.deltaTime * spellPickUpSpeed);
            yield return new WaitForEndOfFrame();
        }
        // Initiate drop sequence
    }
}
