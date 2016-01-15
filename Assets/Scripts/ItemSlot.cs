using UnityEngine;
using UnityEngine.EventSystems;

/**
 * The ItemSlot class is responsible for the dragging and dropping of items.
 */
public class ItemSlot : MonoBehaviour, IDropHandler {

    public int ID { get; set; }
	public int AllowedItemType { get; set; }

    private Inventory inventory;
    
	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        inventory = this.transform.parent.GetComponent<Inventory>();
    }

	/**
	 * The Update method is called auotmatically by Monobehaviours once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
    private void Update() {
		// Double-checks to make sure the inventory's database of items and itemslots are up-to-date.
		// This fixed some horrible bug (whether Unity Engine related or code related) that was extremely difficult to track down the source of.
        if (this.GetComponentInChildren<ItemData>() != null) {
            if (inventory.items[ID] != this.GetComponentInChildren<ItemData>().item) {
                inventory.items[ID] = this.GetComponentInChildren<ItemData>().item;
            }
        } else {
            inventory.items[ID] = new Item();
        }
    }

	/**
	 * Handles the operations that occur when an item is dropped onto the ItemSlot
	 * Called automatically by the IDropHandler class.
	 * 
	 * @param eventData passed in automatically by the IDropHandler
	 */
    public void OnDrop(PointerEventData eventData) {
        ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();

        int droppedItemType = droppedItem.item.Type;
        int droppedItemSlotAllowedType = droppedItem.inventory.itemSlots[droppedItem.Slot_ID].GetComponent<ItemSlot>().AllowedItemType;
        int thisItemType = inventory.items[ID].Type;

		// Checking for item-type mismatches
        if (AllowedItemType != (int)ItemDatabase.ItemType.Undefined) {
            if (droppedItemType != AllowedItemType) {
                return;
            }
        }
        if (droppedItemSlotAllowedType != (int)ItemDatabase.ItemType.Undefined) {
            if (thisItemType != droppedItemSlotAllowedType && thisItemType != (int)ItemDatabase.ItemType.Undefined) {
                return;
            }
        }

		// If this item slot is empty, place the dropped item into this slot.
        if (this.GetComponentInChildren<ItemData>() == null) {
            droppedItem.inventory.items[droppedItem.Slot_ID] = new Item();
			droppedItem.inventory = this.inventory;
            droppedItem.Slot_ID = ID;
            inventory.items[ID] = droppedItem.item;
        } else {
			// If this item slot contains an item of the same ID as the dropped item, combine the stacks
			if (this.GetComponentInChildren<ItemData>().item.ID == droppedItem.item.ID && this.GetComponentInChildren<ItemData>().item.Stackable) {
		        ItemData itemToCompare = this.GetComponentInChildren<ItemData>();
		        int sumAmount = itemToCompare.amount + droppedItem.amount;
		        if (sumAmount > 9) {
		            int maxTransfer = ItemData.MAX_STACK - itemToCompare.amount;
		            itemToCompare.amount += maxTransfer;
		            droppedItem.amount -= maxTransfer;
		        } else {
		            inventory.items[droppedItem.Slot_ID] = new Item();
		            Destroy(droppedItem.gameObject);
		            itemToCompare.amount = sumAmount;
		        }
			// else, swap the two items
			} else {
		        ItemData toSwap = this.GetComponentInChildren<ItemData>();
		        toSwap.Slot_ID = droppedItem.Slot_ID;
				droppedItem.inventory.items[droppedItem.Slot_ID] = toSwap.item;
				toSwap.transform.SetParent(droppedItem.inventory.itemSlots[toSwap.Slot_ID].transform);
				toSwap.transform.position = toSwap.transform.parent.transform.position;
				droppedItem.Slot_ID = ID;
				inventory.items[ID] = droppedItem.item;

				toSwap.inventory = droppedItem.inventory;
				droppedItem.inventory = this.inventory;
			}
    	}
	}
}