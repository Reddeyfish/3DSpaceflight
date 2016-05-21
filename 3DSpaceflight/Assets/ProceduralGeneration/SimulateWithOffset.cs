using UnityEngine;
using System.Collections;

public class SimulateWithOffset : MonoBehaviour {

    public Transform target;
    public Vector3 offset;

	// Update is called once per frame
	void Update () {
        this.transform.position = target.position + offset;
        this.transform.rotation = target.rotation;
	}
}
