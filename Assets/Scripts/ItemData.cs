using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

/**
 * The ItemData class is reponsible for initiatiing the dragging and dropping
 * of items alongside the ItemSlot class. It also keeps track of the amount
 * and type of data associated with its ItemSlot.
 */
public class ItemData : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler {

	public const int MAX_STACK = 9;

    public int Slot_ID { get; set; }

	public Item item;
	public int amount;

    public Inventory inventory;

	private Text amountText;
    private GameObject tooltip;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start() {
        inventory = this.transform.parent.parent.GetComponentInParent<Inventory>();
        amountText = this.transform.FindChild("Item Amount").GetComponent<Text>();
        tooltip = this.transform.FindChild("Item Tooltip").gameObject;
		tooltip.SetActive(false);
        BuildTooltipText();
	}

	/**
	 * The Update method is called automatically by Monobehaviours once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
	private void Update() {
		if (amount < 2) {
			amountText.text = "";
		} else {
			amountText.text = amount.ToString();
		}
	}

	/**
	 * The OnBeginDrag method is called automatically by the IBeginDragHandler
	 * every time the GameObject is dragged by the mouse.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/EventSystems.IDragHandler.html">docs.unity3d.com</a> for more information.
	 * 
	 * @param eventData passed in by the IBeginDragHandler automatically
	 */
    public void OnBeginDrag(PointerEventData eventData) {
        if (item == null) {
            return;
        }
        this.transform.SetParent(this.transform.parent.parent.parent.parent);
        this.transform.position = eventData.position;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

	/**
	 * The OnnDrag method is called automatically by the IDragHandler
	 * every time the GameObject is dragged by the mouse.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/EventSystems.IDragHandler.html">docs.unity3d.com</a> for more information.
	 * 
	 * @param eventData passed in by the IDragHandler automatically
	 */
    public void OnDrag(PointerEventData eventData) {
        if (item == null) {
            return;
        }
        this.transform.position = eventData.position;
    }

	/**
	 * The OnEndDrag method is called automatically by the IEndDragHandler
	 * every time the GameObject is dragged by the mouse.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/EventSystems.IDragHandler.html">docs.unity3d.com</a> for more information.
	 * 
	 * @param eventData passed in by the IEndDragHandler automatically
	 */
    public void OnEndDrag(PointerEventData eventData) {
        this.transform.SetParent(inventory.itemSlots[Slot_ID].transform);
        this.transform.position = inventory.itemSlots[Slot_ID].transform.position;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
    }

	/**
	 * The OnPointerEnter method is called automatically by the IPointerEnterHandler
	 * every time the GameObject comes into contact with the mouse cursor.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/EventSystems.IPointerEnterHandler.html">docs.unity3d.com</a> for more information.
	 * 
	 * @param eventData passed in by the IPointerEnterHandler automatically
	 */
    public void OnPointerEnter(PointerEventData eventData) {
		Vector3 thisPosition = this.transform.position;
		Vector3 tooltipPosition = new Vector3(thisPosition.x - 125, thisPosition.y + 13, thisPosition.z);
		tooltip.transform.position = tooltipPosition;
        tooltip.transform.SetParent(this.transform.parent.parent.parent.parent);
		tooltip.SetActive(true);
    }

	/**
	 * The OnPointerExit method is called automatically by the IPointerExitHandler
	 * every time the GameObject comes into contact with the mouse cursor.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/EventSystems.IPointerExitHandler.html">docs.unity3d.com</a> for more information.
	 * 
	 * @param eventData passed in by the IPointerExitHandler automatically
	 */
    public void OnPointerExit(PointerEventData eventData) {
        if (tooltip != null) {
            tooltip.transform.SetParent(this.transform);
            tooltip.SetActive(false);
        }
    }

	/**
	 * Creates the tooltip of the item and its data.
	 */
    private void BuildTooltipText() {
        StringBuilder sb = new StringBuilder(item.Name + "\n" + item.Description);
        if (item.Type >= 2 && item.Type <= 5) {
			sb.Append ("\n\nProtection: " + item.Protection.ToString ()
				+ "\nSpeed Modifier: " + item.SpeedModifier.ToString ());
		} else if (item.Type == 1) {
			sb.Append ("\n\nDamage: " + item.Damage.ToString ()
				+ "\nSpeed Modifier: " + item.SpeedModifier.ToString ());
		} else if (item.Type == 0) {
			sb.Append ("\n\nValue: " + item.Value.ToString ());
		}
		tooltip.transform.FindChild("Tooltip").GetComponent<Text>().text = sb.ToString();
    }
}
