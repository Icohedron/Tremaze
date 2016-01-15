using UnityEngine;
using System.Collections;

/**
 * The PlayerController class is responsible for taking in user input and keeping track of other Player-related tasks.
 * The PlayerController does all the updates that are based on user input, while the Player class does all
 * the updates to the Player's Rigidbody.
 */
[RequireComponent(typeof(Player), typeof(AudioSource))]
public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float Speed = 8.0f;

	[SerializeField]
	private float SprintSpeedMultiplier = 1.5f;

	[SerializeField]
	private float SprintFOVKick = 10.0f;

	[SerializeField]
	private float MouseSensitivity = 2.0f;

	[SerializeField]
	private float CameraBobSpeed = 0.18f;
	
	[SerializeField]
	private float CameraBobAmount = 0.08f;
	
	[SerializeField]
	private float CameraBobMidpoint = 0.8f;

    [SerializeField]
    private float RayCastDistance = 3.0f;

    [SerializeField]
    private GameObject Dagger;

    [SerializeField]
    private GameObject Sword;

    [SerializeField]
    private AudioClip[] footstepSounds;

    private AudioSource audioSource;
	private float bobTimer;

	private Player player;
	private Equipment equipment;
	private Viewmodel viewmodel;
    private Animation viewmodelAnimation;

    private Chest lastChest;
    private RaycastHit raycastHit;

    private GUIController guiController;

	/**
	 * The Start method is called automatically by Monobehaviors,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
	private void Start() {
		bobTimer = 0.0f;
		player = GetComponent<Player> ();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		equipment = GameObject.Find("Equipment").GetComponent<Equipment>();
        viewmodel = null;

        guiController = GameObject.Find("Graphical User Interface").GetComponent<GUIController>();
        audioSource = GetComponent<AudioSource>();
    }

	/**
	 * The Update method is called automatically by Monobehaviors once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
	private void Update() {
        if (Input.GetKeyDown(KeyCode.I)) {
            if (CursorLocked()) {
                UnlockCursor();
            } else {
                HideAllUIElements();
                LockCursor();
            }
        }

        if (CursorLocked()) {
            calculatePlayerLookDirection();
            checkPlayerInteractions();
        } else {
            standstill();
            checkPlayerStandStillInteractions();
        }

        checkRangeOfChests();
        updateViewmodel();
    }

	/**
	 * The FixedUpdate method is called automatically by Monobehaviors once every fixed number of frames.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">docs.unity3d.com</a> for more information.
	 */
    private void FixedUpdate() {
        float speed = calculateSpeed();
        float sprintMultiplier = calculateMovement(speed);
        calculateCameraBob(sprintMultiplier);
    }

	/**
	 * Creates a floating model of the weapon that is in the player's hand.
	 */
    private void updateViewmodel() {
        if (viewmodel == null || equipment.items[0].ID != viewmodel.ItemID) {
            if (equipment.items[0].ID == (int)ItemDatabase.ItemID.Dagger) {
                setViewmodel(Dagger, (int)ItemDatabase.ItemID.Dagger);
            } else if (equipment.items[0].ID == (int)ItemDatabase.ItemID.Sword) {
                setViewmodel(Sword, (int)ItemDatabase.ItemID.Sword);
            } else if (equipment.items[0].ID == (int)ItemDatabase.ItemID.Undefined) {
                setViewmodel(null, -1);
            }
        }
    }

	/**
	 * Calculates the new speed of the player based on Equipment stats.
	 * 
	 * @return the speed of the player
	 */
    private float calculateSpeed() {
        float speed = Speed;
        for (int i = 0; i < equipment.items.Count; i++) {
            speed += equipment.items[i].SpeedModifier;
        }
        return speed;
    }

	/**
	 * Calculates the amount of damage the player can deal based on Equipment stats.
	 * 
	 * @return damage dealt by the player
	 */
	private float calculateDamage() {
		float damage = equipment.items[0].Damage;
		return damage;
	}

	/**
	 * Calculates how much damage gets negated when the player takes damage
	 * based on Equipment stats.
	 * 
	 * @return the amount of protection the player has
	 */
    private float calculateProtection() {
        float protection = 0.0f;
        for (int i = 0; i < equipment.items.Count; i++) {
            protection += equipment.items[i].Protection;
        }
        return protection;
    }

	/**
	 * Moves the player according to user input on the Vertical and Horizontal axis.
	 * 
	 * @param speed the speed of the player
	 * @return the sprint multiplier as a result of the player holding the sprint key
	 */
    private float calculateMovement(float speed) {
        float vInput = Input.GetAxisRaw("Vertical");
        float hInput = Input.GetAxisRaw("Horizontal");

        float sprintMultiplier = 1.0f;
        if (Input.GetKey(KeyCode.LeftShift) && vInput > 0.0f) {
            sprintMultiplier = SprintSpeedMultiplier;
            player.SetCameraFOV(player.CameraFOV + SprintFOVKick, speed * SprintSpeedMultiplier * Time.deltaTime * 10.0f);
        } else {
            player.SetCameraFOV(player.CameraFOV, speed * SprintSpeedMultiplier * Time.deltaTime * 10.0f);
        }

        Vector3 forwardVelocity = this.transform.forward * vInput;
        Vector3 rightwardVelocity = this.transform.right * hInput;

        Vector3 netVelocity = (forwardVelocity + rightwardVelocity).normalized * speed * sprintMultiplier;
        player.Move(netVelocity);

        return sprintMultiplier;
    }

	/**
	 * Calculates the Camera's "bob" when the player moves.
	 * Sprinting makes the camera bob faster.
	 * 
	 * Also plays a footstep AudioClip when the Camera reaches near the lowest point of the bob.
	 * 
	 * @param sprintMultiplier the amount the bobbing is sped up
	 */
    private void calculateCameraBob(float sprintMultiplier) {
        float waveslice = 0.0f;
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        if (Mathf.Abs(horizontal) == 0 && Mathf.Abs(vertical) == 0) {
            bobTimer = 0.0f;
        } else {
            waveslice = Mathf.Sin(bobTimer);
            bobTimer += CameraBobSpeed * sprintMultiplier;
            if (bobTimer > Mathf.PI * 2) {
                bobTimer -= Mathf.PI * 2;
                playFootstepSound();
            }
        }

        if (waveslice != 0) {
            float deltaTranslation = waveslice * CameraBobAmount;
            float netTranslation = Mathf.Abs(vertical) + Mathf.Abs(horizontal);
            netTranslation = Mathf.Clamp(netTranslation, 0.0f, 1.0f);
            deltaTranslation *= netTranslation;
            player.BobCamera(new Vector3(0.0f, CameraBobMidpoint + deltaTranslation, 0.0f));
            if (viewmodel != null) {
                viewmodel.Bob(new Vector3(viewmodel.DefaultPosition.x, viewmodel.DefaultPosition.y + deltaTranslation * 0.5f, viewmodel.DefaultPosition.z + deltaTranslation * 0.5f));
            }
        } else {
            player.BobCamera(new Vector3(0.0f, CameraBobMidpoint, 0.0f));
            if (viewmodel != null) {
                viewmodel.Bob(new Vector3(viewmodel.DefaultPosition.x, viewmodel.DefaultPosition.y, viewmodel.DefaultPosition.z));
            }
        }
    }

	/**
	 * Checks user input in actions other than movement and mouse input.
	 */
    private void checkPlayerInteractions() {
        // Attacking via raycasting
        if (Input.GetMouseButtonDown(0) && viewmodel != null && !viewmodelAnimation.isPlaying) {
			viewmodelAnimation.Play();
			RaycastHit rayHit;
			Debug.DrawRay(player._Camera.transform.position, player._Camera.transform.forward * RayCastDistance, Color.magenta);
			if (Physics.Raycast(player._Camera.transform.position, player._Camera.transform.forward, out rayHit, RayCastDistance)) {
				
				if (rayHit.collider.gameObject.name.Contains("Monster")) {
					Monster m = rayHit.collider.gameObject.GetComponent<Monster>();
					StartCoroutine(m.TakeDamage(calculateDamage()));
				}
				
			}
        }

        // Interacting with chests via raycasting
        if (Input.GetKeyDown(KeyCode.E)) {
            RaycastHit rayHit;
            Debug.DrawRay(player._Camera.transform.position, player._Camera.transform.forward * RayCastDistance, Color.magenta);
            if (Physics.Raycast(player._Camera.transform.position, player._Camera.transform.forward, out rayHit, RayCastDistance)) {

                if (rayHit.collider.gameObject.name.Contains("Chest")) {
                    // Opening the chest
                    if (lastChest != null && lastChest != rayHit.collider.gameObject.GetComponent<Chest>()) {
                        lastChest.Close();
                    }
                    lastChest = rayHit.collider.gameObject.GetComponent<Chest>();
                    guiController.HideAll();
                    lastChest.Open();
                    UnlockCursor();
                } else if (rayHit.collider.gameObject.name.Contains("Transporter")) {
					rayHit.collider.gameObject.GetComponent<Transporter>().Interact();
                }

            }
        }

        // Opening the shop menu
		if (player.CurrentArea.Equals("Central Area")) {
            if (Input.GetKeyDown(KeyCode.B)) {
                HideAllUIElements();
                guiController.ShowExchange();
                UnlockCursor();
            }
        }

        // Opening the "pause" menu
        if (Input.GetKeyDown(KeyCode.Escape)) {
            HideAllUIElements();
            guiController.ShowGameMenu();
            UnlockCursor();
        }
    }

	/**
	 * Checks player interactions that occur when the user is cursor-locked (standing still)
	 */
    private void checkPlayerStandStillInteractions() {
        // Interacting with chests via Raycasting
        if (Input.GetKeyDown(KeyCode.E)) {
            if (lastChest != null) {
                // Closing the chest
                HideAllUIElements();
                LockCursor();
            }
        }

        // Closing the shop menu
		if (player.CurrentArea.Equals("Central Area")) {
            if (Input.GetKeyDown(KeyCode.B)) {
                if (guiController.ExchangeActive()) {
                    HideAllUIElements();
                    LockCursor();
                } else {
                    HideAllUIElements();
                    guiController.ShowExchange();
                    UnlockCursor();
                }
            }
        }

        // Close all menus
        if (Input.GetKeyDown(KeyCode.Escape)) {
            HideAllUIElements();
            LockCursor();
        }
    }

	/**
	 * Checks user mouse input for looking around.
	 * Rotates the Camera accordingly.
	 */
    private void calculatePlayerLookDirection() {
        float mouseX = Input.GetAxisRaw("Mouse X") * MouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * MouseSensitivity;

        Vector3 rotY = new Vector3(0.0f, mouseX, 0.0f);
        player.Rotate(rotY);

        player.RotateCamera(mouseY);
    }

	/**
	 * If the player is pushed out of the range of the chest it was last
	 * interacting with, it will force-close the chest UI.
	 */
    private void checkRangeOfChests() {
        // Check if the player is still in range of chests
        if (lastChest != null) {
            Vector3 distanceFromLastChest = this.transform.position - lastChest.transform.position;
            if (distanceFromLastChest.magnitude > RayCastDistance + 1.0f) {
                lastChest.Close();
                lastChest = null;
            }
        }
    }

	/**
	 * Things the player does when the cursor is locked (standing still).
	 */
    private void standstill() {
        player.SetCameraFOV(player.CameraFOV, Speed * SprintSpeedMultiplier * Time.deltaTime * 10.0f);
        player.Move(Vector3.zero);
        player.Rotate(Vector3.zero);
        player.RotateCamera(0.0f);
        player.BobCamera(new Vector3(0.0f, CameraBobMidpoint, 0.0f));
        if (viewmodel != null) {
            viewmodel.Bob(new Vector3(viewmodel.DefaultPosition.x, viewmodel.DefaultPosition.y, viewmodel.DefaultPosition.z));
        }
    }

	/**
	 * Sets the viewmodel of the player.
	 * 
	 * @param model the model to render
	 * @param itemID the item ID of the viewmodel
	 */
    private void setViewmodel(GameObject model, int itemID) {
        if (viewmodel != null) {
            Destroy(viewmodel.gameObject);
            viewmodel = null;
        }

        if (model != null) {
            GameObject newViewmodel = Instantiate(model);
            newViewmodel.transform.SetParent(player._Camera.transform);
            viewmodel = newViewmodel.GetComponent<Viewmodel>();
            newViewmodel.transform.localPosition = viewmodel.DefaultPosition;
            viewmodel.ItemID = itemID;
            viewmodelAnimation = viewmodel.GetComponent<Animation>();
        }
    }

	/**
	 * Plays a random footstep AudioClip.
	 */
    private void playFootstepSound() {
		// Picks a random index between 1 and the footstepSounds.length
		// Plays the AudioClip at the index, then moves it to index 0
		// as to not let the same AudioClip play twice in a row.
        int n = Random.Range(1, footstepSounds.Length);
        audioSource.clip = footstepSounds[n];
        audioSource.PlayOneShot(audioSource.clip);
        footstepSounds[n] = footstepSounds[0];
        footstepSounds[0] = audioSource.clip;
    }

	/**
	 * Makes the player take damage--passes it down to the Player class
	 * 
	 * @see Player
	 */
    public void TakeDamage(float damage, Vector3 position, float knockbackForce, float radius) {
        player.TakeDamage(damage - calculateProtection(), position, knockbackForce, radius);
    }

	/**
	 * Hides the player's non-default HUD elements. (chests, menus, etc.)
	 */
    public void HideAllUIElements() {
        guiController.HideAll();
        if (lastChest != null) {
            lastChest.Close();
            lastChest = null;
        }
    }

	/**
	 * Returns whether or not the user is cursor-locked.
	 * 
	 * @return boolean true or false
	 */
    public bool CursorLocked() {
        return Cursor.visible == false;
    }

	/**
	 * Locks and hides the user's cursor.
	 */
    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

	/**
	 * Unlocks and shows the user's cursor.
	 */
    public void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
