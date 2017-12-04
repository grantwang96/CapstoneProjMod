using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagic : MonoBehaviour, SpellCaster {

    List<SpellBook> spellsInventory = new List<SpellBook>();
    public int maxSpells;
    int currentHeld;

    public float spellPickUpSpeed;

    public Transform body;
    [SerializeField] Transform gun;

    bool canFire;

    #region UIStuff
    [SerializeField] Text SpellTitle;
    [SerializeField] Text SpellDescription;
    [SerializeField] Image ammoGaugeFill;
    [SerializeField] Image ammoGaugeBackground;
    [SerializeField] Image reticule;
    #endregion

    // Use this for initialization
    void Start () {
        currentHeld = 0;
        canFire = true;
        updateCurrentHeld();
	}
	
	// Update is called once per frame
	void Update () {
        processScrolling();
        if (Input.GetButtonDown("Fire1") && spellsInventory.Count != 0) {
            fireSpell();
        }
        if (spellsInventory.Count > 0 && ammoGaugeBackground.gameObject.activeInHierarchy) {
            ammoGaugeFill.fillAmount = (float)spellsInventory[currentHeld].getAmmo() / spellsInventory[currentHeld].getMaxAmmo();
        }
    }

    void processScrolling()
    {
        float mouse = Input.GetAxis("Mouse ScrollWheel");
        if(mouse > 0) { currentHeld++; }
        else if(mouse < 0) { currentHeld--; }
        updateCurrentHeld();
    }

    void updateCurrentHeld()
    {
        if(spellsInventory.Count == 0) {
            currentHeld = 0;
            ammoGaugeBackground.gameObject.SetActive(false);
            SpellTitle.text = "None Held";
            SpellDescription.text = "";
        }
        else {
            if (currentHeld >= spellsInventory.Count) { currentHeld = 0; }
            else if (currentHeld < 0) { currentHeld = spellsInventory.Count - 1; }
            ammoGaugeBackground.gameObject.SetActive(true);
            ammoGaugeFill.fillAmount = (float)spellsInventory[currentHeld].getAmmo() / spellsInventory[currentHeld].getMaxAmmo();
            ammoGaugeFill.color = spellsInventory[currentHeld].baseColor;
            SpellTitle.text = spellsInventory[currentHeld].spellTitle;
            SpellDescription.text = spellsInventory[currentHeld].spellDescription;
        }
    }

    void OnTriggerStay(Collider coll)
    {
        if (coll.tag.Contains("Book"))
        {
            SpellBook touchedSpell = coll.GetComponent<SpellBook>();
            if (Input.GetButtonDown("Fire2")) { touchedSpell.Interact(this); }
        }
    }

    #region SpellCaster Implementations

    public void addToSeductionList(Damageable loser)
    {
        
    }

    public void fireSpell()
    {
        if (!canFire) { return; } // If cooling down
        spellsInventory[currentHeld].primaryEffect.ActivateSpell(this, spellsInventory[currentHeld].secondaryEffect, transform.forward);
        spellsInventory[currentHeld].useAmmo();
        float coolDown = spellsInventory[currentHeld].primaryEffect.coolDown;
        coolDown += spellsInventory[currentHeld].secondaryEffect.coolDown;
        StartCoroutine(fireCoolDown(coolDown));
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
        updateCurrentHeld();
        StartCoroutine(pickUpProcess(newSpell));
    }

    IEnumerator pickUpProcess(SpellBook newSpell)
    {
        float startTime = Time.time;
        newSpell.transform.parent = transform;
        while(Time.time - startTime < spellPickUpSpeed)
        {
            newSpell.transform.position = Vector3.Lerp(newSpell.transform.position, transform.position, (Time.deltaTime / spellPickUpSpeed) * 2);
            yield return new WaitForEndOfFrame();
        }
        newSpell.transform.localPosition = Vector3.zero;
        newSpell.Deactivate();
    }

    public void dropSpell(SpellBook dropSpell, Vector3 originPos)
    {
        Debug.Log("Dropped Spell");
        if(dropSpell.owner == this.GetComponent<SpellCaster>()) { dropSpell.owner = null; }
        if (spellsInventory.Contains(dropSpell)) { spellsInventory.Remove(dropSpell); }
        updateCurrentHeld();
        StartCoroutine(dropSpellProcess(dropSpell, originPos));
    }

    IEnumerator dropSpellProcess(SpellBook dropSpell, Vector3 originPos)
    {
        // dropSpell.dead = true;
        dropSpell.Activate();
        dropSpell.transform.parent = null;
        Vector3 startPos = dropSpell.transform.position;
        while (!dropSpell.dead && Vector3.Distance(dropSpell.transform.position, originPos) > 0.2f)
        {
            dropSpell.transform.position = Vector3.Lerp(dropSpell.transform.position, originPos, Time.deltaTime / spellPickUpSpeed);
            yield return new WaitForEndOfFrame();
        }
        // Initiate drop sequence
    }

    public IEnumerator fireCoolDown(float duration)
    {
        canFire = false;
        float recovery = 0f;
        while(recovery < duration) {
            reticule.fillAmount = recovery / duration;
            recovery += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        reticule.fillAmount = 1f;
        canFire = true;
    }

    #endregion
}
