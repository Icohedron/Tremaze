using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

/**
 * The ShopItem class is like an ItemData class, but behaves differently in that
 * it initiates an exchange between the shop and the player upon double-click, and is not meant to be dragged and dropped.
 */
public class ShopItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {

    public Item item;

    private Exchange exchange;
    private GameObject tooltip;

	private float clickDelta;

	/**
	 * The Start method is called automatically by Monobehaviors,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        exchange = this.transform.parent.parent.GetComponentInParent<Exchange>();
        tooltip = this.transform.FindChild("Item Tooltip").gameObject;
		tooltip.SetActive(false);
        BuildTooltipText();
		clickDelta = 0.0f;
    }

	/**
	 * The OnPointerClick method is called automatically by the IPointerClickHandler
	 * every time the GameObject is clicked by the mouse cursor.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/EventSystems.IPointerClickHandler.html">docs.unity3d.com</a> for more information.
	 * 
	 * @param eventData passed in by the IPointerClickHandler automatically
	 */
	public void OnPointerClick(PointerEventData eventData) {
		if (clickDelta == 0.0f) {
			clickDelta = Time.time;
			return;
		}

		if (Time.time - clickDelta < 0.8f) {
            exchange.ExchangeItem(this.item);
		}

		clickDelta = 0.0f;
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
        tooltip.transform.SetParent(this.transform);
        tooltip.SetActive(false);
    }

	/**
	 * Creates the tooltip of the item and its data.
	 */
    private void BuildTooltipText() {
        StringBuilder sb = new StringBuilder(item.Name + "\n" + item.Description);
        if (item.Type >= 2 && item.Type <= 5) {
            sb.Append("\n\nProtection: " + item.Protection.ToString()
                + "\nSpeed Modifier: " + item.SpeedModifier.ToString());
        } else if (item.Type == 1) {
            sb.Append("\n\nDamage: " + item.Damage.ToString()
                + "\nSpeed Modifier: " + item.SpeedModifier.ToString());
        }
        sb.Append("\n\nValue: " + item.Value.ToString());
        tooltip.transform.FindChild("Tooltip").GetComponent<Text>().text = sb.ToString();
    }
}
