using UnityEngine;
using System.Collections.Generic;

/**
 * The ChestInventory class is responsible for managing the inventory of a Chest GameObject.
 */
public class ChestInventory : Inventory {

    [SerializeField]
    private Vector3 position;

    [SerializeField]
    private Vector3 headerPosition;

    public GameObject header;

	/**
	 * A manually-called constructor.
	 */
    public void Initialize() {
        itemDatabase = this.transform.parent.GetComponent<ItemDatabase>();
        items = new List<Item>();
        itemSlots = new List<GameObject>();

		// Fills the inventory with empty item slots
        for (int i = 0; i < Number_of_Slots; i++) {
            items.Add(new Item());
            AddSlot(i);
        }
    }

	/**
	 * Shows the title of the ChestInventory UI element.
	 */
    public void ShowHeader() {
        header.SetActive(true);
    }

	/**
	 * Hides the title of the ChestInventory UI element.
	 */
    public void HideHeader() {
        header.SetActive(false);
    }

	/**
	 * Resets the position of the ChestInventory UI element and its header.
	 */
    public void ResetPosition() {
        this.transform.localPosition = position;
        header.transform.localPosition = headerPosition;
    }
}
