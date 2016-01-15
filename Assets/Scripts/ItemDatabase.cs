using UnityEngine;
using System.Collections.Generic;

/**
 * The ItemDatabase class is responsible for creating and documenting all items in the game.
 */
public class ItemDatabase : MonoBehaviour {

	public enum ItemType : int
	{
		Undefined = -1,
		Treasure = 0,
		Weapon = 1,
		Helmet = 2,
        Chestpiece = 3,
        Leggings = 4,
        Boots = 5
	}

	public enum ItemID : int
	{
        Undefined = -1,
		Diamond = 0,
		Ruby = 1,
		Emerald = 2,
		Sapphire = 3,
		Dagger = 5,
		Sword = 6,
		Metal_Helmet = 10,
		Metal_Chestpiece = 11,
		Metal_Leggings = 12,
		Metal_Boots = 13,
		Magic_Speed_Boots = 14
	}

	private List<Item> itemsList = new List<Item>();

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start() {
        // Item Database
        itemsList.Add (new Item (0, (int) ItemType.Treasure, "Diamond", "Oooh, shiny! It's probably worth a lot.", true, 0.0f, 0.0f, 0.0f, 15.0f, "Sprites/Gems/Diamond"));
        itemsList.Add (new Item (1, (int) ItemType.Treasure, "Ruby", "A red tear drop of fortune.", true, 0.0f, 0.0f, 0.0f, 5.0f, "Sprites/Gems/Ruby"));
        itemsList.Add (new Item (2, (int) ItemType.Treasure, "Emerald", "Its green color makes it worth quite a bit.", true, 0.0f, 0.0f, 0.0f, 5.0f, "Sprites/Gems/Emerald"));
        itemsList.Add (new Item (3, (int) ItemType.Treasure, "Sapphire", "Blue, the favorite color of many people.", true, 0.0f, 0.0f, 0.0f, 5.0f, "Sprites/Gems/Sapphire"));
//		itemsList.Add (new Item (4));
        itemsList.Add (new Item (5, (int) ItemType.Weapon, "Dagger", "A short, sweet, light-weight weapon.", false, 0.0f, 0.0f, 10.0f, 10.0f, "Sprites/Weapons/icon_dagger1"));
        itemsList.Add (new Item (6, (int) ItemType.Weapon, "Sword", "It's heavy, but sure packs a punch!", false, 0.0f, -1.0f, 25.0f, 25.0f, "Sprites/Weapons/icon_sword_short2"));
//		itemsList.Add (new Item (7));
//		itemsList.Add (new Item (8));
//		itemsList.Add (new Item (9));
        itemsList.Add (new Item (10, (int) ItemType.Helmet, "Metal Helmet", "Most important piece of protection is a helmet.", false, 3.0f, -0.2f, 0.0f, 15.0f, "Sprites/Armor/icon_plate_head1"));
        itemsList.Add (new Item (11, (int) ItemType.Chestpiece, "Metal Chestpiece", "Offers the most protection, but is also the heaviest.", false, 8.0f, -0.8f, 0.0f, 25.0f, "Sprites/Armor/icon_plate_breast"));
        itemsList.Add (new Item (12, (int) ItemType.Leggings, "Metal Leggings", "Legs are crucial to moving around, so protect them!", false, 6.0f, -1.0f, 0.0f, 20.0f, "Sprites/Armor/icon_plate_legs"));
        itemsList.Add (new Item (13, (int) ItemType.Boots, "Metal Boots", "If you find yourself dropping your sword quite a lot, consider buying these boots.", false, 3.0f, -0.5f, 0.0f, 15.0f, "Sprites/Armor/icon_plate_boots1"));
        itemsList.Add (new Item (14, (int) ItemType.Boots, "Magic Speed Boots", "Hmm, maybe magic does exist after all!", false, 1.0f, 4.0f, 0.0f, 50.0f, "Sprites/Armor/icon_LEATHER_boots1"));
	}

	/**
	 * Returns an Item object of a specified ID
	 * 
	 * @param ID the ID of the item
	 * @return the Item object found with that ID, or an empty Item object
	 */
	public Item FetchItemByID(int ID) {
		for (int i = 0; i < itemsList.Count; i++) {
			if (ID == itemsList[i].ID) {
				return itemsList[i];
			}
		}
		return new Item();
	}
}

/**
 * The Item class is responsible for keeping track of all item data.
 */
public class Item {

	public int ID { get; private set; }
	public int Type { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
	public bool Stackable { get; private set; }

	public float Protection { get; private set; }
	public float SpeedModifier { get; private set; }
	public float Damage { get; private set; }
    public float Value { get; private set; }

	public Sprite _Sprite { get; private set; }

	public Item() {
		this.ID = -1;
		this.Type = (int) ItemDatabase.ItemType.Undefined;
		this.Name = "Perfectly Generic Item";
		this.Description = "It's so perfect!";
		this.Stackable = true;
		this.Protection = 0.0f;
		this.SpeedModifier = 0.0f;
		this.Damage = 0.0f;
        this.Value = 0.0f;
		this._Sprite = new Sprite();
	}

	public Item(int ID, int type, string name, string description, bool stackable, float protection, float speedModifier, float damage, float value, string spriteFilePath) {
		this.ID = ID;
		this.Type = type;
		this.Name = name;
		this.Description = description;
		this.Stackable = stackable;
		this.Protection = protection;
		this.SpeedModifier = speedModifier;
		this.Damage = damage;
        this.Value = value;
		this._Sprite = Resources.Load<Sprite>(spriteFilePath);
	}
}