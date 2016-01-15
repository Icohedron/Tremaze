using UnityEngine;

/**
 * The Transporter class is responsible for the behavior of the Transporter GameObject.
 */
public class Transporter : MonoBehaviour {

    [SerializeField]
    private GameManager gameManager;

	/**
	 * A callback function when the player interacts with the Transporter
	 */
    public void Interact() {
        gameManager.EndGame();
    }
}
