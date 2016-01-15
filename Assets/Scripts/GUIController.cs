using UnityEngine;

/**
 * The GUIController class is responsible for the showing and hiding of most GUI elements.
 */
public class GUIController : MonoBehaviour {

    [SerializeField]
	private GameObject Exchange;

    [SerializeField]
    private GameObject GameMenu;

    public EndMenu EndGameMenu;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start() {
        HideExchange();
    }

	/**
	 * Hides the shop menu.
	 */
    public void HideExchange() {
        Exchange.GetComponent<Exchange>().HideAllTooltips();
        Exchange.SetActive(false);
    }

	/**
	 * Shows the shop menu.
	 */
    public void ShowExchange() {
        Exchange.SetActive(true);
    }

	/**
	 * Returns whether the shop menu is open or not.
	 * 
	 * @return boolean value, true or false
	 */
    public bool ExchangeActive() {
        return Exchange.activeSelf;
    }

	/**
	 * Hides the "pause" menu.
	 */
    public void HideGameMenu() {
        GameMenu.GetComponent<GameMenu>().HideHeader();
        GameMenu.SetActive(false);
    }

	/**
	 * Shows the "pause" menu.
	 */
    public void ShowGameMenu() {
        GameMenu.SetActive(true);
        GameMenu.GetComponent<GameMenu>().ShowHeader();
    }

	/**
	 * Returns whether or not the "pause" menu is open or not.
	 * 
	 * @return boolean value, true or false
	 */
    public bool GameMenuActive() {
        return GameMenu.activeSelf;
    }

	/**
	 * Hides the scoreboard.
	 */
    public void HideEndGameMenu() {
        EndGameMenu.gameObject.SetActive(false);
    }

	/**
	 * Shows the scoreboard.
	 */
    public void ShowEndGameMenu() {
        EndGameMenu.gameObject.SetActive(true);
    }

	/**
	 * Returns whether or not the scoreboard is open or not.
	 */
    public bool EndGameMenuActive() {
        return EndGameMenu.gameObject.activeSelf;
    }

	/**
	 * Hides all GUI elements.
	 */
    public void HideAll() {
        HideExchange();
        HideGameMenu();
        HideEndGameMenu();
    }
}
