using UnityEngine;
using UnityEngine.UI;

/**
 * The EndMenu class is responsible for setting the valus of the scoreboard for the player to see.
 * These values can be seen when the player interacts with the Transporter.
 */
public class EndMenu : MonoBehaviour {

    [SerializeField]
    private Toggle DayBonus;

    [SerializeField]
    private Toggle NoDeaths;

    [SerializeField]
    private Text Score;

    [SerializeField]
    private Text TimeText;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        this.gameObject.SetActive(false);
    }

	/**
	 * Sets the DayBonus checkbox to true or false. (checked or unchecked)
	 * 
	 * @param value boolean value, checked or not checked
	 */
    public void SetDayBonus(bool value) {
        DayBonus.isOn = value;
    }

	/**
	 * Sets the NoDeaths checkbox to true or false. (checked or unchecked)
	 * 
	 * @param value boolean value, checked or not checked
	 */
    public void SetNoDeaths(bool value) {
        NoDeaths.isOn = value;
    }

	/**
	 * Sets the score value of the scoreboard.
	 * This shows the number of points that the player has accumulated.
	 * 
	 * @param value number of points
	 */
    public void SetScoreValue(float value) {
        Score.text = value.ToString();
    }

	/**
	 * Sets the time at which the player interacted with the Transporter,
	 * essentially being the time taken for the player to complete the game.
	 */
    public void SetTimeValue() {
        float current = Time.timeSinceLevelLoad;
        int hours = (int) current / 3600;
        int minutes = (int) current / 60 - hours * 60;
        int seconds = (int) current - minutes * 60 - hours * 3600;
        TimeText.text = hours + ":" + minutes + ":" + seconds;
    }
}
