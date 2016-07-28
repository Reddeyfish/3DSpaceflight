using UnityEngine;
using System.Collections;

public class newMaterial : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Renderer rend = GetComponent<Renderer>();
        Material mat = rend.material;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
