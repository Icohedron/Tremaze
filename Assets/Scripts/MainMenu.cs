using UnityEngine;
using System.Collections;

/**
 * The MainMenu class is responsible for making the main menu work.
 */
public class MainMenu : MonoBehaviour {

	[SerializeField]
	private GameObject HowToPlay;

	[SerializeField]
	private GameObject Credits;

	[SerializeField]
	private GameObject LoadingScreen;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start() {
		HowToPlay.SetActive(false);
		Credits.SetActive(false);
		LoadingScreen.SetActive(false);
	}

	/**
	 * The Update method is called automatically by MonoBehaviours once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
	private void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			hideAllMenus();
		}
	}

	/**
	 * A callback method when the PlayGame button is clicked.
	 * It loads the game.
	 */
	public void OnPlayGameButtonClick() {
		LoadingScreen.SetActive(true);
		Application.LoadLevel(1);
	}

	/**
	 * A callback method when the HotToPlay button is clicked.
	 * It shows the player how to play.
	 */
	public void OnHowToPlayButtonClick() {
		hideAllMenus();
		HowToPlay.SetActive(true);
	}

	/**
	 * A callback method when the Credits button is clicked.
	 * It shows the player the game credits.
	 */
	public void OnCreditsButtonClick() {
		hideAllMenus();
		Credits.SetActive(true);
	}

	/**
	 * A callback method when the ExitGame button is clicked.
	 * It closes the game application.
	 */
	public void OnExitGameButtonClick() {
		Application.Quit();
	}

	/**
	 * Hides all menus except the main menu.
	 */
	private void hideAllMenus() {
		HowToPlay.SetActive(false);
		Credits.SetActive(false);
	}
}
