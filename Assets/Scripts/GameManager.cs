using UnityEngine;

/**
 * The GameManager class is responsible for keeping track of most
 * of the game's states. It spawns monsters, keeps track of player score,
 * player deaths, number of days passed, and initiates the end game.
 */
public class GameManager : MonoBehaviour {

	[SerializeField]
	private Transform MonsterSpawnPoints;

	[SerializeField]
	private Transform Monsters;

	[SerializeField]
	private GameObject Monster;

    [SerializeField]
    private int MaximumNumberOfMonsters = 5;

    [SerializeField]
	private Player player;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private Backpack PlayerInventory;

    [SerializeField]
    private Transform PlayerChests;

    [SerializeField]
    private Transform Chests;

	[SerializeField]
	private Transform Doors;

    [SerializeField]
    private Notification notification;

    [SerializeField]
    private int MaximumDaysForBonus = 4;

    [SerializeField]
    private int DaysPointsBonus = 10000;

    [SerializeField]
    private int TreasurePointsBonusMultiplier = 5;

    [SerializeField]
    private int PlayerDeathPointDeduction = 500;

    [SerializeField]
    private GUIController guiController;

    [SerializeField]
    private AudioSource MusicPlayer;

    [SerializeField]
    private AudioClip DayMusic;

    [SerializeField]
    private AudioClip NightMusic;

    public int PlayerDeaths;

    private Orbit sun;
    private int numberOfDaysUsed;

    private bool notifyNight;
    private int cPlayerDeaths;

    private bool northAreaEmpty;
    private bool eastAreaEmpty;
    private bool southAreaEmpty;
    private bool westAreaEmpty;

	/**
	 * The Start method is called automatically by Monobehaviours,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        sun = GameObject.Find("Sun").GetComponent<Orbit>();
        notifyNight = false;

        northAreaEmpty = false;
        eastAreaEmpty = false;
        southAreaEmpty = false;
        westAreaEmpty = false;

        PlayerDeaths = 0;
        numberOfDaysUsed = 0;
    }

	/**
	 * The Update method is called automatically by Monobehaviours once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
	private void Update() {
        checkRemainingTreasure();
        doSunlightDependentUpdates();
	}

	/**
	 * Periodically checks for any remaining treasure in the player's current area.
	 * If no treasure is found, it will notify the player that all treasure has been
	 * found in their current area.
	 */
    private void checkRemainingTreasure() {
		if (player.CurrentArea.Equals("Central Area") || player.CurrentArea.Equals("")) {
            return;
        }

        int totalFull = 0;
		for (int i = 0; i < Chests.FindChild(player.CurrentArea).childCount; i++) {
			if (!Chests.FindChild(player.CurrentArea).GetChild(i).gameObject.activeSelf) {
				continue;
			}
			if (Chests.FindChild(player.CurrentArea).GetChild(i).GetComponent<Chest>().inventory.ContainsItemType((int)ItemDatabase.ItemType.Treasure)) {
                totalFull++;
            }
        }

		if (player.CurrentArea.Equals("North Area")) {
            if (totalFull == 0) {
                if (northAreaEmpty == false) {
                    notification.Notify("All available treasure in the North Area was found.");
                }
                northAreaEmpty = true;
            } else {
                northAreaEmpty = false;
            }
		} else if (player.CurrentArea.Equals("East Area")) {
            if (totalFull == 0) {
                if (eastAreaEmpty == false) {
                    notification.Notify("All available treasure in the East Area was found.");
                }
                eastAreaEmpty = true;
            } else {
                eastAreaEmpty = false;
            }
		} else if (player.CurrentArea.Equals("South Area")) {
            if (totalFull == 0) {
                if (southAreaEmpty == false) {
                    notification.Notify("All available treasure in the South Area was found.");
                }
                southAreaEmpty = true;
            } else {
                southAreaEmpty = false;
            }
		} else if (player.CurrentArea.Equals("West Area")) {
            if (totalFull == 0) {
                if (westAreaEmpty == false) {
                    notification.Notify("All available treasure in the West Area was found.");
                }
                westAreaEmpty = true;
            } else {
                westAreaEmpty = false;
            }
        }
    }

	/**
	 * Executes updates that happen based on the time of day.
	 * Notifying the player of day and night time starts, and spawns and kills the Monsters.
	 */
    private void doSunlightDependentUpdates() {
        if (!sun.IsUp()) {
            if (notifyNight == false) {
                notification.Notify("The night has begun; the maze doors are now closing.");
                MusicPlayer.Stop();
                numberOfDaysUsed++;
                notifyNight = true;
                cPlayerDeaths = PlayerDeaths;
            }

            if (PlayerDeaths - cPlayerDeaths > 0) {
                MusicPlayer.Stop();
                MusicPlayer.PlayOneShot(DayMusic);
                cPlayerDeaths = PlayerDeaths;
            }

            if (!MusicPlayer.isPlaying) {
				if (player.CurrentArea.Equals("Central Area")) {
                    MusicPlayer.PlayOneShot(DayMusic);
                } else {
                    MusicPlayer.PlayOneShot(NightMusic);
                }
            }

			if (!player.CurrentArea.Equals("Central Area")) {
                spawnMonsters();
            }

            for (int i = 0; i < Doors.childCount; i++) {
                if (Doors.GetChild(i).GetComponentInChildren<Door>().DoorOpen) {
                    Doors.GetChild(i).GetComponentInChildren<Door>().Close();
                }
            }
        } else {
            if (notifyNight == true) {
                notification.Notify("The day has arrived; the maze doors are now opening.");
                MusicPlayer.Stop();
                notifyNight = false;
            }

            if (!MusicPlayer.isPlaying) {
                MusicPlayer.PlayOneShot(DayMusic);
            }

            for (int i = 0; i < Monsters.childCount; i++) {
                Monster m = Monsters.GetChild(i).GetComponent<Monster>();
                if (!m.Dead) {
                    StartCoroutine(Monsters.GetChild(i).GetComponent<Monster>().Die());
                }
            }

            for (int i = 0; i < Doors.childCount; i++) {
                if (!Doors.GetChild(i).GetComponentInChildren<Door>().DoorOpen) {
                    Doors.GetChild(i).GetComponentInChildren<Door>().Open();
                }
            }
        }
    }

	/**
	 * Returns the score that the player achieved based on the amount of treasure found,
	 * the number of player deaths, and achievements such as the day bonus and no deaths bonus.
	 * 
	 * @return the score of the player as a float value
	 */
    public float GetScore() {
        float score = 0.0f;

        // Add up the values of the treasure found in player storage
        for (int i = 0; i < PlayerChests.childCount; i++) {
            ChestInventory inventory = PlayerChests.GetChild(i).GetComponent<Chest>().inventory;
            for (int j = 0; j < inventory.Number_of_Slots; j++) {
                if (inventory.items[j].Type == (int)ItemDatabase.ItemType.Treasure) {
                    score += inventory.itemSlots[j].transform.GetChild(0).GetComponent<ItemData>().amount * inventory.items[j].Value * TreasurePointsBonusMultiplier; // Each piece of treasure found is worth 10 * its value
                }
            }
        }

        // Add up values of the treasure found in the player's inventory
        for (int j = 0; j < PlayerInventory.Number_of_Slots; j++) {
            if (PlayerInventory.items[j].Type == (int)ItemDatabase.ItemType.Treasure) {
                score += PlayerInventory.itemSlots[j].GetComponentInChildren<ItemData>().amount * PlayerInventory.items[j].Value * TreasurePointsBonusMultiplier; // Each piece of treasure found is worth 10 * its value
            }
        }

        // Give point bonus for getting all treasure within a number of days
        if (numberOfDaysUsed <= MaximumDaysForBonus && northAreaEmpty && eastAreaEmpty && southAreaEmpty && westAreaEmpty) {
            score += DaysPointsBonus;
        }

        // Deduct points for each player death
        score -= PlayerDeaths * PlayerDeathPointDeduction;

        return score > 0.0f ? score : 0.0f;
    }

	/**
	 * Spawns the monsters in the player's area at random spawn points.
	 */
    private void spawnMonsters() {
        if (Monsters.childCount < MaximumNumberOfMonsters) {
            GameObject monster = Instantiate(Monster);
            monster.SetActive(false);
            monster.transform.SetParent(Monsters);
			monster.transform.position = MonsterSpawnPoints.FindChild(player.CurrentArea).GetChild(Random.Range(0, MonsterSpawnPoints.FindChild(player.CurrentArea).childCount)).transform.position;
            monster.GetComponent<Monster>().Initialize();
            monster.SetActive(true);
        }
    }

	/**
	 * Shows the EndGameMenu (the scoreboard) to the player.
	 */
    public void EndGame() {
        guiController.HideAll();
        guiController.EndGameMenu.SetScoreValue(GetScore());
        guiController.EndGameMenu.SetDayBonus(numberOfDaysUsed <= MaximumDaysForBonus && northAreaEmpty && eastAreaEmpty && southAreaEmpty && westAreaEmpty);
        guiController.EndGameMenu.SetNoDeaths(PlayerDeaths == 0);
        guiController.EndGameMenu.SetTimeValue();
        guiController.ShowEndGameMenu();
        playerController.UnlockCursor();
    }

	/**
	 * Brings the player back to the main menu.
	 */
	public void BackToMainMenu() {
		Application.LoadLevel(0);
	}

	/**
	 * Restarts the game.
	 */
	public void RestartGame() {
		Application.LoadLevel(1);
	}

	/**
	 * Quits the game--closes the application.
	 */
    public void QuitGame() {
        Application.Quit();
    }
}
