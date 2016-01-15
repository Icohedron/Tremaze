using UnityEngine;
using System.Collections.Generic;

/**
 * The Equipment class is responsible for managing the player's equipment.
 */
public class Equipment : Inventory {

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start () {
		itemDatabase = this.transform.parent.GetComponent<ItemDatabase>();
		items = new List<Item>();
		itemSlots = new List<GameObject>();
		
		for (int i = 0; i < Number_of_Slots; i++) {
			items.Add(new Item());
		}

		AddSlot(0, (int)ItemDatabase.ItemType.Weapon);
		AddSlot(1, (int)ItemDatabase.ItemType.Helmet);
		AddSlot(2, (int)ItemDatabase.ItemType.Chestpiece);
		AddSlot(3, (int)ItemDatabase.ItemType.Leggings);
		AddSlot(4, (int)ItemDatabase.ItemType.Boots);
    }
}
