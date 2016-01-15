using UnityEngine;

/**
 * The Orbit class is responsible for making objects rotate around the origin from east to west.
 */
public class Orbit : MonoBehaviour {

    [SerializeField]
    private float Speed = 10.0f;

	/**
	 * The Update method is called automatically by MonoBehaviours once every frame update.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html">docs.unity3d.com</a> for more information.
	 */
    private void Update() {
        this.transform.RotateAround(Vector3.zero, Vector3.right, Speed * Time.deltaTime);
        this.transform.LookAt(Vector3.zero);
    }

	/**
	 * Returns whether the oribitng object is near the horizon.
	 * 
	 * @return boolean true or false
	 */
    public bool IsUp() {
        return this.transform.position.y >= -100.0f;
    }
}
