using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/**
 * The Exchange class is responsible for the sale of items from the shop to the player,
 * notifying the player of the requirements needed to buy the item(s) and keeping track
 * of the shop's ShopItems.
 */
[RequireComponent(typeof(ItemDatabase))]
public class Exchange : MonoBehaviour {

    [SerializeField]
    private GameObject Shop_Item_Slot;

    [SerializeField]
    private GameObject Shop_Item;

    [SerializeField]
    private Notification notification;

    [SerializeField]
    private List<int> Items_To_Sell = new List<int>();

    private ItemDatabase itemDatabase;
    private GameObject shop;
    private Backpack backpack;

    public List<ShopItem> shopItems = new List<ShopItem>();

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        itemDatabase = GetComponent<ItemDatabase>();

        shop = this.transform.FindChild("Shop").gameObject;
        backpack = this.transform.parent.FindChild("Inventory").FindChild("Backpack").GetComponent<Backpack>();

        for (int i = 0; i < Items_To_Sell.Count; i++) {
            Item item = itemDatabase.FetchItemByID(Items_To_Sell[i]);
            GameObject itemSlot = Instantiate(Shop_Item_Slot);
            itemSlot.transform.SetParent(shop.transform);
            itemSlot.name = "Shop Item Slot " + i;

            GameObject itemObject = Instantiate(Shop_Item);
            ShopItem shopItem = itemObject.GetComponent<ShopItem>();
            itemObject.transform.SetParent(itemSlot.transform);
            itemObject.GetComponent<Image>().sprite = item._Sprite;
            itemObject.transform.position = Vector2.zero;
            itemObject.name = item.Name;
            shopItem.item = item;
            shopItems.Add(shopItem);
        }
    }

	/**
	 * Initiates trade with the Exchange shop.
	 * Gives the player the item that was double-clicked and takes away
	 * treasure of equal value.
	 * It provides change when necessary.
	 * 
	 * @param item the item to be exchanged/traded
	 */
	public void ExchangeItem(Item item) {
        if (backpack.EmptySlotsLeft() < 2) {
            notification.Notify("Please have at least two slots free in your backpack before initiating trade.");
            return;
        }

        float price = item.Value;

		List<ItemData> currencies = new List<ItemData>();
		float invValue = 0.0f;
		for (int i = 0; i < backpack.items.Count; i++) {
			if (backpack.items[i].Type != 0) {
				continue;
			}
			currencies.Add(backpack.itemSlots[i].GetComponentInChildren<ItemData>());
			invValue += backpack.items[i].Value * backpack.itemSlots[i].GetComponentInChildren<ItemData>().amount;
		}

		if (invValue < item.Value) {
            notification.Notify("Not enough treasure in backpack!");
			return;
		}

        List<int> amountsOfRemoval = new List<int>();
        for (int i = 0; i < currencies.Count; i++) {
            amountsOfRemoval.Add((int)(price / currencies[i].item.Value));
            if (currencies[i].amount < amountsOfRemoval[i]) {
                amountsOfRemoval[i] = currencies[i].amount;
            }
            price -= amountsOfRemoval[i] * currencies[i].item.Value;
		}

        for (int i = 0; i < currencies.Count; i++) {
			backpack.RemoveItemFromBackpack(currencies[i].item.ID, amountsOfRemoval[i]);
        }

        if (price != 0.0f) {
			backpack.RemoveItemFromBackpack((int)ItemDatabase.ItemID.Diamond);
			backpack.AddItem((int)Random.Range((int)ItemDatabase.ItemID.Ruby, (int)ItemDatabase.ItemID.Sapphire + 1));
        }

		backpack.AddItem(item.ID);
	}

	/**
	 * Hides all item tooltips.
	 */
    public void HideAllTooltips() {
        for (int i = 0; i < shopItems.Count; i++) {
            shopItems[i].OnPointerExit(null);
        }
    }
}
