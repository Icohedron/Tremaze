using UnityEngine;

/**
 * The GameMenu class is responsible for showing and hiding the "pause" menu
 * upon the player accessing it via the 'Esc' button.
 */
public class GameMenu : MonoBehaviour {

    [SerializeField]
    private GameObject Header;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        HideHeader();
        this.gameObject.SetActive(false);
    }

	/**
	 * Shows the title of the menu.
	 */
    public void ShowHeader() {
        Header.SetActive(true);
    }

	/**
	 * Hides the title of the menu.
	 */
    public void HideHeader() {
        Header.SetActive(false);
    }
}
