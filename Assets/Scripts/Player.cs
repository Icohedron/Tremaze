using UnityEngine;
using UnityEngine.UI;

/**
 * The Player class is responsible for manipulating the player's RigidBody and Camera.
 * Also responsible for keeping track of its health.
 */
[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

	public float CameraFOV = 60.0f;
	public Camera _Camera;

	[SerializeField]
	private float Health = 100.0f;

    [SerializeField]
    private Slider HealthBar;

    [SerializeField]
    private GameManager gameManager;

    private Vector3 spawnPoint;

	private Vector3 acceleration;
	private Vector3 rotation;
	private Vector3 cameraBob;
	private float cameraRotation;
	private float netCameraRotationX;
	private float desiredFOV;
	private float deltaFOV;

	private Rigidbody rb;
    private float currentHealth;

	public string CurrentArea;

	/**
	 * The Start method is called automatically by Monobehaviorrs,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
		acceleration = Vector3.zero;
		rotation = Vector3.zero;
		cameraRotation = 0.0f;
		netCameraRotationX = 0.0f;
		desiredFOV = CameraFOV;
		deltaFOV = 0.0f;

		rb = GetComponent<Rigidbody> ();
        currentHealth = Health;
        spawnPoint = this.transform.position;
		CurrentArea = "";
	}

	/**
	 * The Update method is called automatically by MonoBehaviours once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
	private void Update() {
		calculateCameraFOV();
		calculateCameraBob();
	}

	/**
	 * The FixedUpdate method is called automatically by Monobehaviorrs once every fixed number of frame updates.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.FixedUpdate.html">docs.unity3d.com</a> for more information.
	 */
	private void FixedUpdate() {
		calculateMovement();
		calculateRotation();
	}

	/**
	 * The OnTriggerEnter method is called automatically by MonoBehaviours every time it enters a Trigger.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.OnTriggerEnter.html">docs.unity3d.com</a> for more information.
	 */
	private void OnTriggerEnter(Collider other) {
		CurrentArea = other.gameObject.name;
	}

	/**
	 * Calculates the movement of the player.
	 * Applies force to the Rigidbody to accelerate the player.
	 */
	private void calculateMovement() {
		if (acceleration != Vector3.zero) {
			rb.AddForce(acceleration);
		}
	}

	/**
	 * Calculates the rotation of the player and the camera.
	 * Rotates the Rigidbody in the Y axis for looking left and right,
	 * rotates the Camera in its local X axis for looking up and down.
	 * Clamps the maximum angles for up and down to positive and negative 70 degrees.
	 */
	private void calculateRotation() {
		rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
		netCameraRotationX += cameraRotation;
		netCameraRotationX = Mathf.Clamp(netCameraRotationX, -70.0f, 70.0f);
		_Camera.transform.localRotation = Quaternion.Euler(new Vector3(-netCameraRotationX, 0.0f, 0.0f));
	}

	/**
	 * Calculates the field of view for the Camera.
	 */
	private void calculateCameraFOV() {
		if (_Camera.fieldOfView != desiredFOV) {
			_Camera.fieldOfView = Mathf.MoveTowards(_Camera.fieldOfView, desiredFOV, deltaFOV);
		}
	}

	/**
	 * Calculates the bobbing of the Camera--the Camera's local position.
	 */
	private void calculateCameraBob() {
		_Camera.transform.localPosition = cameraBob;
	}

	/**
	 * Sets the acceleration of the player.
	 * 
	 * @param acceleration the amount of force to be applied to the player's RigidBody
	 */
	public void Move(Vector3 acceleration) {
		this.acceleration = acceleration;
	}

	/**
	 * Rotates the player's Rigidbody.
	 * 
	 * @param rotation the rotation amount in degrees
	 */
	public void Rotate(Vector3 rotation) {
		this.rotation = rotation;
	}

	/**
	 * Rotates the Camera.
	 * 
	 * @param degrees the number of degrees to rotate around
	 */
	public void RotateCamera(float degrees) {
		cameraRotation = degrees;
	}

	/**
	 * Sets the field of view of the Camera
	 * 
	 * @param fov the desired field of view
	 * @param delta the maximum change in field of view each update
	 */
	public void SetCameraFOV(float fov, float delta) {
		desiredFOV = fov;
		deltaFOV = delta;
	}

	/**
	 * Sets the local position of the Camera
	 * 
	 * @param bob local position Vector3
	 */
	public void BobCamera(Vector3 bob) {
		cameraBob = bob;
	}

	/**
	 * Deduct health from the player
	 * 
	 * @param damage the amount of health taken
	 * @param position the position of the attacker
	 * @param knockbackForce the amount of force in its knockback
	 * @param radius the radius around the attacker in which the player feels the knockback
	 */
	public void TakeDamage(float damage, Vector3 position, float knockbackForce, float radius) {
        currentHealth -= damage;
        HealthBar.value = currentHealth / Health;
        rb.AddExplosionForce(knockbackForce, position, radius);
		if (currentHealth <= 0.0f) {
            Respawn();
        }
	}

	/**
	 * Resets the player's positiom
	 */
    public void Respawn() {
        this.transform.position = spawnPoint;
        currentHealth = Health;
        HealthBar.value = currentHealth / Health;
        gameManager.PlayerDeaths++;
    }
}
