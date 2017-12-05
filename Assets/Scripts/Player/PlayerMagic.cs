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
    public LayerMask interactLayers;
    public float grabRange;
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
        processScrolling(); // if the player scrolls
        if (Input.GetButtonDown("Fire1") && spellsInventory.Count != 0) { // make sure player hits shoot button and has something to shoot
            fireSpell();
        }
        if (spellsInventory.Count > 0 && ammoGaugeBackground.gameObject.activeInHierarchy) { // update the ammo gauge
            ammoGaugeFill.fillAmount = (float)spellsInventory[currentHeld].getAmmo() / spellsInventory[currentHeld].getMaxAmmo();
        }
        processLooking();
    }

    void processLooking()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        // Debug.DrawLine(transform.position, transform.position + transform.forward * grabRange, Color.green, 1f);
        if (Physics.Raycast(ray, out hit, grabRange, interactLayers, QueryTriggerInteraction.Collide)) { // if you hit something that is interactable
            if (Input.GetButtonDown("Fire2")) { // if the player tries to interact with something
                Interactable interactable = hit.collider.GetComponent<Interactable>(); // be REALLY sure we can interact with it
                if (interactable != null) { interactable.Interact(this); }
            }
        }
    }

    void processScrolling()
    {
        float mouse = Input.GetAxis("Mouse ScrollWheel"); // record input from mouse scrollwheel
        if(mouse > 0) { currentHeld++; }
        else if(mouse < 0) { currentHeld--; }
        updateCurrentHeld();
    }

    void updateCurrentHeld() // make sure currentheld is within inventory count
    {
        if(spellsInventory.Count == 0) { // shut off the ammo gauge
            currentHeld = 0;
            ammoGaugeBackground.gameObject.SetActive(false);
            SpellTitle.text = "None Held";
            SpellDescription.text = "";
        }
        else { // update the ammo gauge and makesure current held is within inventory count
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

    public void fireSpell() // Shoot the spell
    {
        if (!canFire) { return; } // If cooling down
        spellsInventory[currentHeld].primaryEffect.ActivateSpell(this, spellsInventory[currentHeld].secondaryEffect, transform.forward); // activate currently held spellbook
        spellsInventory[currentHeld].useAmmo(); // the player uses ammo in a spellbook

        // Calculate and initiate cooldown
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
        if(spellsInventory.Count == maxSpells) { // if the player's inventory is full
            SpellBook lostSpell = spellsInventory[currentHeld];
            spellsInventory[currentHeld] = newSpell;
            dropSpell(lostSpell, newSpell.transform.position);
        }
        else { // otherwise just add the spell
            spellsInventory.Add(newSpell);
            currentHeld = spellsInventory.Count - 1;
        }
        updateCurrentHeld();
        StartCoroutine(pickUpProcess(newSpell)); // visualize pick up
    }

    IEnumerator pickUpProcess(SpellBook newSpell) // pick up the spellbook
    {
        float startTime = Time.time;
        newSpell.transform.parent = transform;
        while(Time.time - startTime < spellPickUpSpeed) // pull spellbook closer
        {
            newSpell.transform.position = Vector3.Lerp(newSpell.transform.position, transform.position, (Time.deltaTime / spellPickUpSpeed) * 2);
            yield return new WaitForEndOfFrame();
        }

        // place spellbook at the middle and stop rendering book)
        newSpell.transform.localPosition = Vector3.zero;
        newSpell.Deactivate();
    }

    public void dropSpell(SpellBook dropSpell, Vector3 originPos) // drop the spellbook
    {
        Debug.Log("Dropped Spell");
        // unlink spellbook from player
        if(dropSpell.owner == this.GetComponent<SpellCaster>()) { dropSpell.owner = null; }
        if (spellsInventory.Contains(dropSpell)) { spellsInventory.Remove(dropSpell); }

        // update current held
        updateCurrentHeld();
        // visualize dropping book
        StartCoroutine(dropSpellProcess(dropSpell, originPos));
    }

    IEnumerator dropSpellProcess(SpellBook dropSpell, Vector3 originPos) // visualize dropping the book
    {
        // dropSpell.dead = true;
        dropSpell.Activate(); // turn on the book
        dropSpell.transform.parent = null;
        Vector3 startPos = dropSpell.transform.position;
        while (!dropSpell.dead && Vector3.Distance(dropSpell.transform.position, originPos) > 0.2f)
        {
            dropSpell.transform.position = Vector3.Lerp(dropSpell.transform.position, originPos, Time.deltaTime / spellPickUpSpeed); // shift book to position
            yield return new WaitForEndOfFrame();
        }
    }

    public IEnumerator fireCoolDown(float duration) // process cool down
    {
        canFire = false;
        float recovery = 0f;
        while(recovery < duration) {
            reticule.fillAmount = recovery / duration; // update reticule to show progress
            recovery += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        reticule.fillAmount = 1f;
        canFire = true;
    }

    #endregion
}
