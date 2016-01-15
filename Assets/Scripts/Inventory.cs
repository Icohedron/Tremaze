using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/**
 * The Inventory class is responsible for managing all inventories in the game.
 */
public abstract class Inventory : MonoBehaviour {

	public int Number_of_Slots;

	[SerializeField]
	protected GameObject Item_Slot;

	[SerializeField]
	protected GameObject _Item;

	protected ItemDatabase itemDatabase;

	public List<Item> items;
	public List<GameObject> itemSlots;
	
	/**
	 * Adds an item slot to the inventory.
	 * 
	 * @param index the index of the item slot
	 * @param itemType optional: the allowed item type of the item slot
	 */
	protected void AddSlot(int index, int itemType = -1) {
		items.Add(new Item());
		itemSlots.Add(Instantiate(Item_Slot));
		itemSlots[index].transform.SetParent(this.transform);
		itemSlots[index].GetComponent<ItemSlot>().ID = index;
		itemSlots[index].GetComponent<ItemSlot>().AllowedItemType = itemType;
		itemSlots[index].name = "Slot " + index;
	}

	/**
	 * Looks for and returns the index of an item with a specified target ID.
	 * 
	 * @param ID the ID of the item
	 * @param start optional: which index to start the search from
	 * @return the index of the item that matches the ID specified
	 */
	public int GetIndexOf(int ID, int start = 0) {
		for (int i = start; i < Number_of_Slots; i++) {
			if (items[i].ID == ID) {
				return i;
			}
		}
		for (int i = start + Number_of_Slots; i < Number_of_Slots; i++) {
			if (items[i].ID == ID) {
				return i;
			}
		}
		return -1;
	}

	/**
	 * Adds an item to first empty item slot.
	 * 
	 * @param ID the ID of the item to be added
	 * @param amount optional: the amount of the item to be added
	 * @param dontStackWithExisting optional: to stack or not to stack with existing items
	 * @return whether the operation failed or completed successfully
	 */
    public bool AddItem(int ID, int amount = 1, bool dontStackWithExisting = true) {
		Item item = itemDatabase.FetchItemByID(ID);
		int existingItemSlot = GetIndexOf(ID);
		if (item.Stackable && existingItemSlot != -1 && dontStackWithExisting == false) {
			ItemData itemData = itemSlots[existingItemSlot].transform.GetChild(0).GetComponent<ItemData>();
			itemData.amount += amount;
            return true;
        } else {
			for (int i = 0; i < Number_of_Slots; i++) {
				if (items[i].ID == (int)ItemDatabase.ItemID.Undefined) {
					items[i] = item;
					GameObject itemObject = Instantiate(_Item);
					ItemData itemData = itemObject.GetComponent<ItemData>();
					itemObject.transform.SetParent(itemSlots[i].transform);
					itemObject.GetComponent<Image>().sprite = item._Sprite;
                    itemObject.transform.position = itemObject.transform.parent.position;
                    itemObject.name = item.Name;
					itemData.item = item;
					itemData.amount = amount;
                    itemData.Slot_ID = i;
                    return true;
				}
			}
		}
        return false;
	}

	/**
	 * Removes an item from the inventory of a specific ID
	 * 
	 * @param ID the ID of the item to be removed
	 * @param amount optional: the amount to be removed
	 * @return whether the operation failed or completed successfully.
	 */
	public bool RemoveItemFromBackpack(int ID, int amount = 1) {
		int existingItemSlot = GetIndexOf(ID);
		ItemData itemData = itemSlots[existingItemSlot].transform.GetChild(0).GetComponent<ItemData>();
		itemData.amount -= amount;
		if (itemData.amount <= 0) {
			items[existingItemSlot] = new Item();
            Destroy(itemSlots[existingItemSlot].GetComponentInChildren<ItemData>().gameObject);
            return true;
		}
        return false;
	}

	/**
	 * Returns the number of empty item slots
	 * 
	 * @return the number of empty item slots
	 */
    public int EmptySlotsLeft() {
        int nullItemCount = 0;
        for (int i = 0; i < Number_of_Slots; i++) {
            if (items[i].ID == (int)ItemDatabase.ItemID.Undefined) {
                nullItemCount++;
            }
        }
        return nullItemCount;
    }

	/**
	 * Hides all item tooltips that are currently visible
	 */
    public void HideAllTooltips() {
        for (int i = 0; i < itemSlots.Count; i++) {
            if (itemSlots[i].GetComponentInChildren<ItemData>() == null) {
                continue;
            }
            itemSlots[i].GetComponentInChildren<ItemData>().OnPointerExit(null);
        }
    }

	/**
	 * Returns whether or not the inventory contains the specified item type.
	 * 
	 * @param type the item type to look for
	 * @return whether the inventory contains the item or not
	 */
    public bool ContainsItemType(int type) {
        for (int i = 0; i < Number_of_Slots; i++) {
            if (items[i].Type == type) {
                return true;
            }
        }
        return false;
    }
}
