using UnityEngine;

/**
 * The Viewmodel class is responsible for the behavior of viewmodel GameObjects.
 * (The Player's viewmodel)
 */
public class Viewmodel : MonoBehaviour {

    public Vector3 DefaultPosition;
    public Vector3 DefaultRotation;

    public int ItemID { get; set; }

	private Vector3 bobAmount;

	/**
	 * The Start method is called automatically by Monobehaviors,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        GetComponent<Animation>().wrapMode = WrapMode.Once;
    }

	/**
	 * The Update method is called automatically by Monobehaviors once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
	private void Update() {
		this.transform.localPosition = bobAmount;
        this.transform.localRotation = Quaternion.Euler(DefaultRotation);
	}

	/**
	 * Moves the viewmodel
	 * 
	 * @param bob the amount to move in a Vector3 format
	 */
	public void Bob(Vector3 bob) {
		bobAmount = bob;
	}
}
