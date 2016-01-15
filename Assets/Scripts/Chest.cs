using UnityEngine;
using System.Collections.Generic;

/**
 * The Chest class is responsible for the opening and closing of a Chest GameObject.
 */
[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class Chest : MonoBehaviour {

    [SerializeField]
    private GameObject ChestInventory;

    [SerializeField]
    private GameObject ChestHeader;

    [SerializeField]
    private AudioClip OpenClip;

    [SerializeField]
    private AudioClip CloseClip;

    [SerializeField]
    private List<int> ItemsToAdd = new List<int>();

    [SerializeField]
    private List<int> ItemAmounts = new List<int>();

    public ChestInventory inventory;

    private Animator animator;
    private AudioSource audioSource;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        animator = GetComponent<Animator>();
        animator.speed = 4.0f;

        audioSource = GetComponent<AudioSource>();

        GameObject _Inventory = GameObject.Find("Inventory");

        inventory = Instantiate(ChestInventory).GetComponent<ChestInventory>();
        inventory.transform.SetParent(_Inventory.transform);
        inventory.Initialize();

        GameObject chestHeader = Instantiate(ChestHeader);
        chestHeader.transform.SetParent(_Inventory.transform);
        inventory.header = chestHeader;

        inventory.ResetPosition();

        for (int i = 0; i < ItemsToAdd.Count; i++) {
            inventory.AddItem(ItemsToAdd[i], ItemAmounts[i]);
        }

        Close();
    }

	/**
	 * Opens the chest by showing the Header (the title of the panel),
	 * setting its ChestInventory UI element to active, plays the box_open animation clip,
	 * and plays the chestopen AudioClip.
	 */
    public void Open() {
        inventory.ShowHeader();
        inventory.gameObject.SetActive(true);
        animator.Play("box_open", -1);
        audioSource.PlayOneShot(OpenClip);
    }

	/**
	 * Closes the chest by hiding the Header (the title of the panel), hiding all visible tooltips,
	 * setting its ChestInventory UI element to inactive, plays the box_close animation clip,
	 * and plays the chestclose AudioClip.
	 */
    public void Close() {
        inventory.HideHeader();
        inventory.HideAllTooltips();
        inventory.gameObject.SetActive(false);
        animator.Play("box_close", -1);
        audioSource.PlayOneShot(CloseClip);
    }

	/**
	 * Toggles between Open and Close states.
	 */
    public void Toggle() {
        if (inventory.gameObject.activeSelf) {
            Close();
        } else {
            Open();
        }
    }
}
