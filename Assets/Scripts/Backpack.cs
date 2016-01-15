using UnityEngine;
using System.Collections.Generic;

/**
 * The Backpack class is responsible for managing the inventory of the player's backpack.
 * 
 * @see Inventory
 */
public class Backpack : Inventory {

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start() {
		itemDatabase = this.transform.parent.GetComponent<ItemDatabase>();
		items = new List<Item>();
		itemSlots = new List<GameObject>();

		// Fills the inventory with empty item slots
		for (int i = 0; i < Number_of_Slots; i++) {
			items.Add(new Item());
			AddSlot(i);
		}
    }
}
