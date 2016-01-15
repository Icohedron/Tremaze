using UnityEngine;
using System.Collections;

/**
 * The Door class is responsible for the opening and closing of a Door GameObject.
 */
[RequireComponent(typeof(Animator))]
public class Door : MonoBehaviour {

	private Animator animator;

	public bool DoorOpen;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start() {
		animator = GetComponent<Animator>();
		DoorOpen = false;
	}

	/**
	 * Opens the door by playing the Door Open animation.
	 */
	public void Open() {
		DoorOpen = true;
		animator.Play("Door Open", -1);
	}

	/**
	 * Closes the door by playing the Door Close animation.
	 */
	public void Close() {
		DoorOpen = false;
		animator.Play ("Door Close", -1);
	}
}
