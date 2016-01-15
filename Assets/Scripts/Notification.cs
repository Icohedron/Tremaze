using UnityEngine;
using UnityEngine.UI;

/**
 * The Notification class is responsible for notifying the player.
 */
[RequireComponent(typeof(Text), typeof(Animator))]
public class Notification : MonoBehaviour {

    private Text text;
    private Animator animator;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        text = GetComponent<Text>();
        animator = GetComponent<Animator>();
    }

	/**
	 * Notifies the player.
	 * 
	 * @param notification the text which you wish to notify the player with
	 */
    public void Notify(string notification) {
        text.text = notification;
        animator.Play("Text Fade", -1, 0.0f);
    }
}
