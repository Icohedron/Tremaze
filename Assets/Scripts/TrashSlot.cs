using UnityEngine;
using UnityEngine.EventSystems;

/**
 * The TrashSlot class is like a ItemSlot, except it deletes any items that are dragged onto it.
 */
public class TrashSlot : MonoBehaviour, IDropHandler {

	/**
	 * Handles the operations that occur when an item is dropped onto the TrashSlot
	 * Called automatically by the IDropHandler class.
	 * 
	 * @param eventData passed in automatically by the IDropHandler
	 */
    public void OnDrop(PointerEventData eventData) {
		ItemData droppedItem = eventData.pointerDrag.GetComponent<ItemData>();
        droppedItem.inventory.items[droppedItem.Slot_ID] = new Item();
        Destroy(eventData.pointerDrag);
    }
}
