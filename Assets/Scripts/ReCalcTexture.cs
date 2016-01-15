#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

/**
 * The ReCalcTexture class is responsible for making textures tile and repeat to the right size and resolution of the GameObject.
 */
public class ReCalcTexture : MonoBehaviour {

	/**
	 * The Start method is called automatically by Monobehaviors,
	 * essentially becoming the constructor of the class.
	 * <p>
	 * See <a href="http://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html">docs.unity3d.com</a> for more information.
	 */
    private void Start() {
        reCalcTexture();
    }

	/**
	 * Recalculates the textures.
	 */
    public void reCalcTexture() {
        Vector2 SS = GetComponent<Renderer>().material.mainTextureScale;
        SS.x = transform.localScale.x;
        SS.y = transform.localScale.y;
        GetComponent<Renderer>().material.mainTextureScale = SS;
    }
}


// Creates a button to allow the recalculating of textures while in the editor
#if UNITY_EDITOR
[CustomEditor(typeof(ReCalcTexture))]
public class UpdateTextures : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        ReCalcTexture myScript = (ReCalcTexture)target;
        if (GUILayout.Button("Update Texture")) {
            myScript.reCalcTexture();
        }
    }
}
#endif